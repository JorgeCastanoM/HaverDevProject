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
using HaverDevProject.CustomControllers;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Engineer, Admin")]
    [ActiveUserOnly]
    public class NcrEngController : ElephantController
    {
        //for sending email
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly HaverNiagaraContext _context;

        public NcrEngController(HaverNiagaraContext context, IMyEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }


        // GET: NcrEng
        public async Task<IActionResult> Index(string SearchCode, int? EngDispositionTypeId, DateTime StartDate, DateTime EndDate,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "desc", string sortField = "Created", string filter = "Active")
        {

            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //Set the date range filer based on the values in the database
            if (EndDate == DateTime.MinValue)
            {
                StartDate = _context.Ncrs
                .Min(f => f.NcrQa.NcrQacreationDate.Date);

                EndDate = _context.Ncrs
                .Max(f => f.NcrQa.NcrQacreationDate.Date);

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

            //List of sort options.
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "Disposition", "Phase", "Last Updated" };


            PopulateDropDownLists();
            GetNcrs();

            var ncrEng = _context.NcrEngs
                .Include(n => n.EngDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.Drawing)
                .Where(n => n.Ncr.NcrPhase != NcrPhase.Archive)
                .AsNoTracking();

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
                    ncrEng = ncrEng.Where(n => n.Ncr.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";

                }
                else //(filter == "Closed")
                {
                    ncrEng = ncrEng.Where(n => n.Ncr.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";

                }
            }

            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncrEng = ncrEng.Where(s => s.Ncr.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (EngDispositionTypeId.HasValue)
            {
                ncrEng = ncrEng.Where(n => n.EngDispositionType.EngDispositionTypeId == EngDispositionTypeId);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncrEng = ncrEng.Where(n => n.Ncr.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncrEng = ncrEng.Where(n => n.Ncr.NcrQa.NcrQacreationDate >= StartDate &&
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

            if (sortField == "NCR #")
            {
                if (sortDirection == "asc")
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Disposition")
            {
                if (sortDirection == "asc")
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrEng.EngDispositionType.EngDispositionTypeName);
                    ViewData["filterApplied:Disposition"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrEng.EngDispositionType.EngDispositionTypeName);
                    ViewData["filterApplied:Disposition"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            else if (sortField == "Created")
            {
                if (sortDirection == "asc") //asc by default
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrPhase);

                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrPhase);

                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "Status")
            {
                if (sortDirection == "asc")
                {
                    ncrEng = ncrEng
                        .OrderBy(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrEng = ncrEng
                        .OrderByDescending(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NcrEng>.CreateAsync(ncrEng.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: NcrEng/Details/5
        public async Task<IActionResult> Details(int? id, string referrer)
        {
            if (id == null || _context.NcrEngs == null)
            {
                return NotFound();
            }

            var ncrEng = await _context.NcrEngs
                .Include(n => n.EngDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Defect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.ItemDefectPhotos)
                .Include(n => n.Drawing)
                .Include(n => n.EngDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.OpDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.FollowUpType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrOperation).ThenInclude(n => n.OpDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement).ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect).ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(m => m.NcrEngId == id);

            if (ncrEng == null)
            {
                return NotFound();
            }

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = true;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.NCRSectionId = id;

            return View(ncrEng);
        }

        // GET: NcrEng/Create
        public async Task<IActionResult> Create(string ncrNumber)
        {
            NcrEngDTO ncrEngDTO;

            int ncrId = _context.Ncrs.Where(n => n.NcrNumber == ncrNumber).Select(n => n.NcrId).FirstOrDefault();

            // Check if there are info in cookies
            if (Request.Cookies.TryGetValue("DraftNCREng" + ncrNumber, out string draftJson))
            {
                // Convert the data from json file
                ncrEngDTO = JsonConvert.DeserializeObject<NcrEngDTO>(draftJson);
                TempData["SuccessMessage"] = "Draft successfully retrieved";
            }
            else
            {

                ncrEngDTO = new NcrEngDTO
                {
                    NcrNumber = ncrNumber, // Set the NcrNumber from the parameter
                    DrawingRevDate = DateTime.Now,
                    NcrEngCompleteDate = DateTime.Now,
                    DrawingOriginalRevNumber = 1,
                    DrawingRequireUpdating = false,
                    NcrEngCustomerNotification = false
                };
            }
            var readOnlyDetails = await _context.Ncrs
                .Include(n => n.NcrQa)
                        .ThenInclude(item => item.Supplier)
                .Include(n => n.NcrQa)
                        .ThenInclude(defect => defect.Item)
                .Include(n => n.NcrQa)
                        .ThenInclude(defect => defect.Defect)
                .Include(n => n.NcrQa)
                    .ThenInclude(qa => qa.ItemDefectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == ncrId);

            ViewBag.IsNCRQaView = true;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            PopulateDropDownLists();
            return View(ncrEngDTO);
        }

        // POST: NcrEng/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NcrEngDTO ncrEngDTO, List<IFormFile> Photos, bool isDraft = false)
        {
            try
            {                
                //if (isDraft)
                //{
                //    // convert the object to json format
                //    var json = JsonConvert.SerializeObject(ncrEngDTO);

                //    // Save the object in a cookie with name "DraftData"
                //    Response.Cookies.Append("DraftNCREng" + ncrEngDTO.NcrNumber, json, new CookieOptions
                //    {
                //        // Define time for cookies
                //        Expires = DateTime.Now.AddMinutes(2880) // Cookied will expire in 48 hrs
                //    });

                //    return Ok(new { success = true, message = "Draft saved successfully.\nNote: This draft will be available for the next 48 hours." });
                //}

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    // Find the Ncr entity based on the NcrNumber in the DTO
                    int ncrIdObt = _context.Ncrs
                        .Where(n => n.NcrNumber == ncrEngDTO.NcrNumber)
                        .Select(n => n.NcrId)
                        .FirstOrDefault();

                    //Checking if NcrOperations exist...
                    var ncrOperations = await _context.NcrOperations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == ncrIdObt);
                    bool ncrOperationExist = ncrOperations != null;

                    //Checking if NcrProcurement exist...
                    var ncrProcurement = await _context.NcrProcurements
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == ncrIdObt);
                    bool ncrProcurementExist = ncrProcurement != null;

                    //PopulateDropDownLists();
                    await AddPictures(ncrEngDTO, Photos);

                    if (isDraft) ncrEngDTO.NcrEngStatusFlag = true;
                    

                    NcrEng ncrEng = new NcrEng
                    {
                        NcrId = ncrIdObt, // Assign the NcrId from the found Ncr entity
                        NcrEngCustomerNotification = ncrEngDTO.NcrEngCustomerNotification,
                        NcrEngDispositionDescription = ncrEngDTO.NcrEngDispositionDescription,
                        NcrEngStatusFlag = ncrEngDTO.NcrEngStatusFlag,
                        NcrEngUserId = user.Id,
                        NcrEngCompleteDate = DateTime.Now,
                        NcrEngCreationDate = ncrEngDTO.NcrEngCreationDate,
                        EngDispositionTypeId = ncrEngDTO.EngDispositionTypeId,
                        DrawingId = ncrEngDTO.DrawingId,
                        DrawingRequireUpdating = ncrEngDTO.DrawingRequireUpdating,
                        DrawingOriginalRevNumber = ncrEngDTO.DrawingOriginalRevNumber,
                        DrawingUpdatedRevNumber = ncrEngDTO.DrawingUpdatedRevNumber,
                        DrawingRevDate = DateTime.Now,
                        DrawingUserId = ncrEngDTO.DrawingUserId,
                        EngDefectPhotos = ncrEngDTO.EngDefectPhotos,
                        NcrEngDefectVideo = ncrEngDTO.NcrEngDefectVideo

                    };
                    _context.NcrEngs.Add(ncrEng);
                    await _context.SaveChangesAsync();

                    //update ncr 
                    var ncr = await _context.Ncrs.AsNoTracking().FirstOrDefaultAsync(n => n.NcrId == ncrIdObt);

                    if (isDraft)
                    {
                        ncr.NcrPhase = NcrPhase.Engineer;
                    }
                    else
                    {
                        if (ncrOperationExist == false)
                        {
                            ncr.NcrPhase = NcrPhase.Operations;
                        }
                        else if (ncrOperationExist == true && ncrProcurementExist == false)
                        {
                            ncr.NcrPhase = NcrPhase.Procurement;
                        }
                        else
                        {
                            ncr.NcrPhase = NcrPhase.ReInspection;
                        }
                    }                    

                    //ncr.NcrPhase = NcrPhase.Operations;
                    ncr.NcrLastUpdated = DateTime.Now;
                    _context.Ncrs.Update(ncr);
                    await _context.SaveChangesAsync();
                   
                    //Delete cookies
                    //Response.Cookies.Delete("DraftNCREng" + ncr.NcrNumber);
                    int ncrEngId = ncrEng.NcrEngId;

                    if(isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was saved as draft successfully!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " saved successfully!";
                        //include supplier name in the email
                        var ncrE = await _context.NcrEngs
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrEngId == ncrEngId);

                        // Send notification email to Procurement
                        var subject = "New NCR Created " + ncr.NcrNumber + "  from Engineer";
                        var emailContent = "A new NCR has been created:<br><br>Ncr #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationCreate(ncrEngId, subject, emailContent);
                    }                    

                    return RedirectToAction("Details", new { id = ncrEngId, referrer = "Create" });
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists();

            return View(ncrEngDTO);
        }

        // GET: NcrEng/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var ncrEng = await _context.NcrEngs
                        .Include(ne => ne.Ncr)
                          .Include(ne => ne.EngDispositionType)
                          .Include(ne => ne.Drawing)
                        .Include(n => n.EngDefectPhotos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(ne => ne.NcrEngId == id);

            if (ncrEng == null)
            {
                return NotFound();
            }

            var ncrEngDTO = new NcrEngDTO
            {
                NcrEngId = ncrEng.NcrEngId,
                NcrNumber = ncrEng.Ncr.NcrNumber,
                NcrEngCustomerNotification = ncrEng.NcrEngCustomerNotification,
                NcrEngDispositionDescription = ncrEng.NcrEngDispositionDescription,
                NcrEngCreationDate = ncrEng.NcrEngCreationDate,
                NcrEngCompleteDate = ncrEng.NcrEngCompleteDate,
                NcrEngStatusFlag = ncrEng.NcrEngStatusFlag,
                NcrEngUserId = user.Id,
                EngDispositionTypeId = ncrEng.EngDispositionTypeId,
                NcrId = ncrEng.NcrId,
                DrawingId = ncrEng.DrawingId,
                DrawingRequireUpdating = ncrEng.DrawingRequireUpdating,
                DrawingOriginalRevNumber = ncrEng.DrawingOriginalRevNumber,
                DrawingUpdatedRevNumber = ncrEng.DrawingUpdatedRevNumber,
                DrawingRevDate = DateTime.Now,
                DrawingUserId = ncrEng.DrawingUserId,
                EngDefectPhotos = ncrEng.EngDefectPhotos,
                NcrEngDefectVideo = ncrEng.NcrEngDefectVideo,
                NcrPhase = ncrEng.Ncr.NcrPhase
            };

            var readOnlyDetails = await _context.Ncrs
                .Include(n => n.NcrQa)
                        .ThenInclude(item => item.Supplier)
                .Include(n => n.NcrQa)
                            .ThenInclude(defect => defect.Item)
                .Include(n => n.NcrQa)
                            .ThenInclude(defect => defect.Defect)
                .Include(n => n.NcrQa)
                    .ThenInclude(qa => qa.ItemDefectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == id);

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            ViewData["EngDispositionTypeId"] = new SelectList(_context.EngDispositionTypes, "EngDispositionTypeId", "EngDispositionTypeName", ncrEng.EngDispositionTypeId);
            return View(ncrEngDTO);
        }

        // POST: NcrEng/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NcrEngDTO ncrEngDTO, List<IFormFile> Photos, bool isDraft = false)
        {
            if (isDraft) ncrEngDTO.NcrEngStatusFlag = true;
            
            if (ModelState.IsValid)
            {
                
                await AddPictures(ncrEngDTO, Photos);
                try
                {
                    var ncrEng = await _context.NcrEngs
                        .Include(ne => ne.Drawing)
                        .FirstOrDefaultAsync(ne => ne.NcrEngId == id);
                    
                    ncrEng.NcrEngCustomerNotification = ncrEngDTO.NcrEngCustomerNotification;
                    ncrEng.NcrEngDispositionDescription = ncrEngDTO.NcrEngDispositionDescription;
                    ncrEng.NcrEngCreationDate = ncrEngDTO.NcrEngCreationDate;
                    ncrEng.NcrEngCompleteDate = ncrEngDTO.NcrEngCompleteDate;
                    ncrEng.NcrEngStatusFlag = ncrEngDTO.NcrEngStatusFlag;
                    ncrEng.NcrEngUserId = ncrEngDTO.NcrEngUserId;
                    ncrEng.EngDispositionTypeId = ncrEngDTO.EngDispositionTypeId;
                    ncrEng.DrawingId = ncrEngDTO.DrawingId;
                    ncrEng.DrawingRequireUpdating = ncrEngDTO.DrawingRequireUpdating;
                    ncrEng.DrawingOriginalRevNumber = ncrEngDTO.DrawingOriginalRevNumber;
                    ncrEng.DrawingUpdatedRevNumber = ncrEngDTO.DrawingUpdatedRevNumber;
                    ncrEng.DrawingRevDate = DateTime.Now;
                    ncrEng.DrawingUserId = ncrEngDTO.DrawingUserId;
                    ncrEng.EngDefectPhotos = ncrEngDTO.EngDefectPhotos;
                    ncrEng.NcrEngDefectVideo = ncrEngDTO.NcrEngDefectVideo;

                    _context.Update(ncrEng);
                    await _context.SaveChangesAsync();

                    var ncr = await _context.Ncrs.AsNoTracking().FirstOrDefaultAsync(n => n.NcrId == ncrEng.NcrId);
                    ncr.NcrLastUpdated = DateTime.Now;

                    if (!isDraft) ncr.NcrPhase = NcrPhase.Operations;

                    _context.Update(ncr);
                    await _context.SaveChangesAsync();

                    int ncrEngId = ncrEng.NcrEngId;

                    if (isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was edited as draft successfully!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " edited successfully!";

                        //include supplier name in the email
                        var ncrE = await _context.NcrEngs
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrEngId == ncrEngId);

                        // Send notification email to Procurement
                        var subject = "NCR Edited " + ncr.NcrNumber + "  from Engineer";
                        var emailContent = "A NCR has been edited :<br><br>Ncr #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationEdit(ncrEngId, subject, emailContent);
                    }                    

                    return RedirectToAction("Details", new { id = ncrEngId, referrer = "Edit" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NcrEngExists(ncrEngDTO.NcrEngId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }
            return View(ncrEngDTO);
        }


        public JsonResult GetNcrs()
        {
            //List<Ncr> pendings = _context.Ncrs
            //    .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
            //    .Where(n => n.NcrPhase == NcrPhase.Engineer)
            //    .ToList();

            List<Ncr> pendings = _context.Ncrs
               .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)               
               .Where(n => n.NcrPhase == NcrPhase.Engineer && n.NcrEng.NcrEngStatusFlag != true)
               .ToList();

            // Extract relevant data for the client-side
            var ncrs = pendings.Select(ncr => new
            {
                NcrId = ncr.NcrId,
                NcrNumber = ncr.NcrNumber,
                SupplierName = ncr.NcrQa.Supplier.SupplierName,
                Created = ncr.NcrQa.NcrQacreationDate
            }).ToList();

            return Json(ncrs);
        }

        public JsonResult GetPendingCount()
        {
            int pendingCount = _context.Ncrs
                .Count(n => n.NcrPhase == NcrPhase.Engineer && n.NcrEng.NcrEngStatusFlag != true);

            //int pendingCount = _context.NcrEngs
            //    .Include(n=>n.Ncr)
            //    .Where(n=>n.NcrEngStatusFlag == false && n.Ncr.NcrPhase == NcrPhase.Engineer)
            //    .Count();

            return Json(pendingCount);
        }

        private async Task AddPictures(NcrEngDTO ncrEngDTO, List<IFormFile> pictures)
        {
            if (pictures != null && pictures.Any())
            {
                ncrEngDTO.EngDefectPhotos = new List<EngDefectPhoto>();

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

                            ncrEngDTO.EngDefectPhotos.Add(new EngDefectPhoto
                            {
                                EngDefectPhotoContent = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                EngDefectPhotoMimeType = "image/webp",
                                FileName = picture.FileName
                            });
                        }
                    }
                }
            }
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.EngDefectPhotos
                .Include(d => d.EngFileContent)
                .Where(f => f.EngDefectPhotoId == id)
                .FirstOrDefaultAsync();
            return File(theFile.EngDefectPhotoContent, theFile.EngDefectPhotoMimeType, theFile.FileName);
        }


        private bool NcrEngExists(int id)
        {
            return _context.NcrEngs.Any(e => e.NcrEngId == id);
        }

        private SelectList EngDispositionTypeSelectList(int? selectedId)
        {
            return new SelectList(_context.EngDispositionTypes
                .OrderBy(s => s.EngDispositionTypeName), "EngDispositionTypeId", "EngDispositionTypeName", selectedId);
        }
        private void PopulateDropDownLists(NcrEng ncrEng = null)
        {
            if ((ncrEng?.EngDispositionTypeId).HasValue)
            {
                if (ncrEng.EngDispositionType == null)
                {
                    ncrEng.EngDispositionType = _context.EngDispositionTypes.Find(ncrEng.EngDispositionTypeId);
                }
                ViewData["EngDispositionTypeId"] = EngDispositionTypeSelectList(ncrEng?.EngDispositionTypeId);
            }
            else
            {
                ViewData["EngDispositionTypeId"] = EngDispositionTypeSelectList(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.EngDefectPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.EngDefectPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Photo deleted successfully." });
            }
            return Json(new { success = false, message = "Photo not found." });
        }

        //// CREATE/POST: Email Noitications
        public async Task<IActionResult> NotificationCreate(int? id, string Subject, string emailContent)
        {

            if (id == null)
            {
                return NotFound();
            }

            NcrOperation o = await _context.NcrOperations.FindAsync(id);

            ViewData["id"] = id;

            try
            {
                var procurementUsers = await _userManager.GetUsersInRoleAsync("Operations");
                var emailAddresses = procurementUsers.Select(u => new EmailAddress
                {
                    Name = u.UserName,
                    Address = u.Email
                }).ToList();

                if (emailAddresses.Any())
                {
                    string link = "https://haverv2team3.azurewebsites.net";
                    string logo = "https://haverniagara.com/wp-content/themes/haver/images/logo-haver.png";
                    var msg = new EmailMessage()
                    {
                        ToAddresses = emailAddresses,
                        Subject = Subject,
                        Content = "<p>" + emailContent + "<br><br></p><p>Please access the <strong>Haver NCR APP</strong> to review.</p><br>Link: <a href=\"" + link + "\">" + "Go to NCR" + "</a><br>" + "<br><img src=\"" + logo + "\">" + "<p>This is an automated email. Please do not reply.</p>",
                    };
                    await _emailSender.SendToManyAsync(msg);
                }
                else
                {
                    ViewData["Message"] = "Message NOT sent! No Procurement users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to Operations users. Error: {errMsg}";
            }

            return View();
        }

        //// EDIT/POST: Email  Notification
        public async Task<IActionResult> NotificationEdit(int? id, string Subject, string emailContent)
        {

            if (id == null)
            {
                return NotFound();
            }

            NcrOperation o = await _context.NcrOperations.FindAsync(id);

            ViewData["id"] = id;

            try
            {
                var qualityUsers = await _userManager.GetUsersInRoleAsync("Quality");
                var operationsUsers = await _userManager.GetUsersInRoleAsync("Operations");
                var procurementUsers = await _userManager.GetUsersInRoleAsync("Procurement");

                var allUsers = procurementUsers
                    .Concat(qualityUsers)
                    .Concat(operationsUsers)
                    .Distinct();


                var emailAddresses = allUsers.Select(u => new EmailAddress
                {
                    Name = u.UserName,
                    Address = u.Email
                }).ToList();

                if (emailAddresses.Any())
                {
                    string link = "https://haverv2team3.azurewebsites.net";
                    string logo = "https://haverniagara.com/wp-content/themes/haver/images/logo-haver.png";
                    var msg = new EmailMessage()
                    {
                        ToAddresses = emailAddresses,
                        Subject = Subject,
                        Content = "<p>" + emailContent + "<br><br></p><p>Please access the <strong>Haver NCR APP</strong> to review.</p><br>Link: <a href=\"" + link + "\">" + "Go to NCR" + "</a><br>" + "<br><img src=\"" + logo + "\">" + "<p>This is an automated email. Please do not reply.</p>",
                    };
                    await _emailSender.SendToManyAsync(msg);
                }
                else
                {
                    ViewData["Message"] = "Message NOT sent! No Procurement users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to users. Error: {errMsg}";
            }

            return View();
        }

        //24 Hour Notification
        public async Task CheckAndSendEmailNotifications()
        {
            // Get pending NCRs from Engineering that are older than 24 hours
            var pendingNCRs = await _context.NcrQas
                .Where(n => n.Ncr.NcrPhase == NcrPhase.QualityInspector && n.NcrQacreationDate <= DateTime.Now.AddHours(-24))
                .ToListAsync();

            foreach (var ncr in pendingNCRs)
            {
                // Check if an NCR has not been created in Operations
                var ncrInOps = await _context.NcrEngs.FirstOrDefaultAsync(op => op.NcrId == ncr.NcrId);
                if (ncrInOps == null)
                {
                    // Check if it's exactly 24 hours since the NCR was created in Engineering
                    if (DateTime.Now.Subtract(ncr.NcrQacreationDate).TotalHours == 24)
                    {
                        // Send notification email to Operations or Admin role
                        var subject = "NCR Pending in Engineer";
                        var emailContent = "An NCR from Quality is pending in Operations and has not been created yet.";
                        await NotificationCreate(ncr.NcrId, subject, emailContent);
                    }
                }
            }
        }

        public IActionResult ExportToExcel()
        {
            var ncrOperation = _context.NcrEngs
                .Include(n => n.EngDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.Drawing)
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
                worksheet.Cells[2, 3].Value = "Disposition";
                worksheet.Cells[2, 4].Value = "Phase";
                worksheet.Cells[2, 5].Value = "Created";
                worksheet.Cells[2, 6].Value = "Last Update";

                // Apply center alignment to the header cells
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the header with col span over all columns
                worksheet.Cells[1, 1, 1, 6].Merge = true;  // Merge 6 columns starting from A1
                worksheet.Cells[1, 1].Value = "NCR Engineering Log";

                // Apply styling to header row (including the merged header cell)
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Fill data into Excel
                int row = 3;
                foreach (var item in ncrOperation)
                {
                    worksheet.Cells[row, 1].Value = item.Ncr.NcrNumber;
                    worksheet.Cells[row, 2].Value = item.Ncr.NcrQa.Supplier.SupplierName;
                    worksheet.Cells[row, 3].Value = item.Ncr.NcrEng.EngDispositionType.EngDispositionTypeName;
                    worksheet.Cells[row, 4].Value = item.Ncr.NcrPhase.ToString();
                    worksheet.Cells[row, 5].Value = item.Ncr.NcrQa.NcrQacreationDate.ToString();
                    worksheet.Cells[row, 6].Value = item.Ncr.NcrLastUpdated.ToString();

                    row++;
                }

                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set content type and filename for the Excel file
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "NCR_Engineering_Data.xlsx";

                // Return the Excel file as a file download
                return File(content, contentType, fileName);
            }
        }
    }
}
