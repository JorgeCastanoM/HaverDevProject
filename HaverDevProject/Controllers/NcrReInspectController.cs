using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using Microsoft.EntityFrameworkCore.Storage;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using HaverDevProject.CustomControllers;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SkiaSharp;
using NuGet.Protocol;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Quality, Admin")]
    [ActiveUserOnly]
    public class NcrReInspectController : ElephantController
    {
        private readonly HaverNiagaraContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NcrReInspectController(HaverNiagaraContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: NcrReInspect
        public async Task<IActionResult> Index(string SearchCode, int? page, int? pageSizeID, string actionButton,
            DateTime StartDate, DateTime EndDate, string sortDirection = "desc", string sortField = "Created", string filter = "Active")
        {
            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //Set the date range filer based on the values in the database
            if (EndDate == DateTime.MinValue)
            {
                StartDate = _context.NcrQas
                .Min(f => f.NcrQacreationDate.Date);

                EndDate = _context.NcrQas
                .Max(f => f.NcrQacreationDate.Date);

                ViewData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
                ViewData["EndDate"] = EndDate.ToString("yyyy-MM-dd");
            }
            //Check the order of the dates and swap them if required
            if (EndDate < StartDate)
            {
                DateTime temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }

            string[] sortOptions = new[] { "Created", "Acceptable", "Supplier", "NCR #", "Last Updated", "Inspected By", "Phase" };

            var ncrReInspect = _context.NcrReInspects
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Where(n => n.Ncr.NcrPhase != NcrPhase.Archive)
                .AsNoTracking();

            GetNcrs();
            //Filtering values
            if (!String.IsNullOrEmpty(filter))
            {
                if (filter == "All")
                {
                    ViewData["filterApplied:ButtonAll"] = "btn-primary";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else if (filter == "Active")
                {
                    ncrReInspect = ncrReInspect.Where(n => n.Ncr.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";

                }
                else //(filter == "Closed")
                {
                    ncrReInspect = ncrReInspect.Where(n => n.Ncr.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";

                }
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncrReInspect = ncrReInspect.Where(s => s.Ncr.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncrReInspect = ncrReInspect.Where(n => n.Ncr.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncrReInspect = ncrReInspect.Where(n => n.Ncr.NcrQa.NcrQacreationDate >= StartDate &&
                         n.Ncr.NcrQa.NcrQacreationDate <= EndDate);
            }

            //keep track of the number of filters 
            if (numberFilters != 0)
            {
                ViewData["Filtering"] = " btn-danger";
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }

            //Sorting columns
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }

            if (sortField == "Created")
            {
                if (sortDirection == "asc") //asc by default
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Acceptable")
            {
                if (sortDirection == "asc")
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.NcrReInspectAcceptable);
                    ViewData["filterApplied:NcrReInspectAcceptable"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.NcrReInspectAcceptable);
                    ViewData["filterApplied:NcrReInspectAcceptable"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Inspected By")
            {
                if (sortDirection == "asc")
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.NcrReInspectId);
                    ViewData["filterApplied:NcrReInspectUserId"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.NcrReInspectId);
                    ViewData["filterApplied:NcrReInspectUserId"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.Ncr.NcrPhase);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.Ncr.NcrPhase);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    ncrReInspect = ncrReInspect
                        .OrderBy(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:Ncr"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrReInspect = ncrReInspect
                        .OrderByDescending(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:Ncr"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NcrReInspect>.CreateAsync(ncrReInspect.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: NcrReInspect/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NcrReInspects == null)
            {
                return NotFound();
            }

            var ncrReInspect = await _context.NcrReInspects
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.NcrReInspectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Defect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.ItemDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.EngDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.Drawing)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.EngDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.OpDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.FollowUpType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.OpDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement).ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(m => m.NcrReInspectId == id);

            if (ncrReInspect == null)
            {
                return NotFound();
            }

            //Added code to take the NCR id from the newly created NCR and pass it to the viewbag for use in the details partial view
            var ncrId = FindNcrId(ncrReInspect.NcrReInspectNewNcrNumber);            
            ViewBag.NCRId = ncrId;

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = true;

            ViewBag.NCRSectionId = id;
            return View(ncrReInspect);
        }

        // GET: NcrReInspect/Create
        public async Task<IActionResult> Create(string ncrNumber)
        {
            NcrReInspect ncrReInspect;
            int ncrId = _context.Ncrs.Where(n => n.NcrNumber == ncrNumber).Select(n => n.NcrId).FirstOrDefault();                       

            // Check if there are info in cookies
            if (Request.Cookies.TryGetValue("DraftNCRReInspect" + ncrNumber, out string draftJson))
            {
                // Convert the data from json file
                ncrReInspect = JsonConvert.DeserializeObject<NcrReInspect>(draftJson);
                TempData["SuccessMessage"] = "Draft successfully retrieved";
            }
            else
            {
                ncrReInspect = new NcrReInspect
                {
                    NcrId = ncrId,
                    NcrReInspectCreationDate = DateTime.Now,
                    NcrNumber = ncrNumber
                };
            }

            var ncr = await _context.Ncrs
                .Include(n => n.NcrQa)
                          .ThenInclude(item => item.Supplier)
                      .Include(n => n.NcrQa)
                            .ThenInclude(defect => defect.Defect)
                      .Include(n => n.NcrQa)
                            .ThenInclude(i => i.Item)
                      .Include(n => n.NcrQa)
                        .ThenInclude(qa => qa.ItemDefectPhotos)
                      .Include(n => n.NcrEng)
                        .ThenInclude(eng => eng.EngDispositionType)
                      .Include(n => n.NcrEng)
                        .ThenInclude(eng => eng.Drawing)
                      .Include(n => n.NcrEng)
                        .ThenInclude(eng => eng.EngDefectPhotos)
                      .Include(n => n.NcrOperation)
                        .ThenInclude(op => op.OpDispositionType)
                      .Include(n => n.NcrOperation)
                        .ThenInclude(op => op.FollowUpType)
                      .Include(n => n.NcrOperation)
                        .ThenInclude(op => op.OpDefectPhotos)
                      .Include(n => n.NcrProcurement)
                        .ThenInclude(proc => proc.ProcDefectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == ncrId);

            ncrReInspect.Ncr = ncr;

            var readOnlyDetails = await _context.Ncrs
                .Include(n => n.NcrQa)
                        .ThenInclude(item => item.Supplier)
                .Include(n => n.NcrQa)
                            .ThenInclude(defect => defect.Defect)
                .Include(n => n.NcrQa)
                            .ThenInclude(i => i.Item)
                .Include(n => n.NcrQa)
                    .ThenInclude(qa => qa.ItemDefectPhotos)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.EngDispositionType)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.Drawing)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.EngDefectPhotos)
                .Include(n => n.NcrOperation)
                    .ThenInclude(op => op.OpDispositionType)
                .Include(n => n.NcrOperation)
                    .ThenInclude(op => op.FollowUpType)
                .Include(n => n.NcrOperation)
                    .ThenInclude(op => op.OpDefectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == ncrId);

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = true;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            return View(ncrReInspect);
        }

        // POST: NcrReInspect/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NcrReInspect ncrReInspect, List<IFormFile> Photos, IFormCollection form, bool isDraft = false)
        {
            try
            {
                //if (isDraft)
                //{
                //    // convert the object to json format
                //    var json = JsonConvert.SerializeObject(ncrReInspect);

                //    // Save the object in a cookie with name "DraftData"
                //    Response.Cookies.Append("DraftNCRReInspect" + ncrReInspect.NcrNumber, json, new CookieOptions
                //    {
                //        // Define time for cookies
                //        Expires = DateTime.Now.AddMinutes(2880) // Cookied will expire in 48 hrs
                //    });

                //    return Ok(new { success = true, message = "Draft saved successfully.\nNote: This draft will be available for the next 48 hours." });
                //}

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user != null)
                    {
                        ncrReInspect.NcrReInspectUserId = user.Id;
                    }                    

                    string isAcceptable = form["NcrReInspectAcceptable"];

                    if (isDraft) ncrReInspect.NcrReInspStatusFlag = true;

                    _context.Add(ncrReInspect);
                    await _context.SaveChangesAsync();

                    await AddReInspectPictures(ncrReInspect, Photos);

                    var ncrToUpdate = await _context.Ncrs
                        .AsNoTracking()
                        .Include(n => n.NcrQa.Supplier)
                        .Include(n => n.NcrQa.Item)
                        .Include(n => n.NcrQa.Defect)
                        .FirstOrDefaultAsync(n => n.NcrId == ncrReInspect.NcrId);

                    if (isDraft)
                    {
                        ncrToUpdate.NcrPhase = NcrPhase.ReInspection;
                        ncrToUpdate.NcrStatus = true;
                    }
                    else
                    {
                        ncrToUpdate.NcrPhase = NcrPhase.Closed;
                        ncrToUpdate.NcrStatus = false;
                    }
                    
                    ncrToUpdate.NcrLastUpdated = DateTime.Today;
                    _context.Ncrs.Update(ncrToUpdate);
                    await _context.SaveChangesAsync();

                    int ncrReInspectId = ncrReInspect.NcrReInspectId;

                    if(isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncrToUpdate.NcrNumber + " was saved as draft successfully!";
                    }
                    else
                    {
                        if (isAcceptable == "false")
                        {
                            string newNcrNumber = GetNcrNumber();

                            ncrReInspect.NcrReInspectNewNcrNumber = newNcrNumber;

                            Ncr newNcr = new Ncr
                            {
                                NcrNumber = newNcrNumber,
                                NcrStatus = true,
                                NcrLastUpdated = DateTime.Now,
                                NcrPhase = NcrPhase.QualityInspector
                            };

                            _context.Ncrs.Add(newNcr);
                            await _context.SaveChangesAsync();

                            var ncrNewId = await _context.Ncrs
                                .AsNoTracking()
                                .FirstOrDefaultAsync(n => n.NcrNumber == newNcrNumber);

                            NcrQa newNcrQa = new NcrQa
                            {
                                NcrId = ncrNewId.NcrId,
                                NcrQaOrderNumber = ncrToUpdate.NcrQa.NcrQaOrderNumber,
                                NcrQaItemMarNonConforming = ncrToUpdate.NcrQa.NcrQaItemMarNonConforming,
                                NcrQaProcessApplicable = ncrToUpdate.NcrQa.NcrQaProcessApplicable,
                                NcrQaSalesOrder = ncrToUpdate.NcrQa.NcrQaSalesOrder,
                                NcrQaQuanReceived = ncrToUpdate.NcrQa.NcrQaQuanReceived,
                                NcrQaQuanDefective = ncrToUpdate.NcrQa.NcrQaQuanDefective,
                                NcrQaDescriptionOfDefect = $"NCR was recreated from NCR #{ncrReInspect.NcrNumber} as it was not acceptable." /*ncrToUpdate.NcrQa.NcrQaDescriptionOfDefect*/,
                                SupplierId = ncrToUpdate.NcrQa.SupplierId,
                                ItemId = ncrToUpdate.NcrQa.ItemId,
                                DefectId = ncrToUpdate.NcrQa.DefectId,
                                NcrQacreationDate = DateTime.Now.Date,
                                NcrQaUserId = user.Id,
                                NcrQaEngDispositionRequired = ncrToUpdate.NcrQa.NcrQaEngDispositionRequired,
                                NcrQaStatusFlag = true
                            };

                            _context.NcrQas.Add(newNcrQa);
                            await _context.SaveChangesAsync();

                            TempData["SuccessMessage"] = $"This NCR was automatically created using NCR #{ncrReInspect.NcrNumber}s information";
                            return RedirectToAction("Edit", "NcrQa", new { id = ncrNewId.NcrId });
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "NCR " + ncrReInspect.NcrNumber + " closed successfully!";
                            //Delete cookies
                            //Response.Cookies.Delete("DraftNCRReInspect" + ncrReInspect.NcrNumber);                            
                        }           
                    }
                    return RedirectToAction("Details", new { id = ncrReInspect.NcrReInspectId });
                }
                //else
                //{
                //    //Debugging Approach: Not for production code.
                //    //This code will list validation errors at the top of the View.
                //    //Use it to diagnose when there seems to be a Validation Error
                //    //that is going unreported.  Remove this code when you are
                //    //finished debugging.
                //    var booBoos = ModelState.Where(x => x.Value.Errors.Count > 0)
                //        .Select(x => new { x.Key, x.Value.Errors });

                //    foreach (var booBoo in booBoos)
                //    {
                //        string key = booBoo.Key;
                //        foreach (var error in booBoo.Errors)
                //        {
                //            var errorMessage = error?.ErrorMessage;
                //            ModelState.AddModelError("", "For " + key + ": " + errorMessage);
                //        }
                //    }
                //}
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(ncrReInspect);
        }

        // GET: NcrReInspect/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NcrReInspects == null)
            {
                return NotFound();
            }

            var ncrReInspect = await _context.NcrReInspects
                .Include(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(n=>n.NcrReInspectId == id);

            if (ncrReInspect == null)
            {
                return NotFound();
            }
            ViewData["NcrId"] = new SelectList(_context.Ncrs, "NcrId", "NcrNumber", ncrReInspect.NcrId);
            return View(ncrReInspect);
        }

        // POST: NcrReInspect/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<IFormFile> Photos, IFormCollection form, bool isDraft = false)
        {           
            var ncrReInspectToUpdate = await _context.NcrReInspects
                .Include(r => r.Ncr)
                .Include(r => r.NcrReInspectPhotos)
                .FirstOrDefaultAsync(r => r.NcrReInspectId == id);

            ncrReInspectToUpdate.NcrReInspStatusFlag = isDraft;

            if (ncrReInspectToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<NcrReInspect>(ncrReInspectToUpdate, "",
                r => r.NcrReInspectAcceptable, r => r.NcrReInspectNewNcrNumber, r => r.NcrReInspectUserId,
                r => r.NcrId, r => r.NcrReInspectDefectVideo, r => r.NcrReInspectPhotos, r => r.NcrReInspectCreationDate, r => r.NcrReInspectNotes, r => r.NcrReInspStatusFlag))
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        ncrReInspectToUpdate.NcrReInspectUserId = user.Id;
                    }

                    await AddReInspectPictures(ncrReInspectToUpdate, Photos);

                    var ncrToUpdate = await _context.Ncrs
                        .Include(n=>n.NcrQa)
                        .FirstOrDefaultAsync(n => n.NcrId == ncrReInspectToUpdate.NcrId);
                    //var ncrToUpdate = await _context.Ncrs
                    //    .AsNoTracking()
                    //    .Include(n => n.NcrQa.Supplier)
                    //    .Include(n => n.NcrQa.Item)
                    //    .Include(n => n.NcrQa.Defect)
                    //    .FirstOrDefaultAsync(n => n.NcrId == ncrReInspectToUpdate.NcrId);

                    bool isAcceptable = form["NcrReInspectAcceptable"] == "true";

                    if (ncrToUpdate != null)
                    {
                        if(isDraft)
                        {
                            ncrToUpdate.NcrStatus = true;
                            ncrToUpdate.NcrPhase = NcrPhase.ReInspection;
                        }
                        else
                        {
                            ncrToUpdate.NcrStatus = false;
                            ncrToUpdate.NcrPhase = NcrPhase.Closed;
                        }                        

                        _context.Entry(ncrToUpdate).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    if (isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncrReInspectToUpdate.NcrNumber + " was edited as draft successfully!";
                    }
                    else
                    {
                        

                        if (ncrReInspectToUpdate.NcrReInspectAcceptable == false)
                        {
                            string newNcrNumber = GetNcrNumber();

                            ncrReInspectToUpdate.NcrReInspectNewNcrNumber = newNcrNumber;

                            Ncr newNcr = new Ncr
                            {
                                NcrNumber = newNcrNumber,
                                NcrStatus = true,
                                NcrLastUpdated = DateTime.Now,
                                NcrPhase = NcrPhase.Engineer
                            };

                            _context.Ncrs.Add(newNcr);
                            await _context.SaveChangesAsync();

                            var ncrNewId = await _context.Ncrs
                                .AsNoTracking()
                                .FirstOrDefaultAsync(n => n.NcrNumber == newNcrNumber);

                            NcrQa newNcrQa = new NcrQa
                            {
                                NcrId = ncrNewId.NcrId,
                                NcrQaOrderNumber = ncrToUpdate.NcrQa.NcrQaOrderNumber,
                                NcrQaItemMarNonConforming = ncrToUpdate.NcrQa.NcrQaItemMarNonConforming,
                                NcrQaProcessApplicable = ncrToUpdate.NcrQa.NcrQaProcessApplicable,
                                NcrQaSalesOrder = ncrToUpdate.NcrQa.NcrQaSalesOrder,
                                NcrQaQuanReceived = ncrToUpdate.NcrQa.NcrQaQuanReceived,
                                NcrQaQuanDefective = ncrToUpdate.NcrQa.NcrQaQuanDefective,
                                NcrQaDescriptionOfDefect = $"NCR was recreated from NCR #{ncrReInspectToUpdate.NcrNumber} as it was not acceptable." /*ncrToUpdate.NcrQa.NcrQaDescriptionOfDefect*/,
                                SupplierId = ncrToUpdate.NcrQa.SupplierId,
                                ItemId = ncrToUpdate.NcrQa.ItemId,
                                DefectId = ncrToUpdate.NcrQa.DefectId,
                                NcrQacreationDate = DateTime.Now.Date,
                                NcrQaUserId = user.Id,
                                NcrQaEngDispositionRequired = ncrToUpdate.NcrQa.NcrQaEngDispositionRequired
                            };

                            _context.NcrQas.Add(newNcrQa);
                            await _context.SaveChangesAsync();

                            TempData["SuccessMessage"] = $"This NCR was automatically created using NCR #{ncrReInspectToUpdate.NcrNumber}s information";
                            return RedirectToAction("Edit", "NcrQa", new { id = ncrNewId.NcrId });
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "NCR " + ncrReInspectToUpdate.NcrNumber + " closed successfully!";
                            //Delete cookies
                            //Response.Cookies.Delete("DraftNCRReInspect" + ncrReInspect.NcrNumber);                            
                        }
                    }                  
                                        
                    int updateNcrReInspect = ncrReInspectToUpdate.NcrReInspectId;
                    //Response.Cookies.Delete("DraftNCRReInspect" + ncrReInspectToUpdate.NcrNumber);

                    return RedirectToAction("Details", new { id = updateNcrReInspect });

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NcrReInspectExists(ncrReInspectToUpdate.NcrReInspectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            //else
            //{
            //    //Debugging Approach: Not for production code.
            //    //This code will list validation errors at the top of the View.
            //    //Use it to diagnose when there seems to be a Validation Error
            //    //that is going unreported.  Remove this code when you are
            //    //finished debugging.
            //    var booBoos = ModelState.Where(x => x.Value.Errors.Count > 0)
            //        .Select(x => new { x.Key, x.Value.Errors });

            //    foreach (var booBoo in booBoos)
            //    {
            //        string key = booBoo.Key;
            //        foreach (var error in booBoo.Errors)
            //        {
            //            var errorMessage = error?.ErrorMessage;
            //            ModelState.AddModelError("", "For " + key + ": " + errorMessage);
            //        }
            //    }
            //}
            ViewData["NcrId"] = new SelectList(_context.Ncrs, "NcrId", "NcrNumber", ncrReInspectToUpdate.NcrId);
            return View(ncrReInspectToUpdate);
        }

        private async Task AddReInspectPictures(NcrReInspect ncrReInspect, List<IFormFile> pictures)
        {
            var thumbnailUrls = new List<string>();

            if (pictures != null && pictures.Any())
            {
                // If the NcrReInspect already has some photos, keep them
                if (ncrReInspect.NcrReInspectPhotos == null)
                {
                    ncrReInspect.NcrReInspectPhotos = new List<NcrReInspectPhoto>();
                }

                foreach (var picture in pictures)
                {
                    string mimeType = picture.ContentType;
                    long fileLength = picture.Length;

                    if (!(mimeType == "" || fileLength == 0))
                    {
                        if (mimeType.Contains("image"))
                        {
                            using var memoryStream = new MemoryStream();
                            await picture.CopyToAsync(memoryStream);
                            var pictureArray = memoryStream.ToArray();

                            ncrReInspect.NcrReInspectPhotos.Add(new NcrReInspectPhoto
                            {
                                NcrReInspectPhotoContent = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                NcrReInspectPhotoMimeType = "image/webp",
                                FileName = picture.FileName
                            });
                        }
                    }
                }
            }
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.NcrReInspectPhotos
                .Include(d => d.ReInspectFileContent)
                .Where(f => f.NcrReInspectPhotoId == id)
                .FirstOrDefaultAsync();
            return File(theFile.NcrReInspectPhotoContent, theFile.NcrReInspectPhotoMimeType, theFile.FileName);
        }

        public JsonResult GetNcrs()
        {
            //List<Ncr> pendings = _context.Ncrs
            //    .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
            //    .Where(n => n.NcrPhase == NcrPhase.ReInspection)
            //    .ToList();
            List<Ncr> pendings = _context.Ncrs
                .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Where(n => n.NcrPhase == NcrPhase.ReInspection && n.NcrReInspect.NcrReInspStatusFlag != true)
                .ToList();

            // Extract relevant data for the client-side
            var ncrs = pendings.Select(ncr => new
            {
                NcrId = ncr.NcrId,
                NcrNumber = ncr.NcrNumber,
                SupplierName = ncr.NcrQa.Supplier.SupplierName
            }).ToList();

            return Json(ncrs);
        }

        public JsonResult GetPendingCount()
        {
            //int pendingCount = _context.Ncrs
            //    .Where(n => n.NcrPhase == NcrPhase.ReInspection)
            //    .Count();
            int pendingCount = _context.Ncrs
                .Where(n => n.NcrPhase == NcrPhase.ReInspection && n.NcrReInspect.NcrReInspStatusFlag != true)
                .Count();

            return Json(pendingCount);
        }

        //public async Task<FileContentResult> Download(int id)
        //{
        //    var theFile = await _context.NcrReInspectPhotos
        //        .Include(d => d.FileContent)
        //        .Where(f => f.NcrReInspectPhotoId == id)
        //        .FirstOrDefaultAsync();
        //    return File(theFile.NcrReInspectPhotoContent, theFile.NcrReInspectPhotoMimeType, theFile.FileName);
        //}

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.NcrReInspectPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.NcrReInspectPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Photo deleted successfully." });
            }
            return Json(new { success = false, message = "Photo not found." });
        }

        public string GetNcrNumber()
        {
            string lastNcrNumber = _context.Ncrs
                .OrderByDescending(n => n.NcrNumber)
                .Select(n => n.NcrNumber)
                .FirstOrDefault();

            if (lastNcrNumber != null)
            {
                string lastYear = lastNcrNumber.Substring(0, 4);
                string lastConsecutiveNumber = lastNcrNumber.Substring(5);

                if (lastYear == DateTime.Today.Year.ToString())
                {
                    int nextNumber = int.Parse(lastConsecutiveNumber) + 1;
                    string nextNumberString = nextNumber.ToString("000");

                    //Ncr Format
                    return $"{lastYear}-{nextNumberString}";
                }
            }

            string currentYear = DateTime.Today.Year.ToString();
            int nextConsecutiveNumber = 1;
            string nextConsecutiveNumberString = nextConsecutiveNumber.ToString("000");

            //Ncr Format
            return $"{currentYear}-{nextConsecutiveNumberString}";
        }
        private bool NcrReInspectExists(int id)
        {
            return _context.NcrReInspects.Any(e => e.NcrReInspectId == id);
        }

        private int FindNcrId(string ncrNumber)
        {
            var ncr = _context.Ncrs.FirstOrDefault(n => n.NcrNumber == ncrNumber);
            //if NCR id is null pass 0, otherwise pass the id
            return ncr?.NcrId ?? 0;
        }

        public async Task<IActionResult> ExportToExcelAsync()
        {
            var ncrReinspect = _context.NcrReInspects
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Where(n => n.Ncr.NcrPhase != NcrPhase.Archive)
                .AsNoTracking()
                .ToList(); // Load data into memory

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                var worksheet = package.Workbook.Worksheets.Add("NCR Data");

                // Define the columns in Excel
                worksheet.Cells[2, 1].Value = "NCR Number";
                worksheet.Cells[2, 2].Value = "Supplier";
                worksheet.Cells[2, 3].Value = "Acceptable";
                worksheet.Cells[2, 4].Value = "Inspected By";
                worksheet.Cells[2, 5].Value = "Phase";
                worksheet.Cells[2, 6].Value = "Created";
                worksheet.Cells[2, 7].Value = "Last Update";

                // Apply center alignment to the header cells
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the header with col span over all columns
                worksheet.Cells[1, 1, 1, 7].Merge = true;  // Merge 6 columns starting from A1
                worksheet.Cells[1, 1].Value = "NCR Reinspection Log";

                // Apply styling to header row (including the merged header cell)
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }



                // Fill data into Excel
                int row = 3;
                foreach (var item in ncrReinspect)
                {
                    //Get the inspector name from _userManager using the NcrReInspectUserId.
                    var inspector = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == item.NcrReInspectUserId);
                    string inspectorName = inspector != null ? inspector.FirstName + ' ' + inspector.LastName  : "Unknown";

                    worksheet.Cells[row, 1].Value = item.Ncr.NcrNumber;
                    worksheet.Cells[row, 2].Value = item.Ncr.NcrQa.Supplier.SupplierName;
                    worksheet.Cells[row, 3].Value = item.NcrReInspectAcceptable ? "Yes" : "No";
                    worksheet.Cells[row, 4].Value = inspectorName;
                    worksheet.Cells[row, 5].Value = item.Ncr.NcrPhase.ToString();
                    worksheet.Cells[row, 6].Value = item.Ncr.NcrQa.NcrQacreationDate.ToString();
                    worksheet.Cells[row, 7].Value = item.Ncr.NcrLastUpdated.ToString();

                    row++;
                }

                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set content type and filename for the Excel file
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "NCR_Reinspection_Data.xlsx";

                // Return the Excel file as a file download
                return File(content, contentType, fileName);
            }
        }
    }
}
