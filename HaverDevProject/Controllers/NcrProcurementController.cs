using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.ViewModels;
using HaverDevProject.CustomControllers;
using HaverDevProject.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Procurement, Admin")]
    [ActiveUserOnly]
    public class NcrProcurementController : ElephantController
    {
        //for sending email
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly HaverNiagaraContext _context;

        public NcrProcurementController(HaverNiagaraContext context, IMyEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: NcrProcurement
        public async Task<IActionResult> Index(string SearchCode, string SearchSupplier, DateTime StartDate, DateTime EndDate,
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

            //Check the order of the dates and swap them if required
            if (EndDate < StartDate)
            {
                DateTime temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }

            //List of sort options.
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "SupplierReturn", "SAP", "Phase", "Last Updated" };

            ViewData["SupplierId"] = SupplierSelectList();

            var ncrProc = _context.NcrProcurements
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                //.Where(n => n.Ncr.NcrPhase == NcrPhase.ReInspection)
                .AsNoTracking();

            GetNcrs();

            //Filterig values            
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
                    ncrProc = ncrProc.Where(n => n.Ncr.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    ncrProc = ncrProc.Where(n => n.Ncr.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncrProc = ncrProc.Where(s => s.Ncr.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }            
            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                ncrProc = ncrProc.Where(n => n.Ncr.NcrQa.Supplier.SupplierName == SearchSupplier);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncrProc = ncrProc.Where(n => n.Ncr.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncrProc = ncrProc.Where(n => n.Ncr.NcrQa.NcrQacreationDate >= StartDate &&
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
                    ncrProc = ncrProc
                        .OrderBy(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "SupplierReturn")
            {
                if (sortDirection == "asc")
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.NcrProcSupplierReturnReq);
                    ViewData["filterApplied:SupplierReturn"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.NcrProcSupplierReturnReq);
                    ViewData["filterApplied:SupplierReturn"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "SAP")
            {
                if (sortDirection == "asc")
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.NcrProcSAPReturnCompleted);
                    ViewData["filterApplied:SAP"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.NcrProcSAPReturnCompleted);
                    ViewData["filterApplied:SAP"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "asc") //asc by default
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.Ncr.NcrPhase);

                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.Ncr.NcrPhase);

                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else /*if (sortField == "Last Updated")*/
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrProc = ncrProc
                        .OrderBy(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrProc = ncrProc
                        .OrderByDescending(p => p.Ncr.NcrLastUpdated);

                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NcrProcurement>.CreateAsync(ncrProc.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: NcrProcurement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NcrProcurements == null)
            {
                return NotFound();
            }

            var ncrProcurement = await _context.NcrProcurements
                .Include(n => n.Ncr)
                .Include(n => n.ProcDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
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
                .Include(n => n.ProcDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect).ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(m => m.NcrProcurementId == id);
            if (ncrProcurement == null)
            {
                return NotFound();
            }

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = true;
            ViewBag.IsNCRReInspView = false;

            ViewBag.NCRSectionId = id;

            return View(ncrProcurement);
        }

        // GET: NcrProcurement/Create
        public async Task<IActionResult> Create(string ncrNumber)
        {
            NcrProcurementDTO ncr;

            int ncrId = _context.Ncrs.Where(n => n.NcrNumber == ncrNumber).Select(n => n.NcrId).FirstOrDefault();

            // Check if there are info in cookies
            if (Request.Cookies.TryGetValue("DraftNCRProcurement" + ncrNumber, out string draftJson))
            {
                // Convert the data from json file
                ncr = JsonConvert.DeserializeObject<NcrProcurementDTO>(draftJson);
                TempData["SuccessMessage"] = "Draft successfully retrieved";
            }
            else
            {
                ncr = new NcrProcurementDTO
                {
                    NcrNumber = ncrNumber,
                    NcrProcCompleteDate = DateTime.Now,
                    NcrProcExpectedDate = DateTime.Now,
                    NcrProcSupplierReturnReq = true,
                    NcrStatus = true, //Active
                };
            }

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
            ViewBag.IsNCROpView = true;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            return View(ncr);
        }

        // POST: NcrProcurement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NcrProcurementDTO ncrProcDTO, List<IFormFile> photos, bool isDraft = false)
        {
            try
            {
                //if (isDraft)
                //{
                //    // convert the object to json format
                //    var json = JsonConvert.SerializeObject(ncrProcDTO);

                //    // Save the object in a cookie with name "DraftData"
                //    Response.Cookies.Append("DraftNCRProcurement" + ncrProcDTO.NcrNumber, json, new CookieOptions
                //    {
                //        // Define time for cookies
                //        Expires = DateTime.Now.AddMinutes(2880) // Cookied will expire in 48 hrs
                //    });

                //    return Ok(new { success = true, message = "Draft saved successfully.\nNote: This draft will be available for the next 48 hours." });
                //}

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);

                    int ncrIdObt = _context.Ncrs
                        .Where(n => n.NcrNumber == ncrProcDTO.NcrNumber)
                        .Select(n => n.NcrId)
                        .FirstOrDefault();

                    await AddPictures(ncrProcDTO, photos);

                    if (isDraft) ncrProcDTO.NcrProcFlagStatus = true;
                    NcrProcurement ncrProc = new NcrProcurement
                    {
                        NcrId = ncrIdObt,
                        NcrProcurementId = ncrProcDTO.NcrProcurementId,
                        NcrProcSupplierReturnReq = ncrProcDTO.NcrProcSupplierReturnReq,
                        NcrProcExpectedDate = ncrProcDTO.NcrProcExpectedDate,
                        NcrProcDisposedAllowed = ncrProcDTO.NcrProcDisposedAllowed,
                        NcrProcSAPReturnCompleted = ncrProcDTO.NcrProcSAPReturnCompleted,
                        NcrProcCreditExpected = ncrProcDTO.NcrProcCreditExpected,
                        NcrProcSupplierBilled = ncrProcDTO.NcrProcSupplierBilled,
                        NcrProcRejectedValue = ncrProcDTO.NcrProcRejectedValue,
                        NcrProcFlagStatus = ncrProcDTO.NcrProcFlagStatus,
                        NcrProcUserId = user.Id,
                        NcrProcCompleteDate = DateTime.Now,
                        NcrProcCreated = ncrProcDTO.NcrProcCreated,
                        SupplierReturnMANum = ncrProcDTO.SupplierReturnMANum,
                        SupplierReturnName = ncrProcDTO.SupplierReturnName,
                        SupplierReturnAccount = ncrProcDTO.SupplierReturnAccount,
                        NcrProcDefectVideo = ncrProcDTO.NcrProcDefectVideo,
                        ProcDefectPhotos = ncrProcDTO.ProcDefectPhotos,
                    };

                    _context.NcrProcurements.Add(ncrProc);
                    await _context.SaveChangesAsync();

                    //update ncr
                    var ncr = await _context.Ncrs.AsNoTracking().FirstOrDefaultAsync(n => n.NcrId == ncrIdObt);

                    if (isDraft)
                    {
                        ncr.NcrPhase = NcrPhase.Procurement;
                    }
                    else
                    {
                        ncr.NcrPhase = NcrPhase.ReInspection;
                    }
                    
                    ncr.NcrLastUpdated = DateTime.Now;
                    _context.Ncrs.Update(ncr);
                    await _context.SaveChangesAsync();

                    int ncrProcId = ncrProc.NcrProcurementId;
                    if (isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was saved as draft successfully!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " saved successfully!";
                        //Delete cookies
                        //Response.Cookies.Delete("DraftNCRProcurement" + ncr.NcrNumber);

                        //include supplier name in the email
                        var ncrOp = await _context.NcrProcurements
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrProcurementId == ncrProcId);

                        // Send notification email to Procurement
                        var subject = "New NCR Created in Procurement " + ncr.NcrNumber;
                        var emailContent = "A new NCR has been created:<br><br>NCR #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationCreate(ncrProcId, subject, emailContent);
                    }                                       

                    return RedirectToAction("Details", new { id = ncrProcId });
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
            return View(ncrProcDTO);
        }

        // GET: NcrProcurement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var ncrProc = await _context.NcrProcurements
                .Include(n => n.Ncr)
                .Include(n => n.ProcDefectPhotos)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NcrProcurementId == id);

            if (ncrProc == null)
            {
                return NotFound();
            }

            var ncrProcDTO = new NcrProcurementDTO
            {
                NcrNumber = ncrProc.Ncr.NcrNumber,
                NcrStatus = ncrProc.Ncr.NcrStatus,
                NcrProcurementId = ncrProc.NcrProcurementId,
                NcrProcSupplierReturnReq = ncrProc.NcrProcSupplierReturnReq,
                NcrProcExpectedDate = ncrProc.NcrProcExpectedDate,
                NcrProcDisposedAllowed = ncrProc.NcrProcDisposedAllowed,
                NcrProcSAPReturnCompleted = ncrProc.NcrProcSAPReturnCompleted,
                NcrProcCreditExpected = ncrProc.NcrProcCreditExpected,
                NcrProcSupplierBilled = ncrProc.NcrProcSupplierBilled,
                NcrProcRejectedValue = ncrProc.NcrProcRejectedValue,
                NcrProcFlagStatus = ncrProc.NcrProcFlagStatus,
                NcrProcUserId = user.Id,
                NcrProcCreated = ncrProc.NcrProcCreated,
                NcrProcCompleteDate = ncrProc.NcrProcCompleteDate,
                NcrId = ncrProc.NcrId,
                SupplierReturnMANum = ncrProc.SupplierReturnMANum,
                SupplierReturnName = ncrProc.SupplierReturnName,
                SupplierReturnAccount = ncrProc.SupplierReturnAccount,
                NcrProcDefectVideo = ncrProc.NcrProcDefectVideo,
                ProcDefectPhotos = ncrProc.ProcDefectPhotos,
            };

            var readOnlyDetails = await _context.Ncrs
                .Include(n => n.NcrProcurement)
                    .ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.NcrQa)
                    .ThenInclude(item => item.Supplier)
                .Include(n => n.NcrQa)
                    .ThenInclude(item => item.Item)
                .Include(n => n.NcrQa)
                            .ThenInclude(defect => defect.Defect)
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
                .Include(n => n.NcrReInspect)
                    .ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == id);

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = true;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            return View(ncrProcDTO);
        }

        // POST: NcrProcurement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NcrProcurementDTO ncrProcDTO, List<IFormFile> photos, bool isDraft = false)
        {
            if (isDraft) ncrProcDTO.NcrProcFlagStatus = true;


            if (ModelState.IsValid)
            {
                await AddPictures(ncrProcDTO, photos);
                try
                {
                    var ncrProc = await _context.NcrProcurements
                        .FirstOrDefaultAsync(n => n.NcrProcurementId == id);

                    ncrProc.NcrProcSupplierReturnReq = ncrProcDTO.NcrProcSupplierReturnReq;
                    ncrProc.NcrProcExpectedDate = ncrProcDTO.NcrProcExpectedDate;
                    ncrProc.NcrProcDisposedAllowed = ncrProcDTO.NcrProcDisposedAllowed;
                    ncrProc.NcrProcSAPReturnCompleted = ncrProcDTO.NcrProcSAPReturnCompleted;
                    ncrProc.NcrProcCreditExpected = ncrProcDTO.NcrProcCreditExpected;
                    ncrProc.NcrProcSupplierBilled = ncrProcDTO.NcrProcSupplierBilled;
                    ncrProc.NcrProcRejectedValue = ncrProcDTO.NcrProcRejectedValue;
                    ncrProc.NcrProcFlagStatus = ncrProcDTO.NcrProcFlagStatus;
                    ncrProc.NcrProcUserId = ncrProcDTO.NcrProcUserId;
                    ncrProc.NcrProcCreated = ncrProcDTO.NcrProcCreated;
                    ncrProc.NcrProcCompleteDate = ncrProcDTO.NcrProcCompleteDate;
                    ncrProc.SupplierReturnMANum = ncrProcDTO.SupplierReturnMANum;
                    ncrProc.SupplierReturnName = ncrProcDTO.SupplierReturnName;
                    ncrProc.SupplierReturnAccount = ncrProcDTO.SupplierReturnAccount;
                    ncrProc.NcrProcDefectVideo = ncrProcDTO.NcrProcDefectVideo;
                    ncrProc.ProcDefectPhotos = ncrProcDTO.ProcDefectPhotos;

                    _context.Update(ncrProc);
                    await _context.SaveChangesAsync(); //saving info for ncrProc

                    var ncr = await _context.Ncrs.AsNoTracking().FirstOrDefaultAsync(n => n.NcrId == ncrProc.NcrId);
                    ncr.NcrLastUpdated = DateTime.Now;
                    if (!isDraft) ncr.NcrPhase = NcrPhase.ReInspection;

                    _context.Update(ncr);
                    await _context.SaveChangesAsync();

                    int ncrProcId = ncrProc.NcrProcurementId;
                    if (isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was edited as draft successfully!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " edited successfully!";

                        //include supplier name in the email
                        var ncrOp = await _context.NcrProcurements
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrProcurementId == ncrProcId);

                        // Send notification email to Procurement
                        var subject = "NCR Edited in Procurement " + ncr.NcrNumber;
                        var emailContent = "A NCR has been edited :<br><br>NCR #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationEdit(ncrProcId, subject, emailContent);
                    }                                       

                    return RedirectToAction("Details", new { id = ncrProcId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NcrProcurementExists(ncrProcDTO.NcrProcurementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(ncrProcDTO);
        }

        public JsonResult GetNcrs()
        {
            //List<Ncr> pendings = _context.Ncrs
            //    .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
            //    .Include(n => n.NcrOperation)
            //    .Where(n => n.NcrPhase == NcrPhase.Procurement)
            //    .ToList();

            List<Ncr> pendings = _context.Ncrs
                .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.NcrOperation)
                .Where(n => n.NcrPhase == NcrPhase.Procurement && n.NcrProcurement.NcrProcFlagStatus != true)
                .ToList();

            // Extract relevant data for the client-side
            var ncrs = pendings.Select(ncr => new
            {
                NcrId = ncr.NcrId,
                NcrNumber = ncr.NcrNumber,
                SupplierName = ncr.NcrQa.Supplier.SupplierName,
                Created = ncr.NcrOperation.NcrOpCreationDate
            }).ToList();

            return Json(ncrs);
        }

        public JsonResult GetPendingCount()
        {
            //int pendingCount = _context.Ncrs
            //    .Where(n => n.NcrPhase == NcrPhase.Procurement)
            //    .Count();

            int pendingCount = _context.Ncrs
                .Where(n => n.NcrPhase == NcrPhase.Procurement && n.NcrProcurement.NcrProcFlagStatus != true)
                .Count();

            return Json(pendingCount);
        }
        private async Task AddPictures(NcrProcurementDTO ncrProcDTO, List<IFormFile> pictures)
        {
            if (pictures != null && pictures.Any())
            {
                ncrProcDTO.ProcDefectPhotos = new List<ProcDefectPhoto>();

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

                            ncrProcDTO.ProcDefectPhotos.Add(new ProcDefectPhoto
                            {
                                ProcDefectPhotoContent = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                ProcDefectPhotoMimeType = "image/webp",
                                FileName = picture.FileName
                            });
                        }
                    }
                }
            }
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.ProcDefectPhotos
                .Include(d => d.ProcFileContent)
                .Where(f => f.ProcDefectPhotoId == id)
                .FirstOrDefaultAsync();
            return File(theFile.ProcDefectPhotoContent, theFile.ProcDefectPhotoMimeType, theFile.FileName);
        }

        private SelectList SupplierSelectList()
        {
            return new SelectList(_context.Suppliers
                .Where(s => s.SupplierStatus == true && s.SupplierName != "NO SUPPLIER PROVIDED")
                .OrderBy(s => s.SupplierName), "SupplierId", "SupplierName");
        }

        private SelectList ItemSelectList()
        {
            return new SelectList(_context.Items
                .OrderBy(s => s.ItemName), "ItemId", "ItemName");
        }

        private void PopulateDropDownLists(NcrQa ncrQa = null)
        {            
                if (ncrQa.Item == null)
                {
                    ncrQa.Item = _context.Items.Find(ncrQa.ItemId);
                }
                ViewData["SupplierId"] = SupplierSelectList();
                ViewData["ItemId"] = ItemSelectList();           
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.ProcDefectPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.ProcDefectPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Photo deleted successfully." });
            }
            return Json(new { success = false, message = "Photo not found." });
        }
        private bool NcrProcurementExists(int id)
        {
            return _context.NcrProcurements.Any(e => e.NcrProcurementId == id);
        }

        //// CREATE/POST: Email-Notification
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
                var qualityUsers = await _userManager.GetUsersInRoleAsync("Quality");
                var emailAddresses = qualityUsers.Select(u => new EmailAddress
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
                    ViewData["Message"] = "Message NOT sent! No Quality users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to Procurement users. Error: {errMsg}";
            }

            return View();
        }

        //// EDIT/POST: Email Notification
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
                var emailAddresses = qualityUsers.Select(u => new EmailAddress
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
                    ViewData["Message"] = "Message NOT sent! No Quality users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to Quality users. Error: {errMsg}";
            }

            return View();
        }

        //24 Hour Notification
        public async Task CheckAndSendEmailNotifications()
        {
            // Get pending NCRs from Engineering that are older than 24 hours
            var pendingNCRs = await _context.NcrOperations
                .Where(n => n.Ncr.NcrPhase == NcrPhase.QualityInspector && n.NcrOpCreationDate <= DateTime.Now.AddHours(-24))
                .ToListAsync();

            foreach (var ncr in pendingNCRs)
            {
                // Check if an NCR has not been created in Operations
                var ncrInOps = await _context.NcrProcurements.FirstOrDefaultAsync(op => op.NcrId == ncr.NcrId);
                if (ncrInOps == null)
                {
                    // Check if it's exactly 24 hours since the NCR was created in Engineering
                    if (DateTime.Now.Subtract(ncr.NcrOpCreationDate).TotalHours == 24)
                    {
                        // Send notification email to Operations or Admin role
                        var subject = "NCR Pending in Procurement";
                        var emailContent = "An NCR from Operations is pending in Operations and has not been created yet.";
                        await NotificationCreate(ncr.NcrId, subject, emailContent);
                    }
                }
            }
        }

        public IActionResult ExportToExcel()
        {
            var ncrOperation = _context.NcrProcurements
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
                worksheet.Cells[2, 3].Value = "Supplier Return";
                worksheet.Cells[2, 4].Value = "SAP Completed";
                worksheet.Cells[2, 5].Value = "Phase";
                worksheet.Cells[2, 6].Value = "Created";
                worksheet.Cells[2, 7].Value = "Last Update";

                // Apply center alignment to the header cells
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the header with col span over all columns
                worksheet.Cells[1, 1, 1, 7].Merge = true;  // Merge 7 columns starting from A1
                worksheet.Cells[1, 1].Value = "NCR Procurement Log";

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
                foreach (var item in ncrOperation)
                {
                    worksheet.Cells[row, 1].Value = item.Ncr.NcrNumber;
                    worksheet.Cells[row, 2].Value = item.Ncr.NcrQa.Supplier.SupplierName;
                    worksheet.Cells[row, 3].Value = item.NcrProcSupplierReturnReq ? "Yes" : "No";
                    worksheet.Cells[row, 4].Value = item.NcrProcSAPReturnCompleted ? "Yes" : "No";
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
                var fileName = "NCR_Procurement_Data.xlsx";

                // Return the Excel file as a file download
                return File(content, contentType, fileName);
            }
        }
    }
}
