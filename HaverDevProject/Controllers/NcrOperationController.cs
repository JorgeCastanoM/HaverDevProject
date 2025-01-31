using HaverDevProject.CustomControllers;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;


namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Operations, Admin")]
    public class NcrOperationController : ElephantController
    {
        //for sending email
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly HaverNiagaraContext _context;
        public NcrOperationController(HaverNiagaraContext context, IMyEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: NcrOperation
        public async Task<IActionResult> Index(string SearchCode, int? OpDispositionTypeId, DateTime StartDate, DateTime EndDate,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "desc", string sortField = "Creation", string filter = "Active")
        {

            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //Set the date range filer based on the values in the database
            if (EndDate == DateTime.MinValue)
            {
                StartDate = _context.Ncrs
                    .Min(f => f.NcrQa.NcrQacreationDate.Date)
                    .Subtract(TimeSpan.FromDays(1));

                EndDate = _context.Ncrs
                    .Max(f => f.NcrQa.NcrQacreationDate.Date)
                    .Add(TimeSpan.FromDays(1));

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
            string[] sortOptions = new[] { "Creation", "NCR #", "Supplier", "DispositionType", "Phase", "Created" };

            PopulateDropDownLists();

            var ncrOperation = _context.NcrOperations
                .Include(n => n.NcrEng)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.OpDispositionType)
                .Include(n => n.FollowUpType)
                .Where(n => n.Ncr.NcrPhase != NcrPhase.Archive)
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
                    ncrOperation = ncrOperation.Where(n => n.Ncr.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    ncrOperation = ncrOperation.Where(n => n.Ncr.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncrOperation = ncrOperation.Where(s => s.Ncr.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (OpDispositionTypeId.HasValue)
            {
                ncrOperation = ncrOperation.Where(n => n.OpDispositionType.OpDispositionTypeId == OpDispositionTypeId);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncrOperation = ncrOperation.Where(n => n.Ncr.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncrOperation = ncrOperation.Where(n => n.Ncr.NcrQa.NcrQacreationDate >= StartDate &&
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
            //Now we know which field and direction to sort by
            if (sortField == "NCR #")
            {
                if (sortDirection == "asc")
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.Ncr.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.UpdateOp);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.UpdateOp);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Creation")
            {
                if (sortDirection == "asc") //asc by default
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Creation"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.Ncr.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Creation"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Status")
            {
                if (sortDirection == "asc")
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "DispositionType")
            {
                if (sortDirection == "asc")
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.OpDispositionType.OpDispositionTypeName);
                    ViewData["filterApplied:DispositionType"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.OpDispositionType.OpDispositionTypeName);
                    ViewData["filterApplied:DispositionType"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncrOperation = ncrOperation
                        .OrderBy(p => p.Ncr.NcrPhase);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrOperation = ncrOperation
                        .OrderByDescending(p => p.Ncr.NcrPhase);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }


            //Set sort for next time

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NcrOperation>.CreateAsync(ncrOperation.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: NcrOperation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NcrOperations == null)
            {
                return NotFound();
            }

            var ncrOperation = await _context.NcrOperations
                .Include(n => n.FollowUpType)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(i => i.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Defect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.ItemDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.EngDispositionType)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.Drawing)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrEng).ThenInclude(n => n.EngDefectPhotos)
                .Include(n => n.OpDispositionType)
                .Include(n => n.OpDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement).ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect).ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(m => m.NcrOpId == id);

            if (ncrOperation == null)
            {
                return NotFound();
            }

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = true;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.NCRSectionId = id;

            return View(ncrOperation);
        }

        // GET: NcrOperation/Create
        public async Task<IActionResult> Create(string ncrNumber)
        {

            NcrOperationDTO ncr;

            int ncrId = _context.Ncrs.Where(n => n.NcrNumber == ncrNumber).Select(n => n.NcrId).FirstOrDefault();

            // Check if there are info in cookies
            if (Request.Cookies.TryGetValue("DraftNCROperation" + ncrNumber, out string draftJson))
            {
                // Convert the data from json file
                ncr = JsonConvert.DeserializeObject<NcrOperationDTO>(draftJson);
                TempData["SuccessMessage"] = "Draft successfully retrieved";
            }
            else
            {

                ncr = new NcrOperationDTO
                {
                    NcrNumber = ncrNumber, // Set the NcrNumber from the parameter
                    NcrOpCreationDate = DateTime.Now,
                    NcrOpCompleteDate = DateTime.Now,
                    UpdateOp = DateTime.Now,
                    ExpectedDate = DateTime.Now,
                    NcrStatus = true, // Active
                    FollowUp = true,
                    Car = true
                };
            }

            var readOnlyDetails = await _context.Ncrs
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
                .FirstOrDefaultAsync(n => n.NcrId == ncrId);


            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = true;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            PopulateDropDownLists();
            return View(ncr);
        }

        // POST: NcrOperation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NcrOperationDTO ncrOperationDTO, List<IFormFile> Photos, bool isDraft = false)
        {
            try
            {
                //if (isDraft)
                //{
                //    // convert the object to json format
                //    var json = JsonConvert.SerializeObject(ncrOperationDTO);

                //    // Save the object in a cookie with name "DraftData"
                //    Response.Cookies.Append("DraftNCROperation" + ncrOperationDTO.NcrNumber, json, new CookieOptions
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
                        .Where(n => n.NcrNumber == ncrOperationDTO.NcrNumber)
                        .Select(n => n.NcrId)
                        .FirstOrDefault();

                    await AddPictures(ncrOperationDTO, Photos);

                    if (isDraft) ncrOperationDTO.NcrOpStatusFlag = true;

                    NcrOperation ncrOperation = new NcrOperation
                    {
                        NcrId = ncrIdObt,
                        OpDispositionTypeId = ncrOperationDTO.OpDispositionTypeId,
                        NcrPurchasingDescription = ncrOperationDTO.NcrPurchasingDescription,
                        Car = ncrOperationDTO.Car,
                        CarNumber = ncrOperationDTO.CarNumber,
                        FollowUp = ncrOperationDTO.FollowUp,
                        ExpectedDate = ncrOperationDTO.ExpectedDate,
                        NcrOpCreationDate = DateTime.Now,
                        NcrOpCompleteDate = DateTime.Now,
                        FollowUpTypeId = ncrOperationDTO.FollowUpTypeId,
                        UpdateOp = DateTime.Now,
                        NcrOperationUserId = user.Id,
                        OpDefectPhotos = ncrOperationDTO.OpDefectPhotos,
                        NcrOperationVideo = ncrOperationDTO.NcrOperationVideo,
                        NcrOpStatusFlag = ncrOperationDTO.NcrOpStatusFlag
                    };

                    _context.NcrOperations.Add(ncrOperation);
                    await _context.SaveChangesAsync();

                    //update ncr 
                    var ncr = await _context.Ncrs.AsNoTracking().FirstOrDefaultAsync(n => n.NcrId == ncrIdObt);

                    if (isDraft)
                    {
                        ncr.NcrPhase = NcrPhase.Operations;
                    }
                    else
                    {
                        ncr.NcrPhase = NcrPhase.Procurement;
                    }
                    _context.Ncrs.Update(ncr);
                    await _context.SaveChangesAsync();

                    int ncrOpId = ncrOperation.NcrOpId;
                    if (isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was saved as draft successfully!";
                    }
                    else
                    {                       
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " saved successfully!";
                        //Delete cookies
                        //Response.Cookies.Delete("DraftNCROperation" + ncr.NcrNumber);
                    
                        //include supplier name in the email
                        var ncrOp = await _context.NcrOperations
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrOpId == ncrOpId);

                        // Send notification email to Procurement
                        var subject = "New NCR Created " + ncr.NcrNumber + "  from Operations";
                        var emailContent = "A new NCR has been created:<br><br>Ncr #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationCreate(ncrOpId, subject, emailContent);
                    }
                    return RedirectToAction("Details", new { id = ncrOpId });
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

            return View(ncrOperationDTO);
        }

        // GET: NcrOperation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var ncrOperation = await _context.NcrOperations
                .Include(n => n.NcrEng)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Defect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.OpDispositionType)
                .Include(n => n.FollowUpType)
                .Include(n => n.OpDefectPhotos)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NcrOpId == id);

            if (ncrOperation == null)
            {
                return NotFound();
            }

            var ncrOperationDTO = new NcrOperationDTO
            {
                NcrOpId = ncrOperation.NcrOpId,
                NcrNumber = ncrOperation.Ncr.NcrNumber,
                NcrOpCreationDate = ncrOperation.NcrOpCreationDate,
                NcrOpCompleteDate = ncrOperation.NcrOpCompleteDate,
                NcrId = ncrOperation.NcrId,
                Ncr = ncrOperation.Ncr,
                OpDispositionTypeId = ncrOperation.OpDispositionTypeId,
                OpDispositionType = ncrOperation.OpDispositionType,
                NcrPurchasingDescription = ncrOperation.NcrPurchasingDescription,
                Car = ncrOperation.Car,
                CarNumber = ncrOperation.CarNumber,
                FollowUp = ncrOperation.FollowUp,
                ExpectedDate = DateTime.Now,
                FollowUpTypeId = ncrOperation.FollowUpTypeId,
                UpdateOp = ncrOperation.UpdateOp,
                NcrOperationUserId = user.Id,
                NcrEng = ncrOperation.NcrEng,
                NcrOperationVideo = ncrOperation.NcrOperationVideo,
                OpDefectPhotos = ncrOperation.OpDefectPhotos,
                NcrOpStatusFlag = ncrOperation.NcrOpStatusFlag
            };

            var readOnlyDetails = await _context.Ncrs
                .Include(n => n.NcrQa)
                        .ThenInclude(item => item.Supplier)
                .Include(n => n.NcrQa)
                        .ThenInclude(defect => defect.Defect)
                .Include(n => n.NcrQa)
                        .ThenInclude(defect => defect.Item)
                .Include(n => n.NcrQa)
                    .ThenInclude(qa => qa.ItemDefectPhotos)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.EngDispositionType)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.Drawing)
                .Include(n => n.NcrEng)
                    .ThenInclude(eng => eng.EngDefectPhotos)
                .FirstOrDefaultAsync(n => n.NcrId == id);

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;

            ViewData["FollowUpTypeId"] = new SelectList(_context.FollowUpTypes, "FollowUpTypeId", "FollowUpTypeName", ncrOperation.FollowUpTypeId);
            ViewData["OpDispositionTypeId"] = new SelectList(_context.OpDispositionTypes, "OpDispositionTypeId", "OpDispositionTypeName", ncrOperation.OpDispositionTypeId);
            return View(ncrOperationDTO);
        }

        // POST: NcrOperation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NcrOperationDTO ncrOperationDTO, List<IFormFile> Photos, bool isDraft = false)
        {
            if (isDraft) ncrOperationDTO.NcrOpStatusFlag = true; 

            if (ModelState.IsValid)
            {
                await AddPictures(ncrOperationDTO, Photos);
                try
                {
                    var ncrOperation = await _context.NcrOperations
                    .Include(n => n.Ncr)
                    .Include(n => n.OpDispositionType)
                    .Include(n => n.FollowUpType)
                    .FirstOrDefaultAsync(ne => ne.NcrOpId == id);

                    ncrOperation.OpDispositionTypeId = ncrOperationDTO.OpDispositionTypeId;
                    ncrOperation.NcrPurchasingDescription = ncrOperationDTO.NcrPurchasingDescription;
                    ncrOperation.NcrOpCreationDate = ncrOperationDTO.NcrOpCreationDate;
                    ncrOperation.NcrOpCompleteDate = ncrOperationDTO.NcrOpCompleteDate;
                    ncrOperation.Car = ncrOperationDTO.Car;
                    ncrOperation.CarNumber = ncrOperationDTO.CarNumber;
                    ncrOperation.FollowUp = ncrOperationDTO.FollowUp;
                    ncrOperation.ExpectedDate = ncrOperationDTO.ExpectedDate;
                    ncrOperation.FollowUpTypeId = ncrOperationDTO.FollowUpTypeId;
                    ncrOperation.UpdateOp = DateTime.Today;
                    ncrOperation.NcrOperationUserId = "b58514b4-b008-43a4-bae2-4a4f4f5408ff";
                    ncrOperation.NcrOperationVideo = ncrOperationDTO.NcrOperationVideo;
                    ncrOperation.OpDefectPhotos = ncrOperationDTO.OpDefectPhotos;
                    ncrOperation.NcrOpStatusFlag = ncrOperationDTO.NcrOpStatusFlag;

                    _context.Update(ncrOperation);
                    await _context.SaveChangesAsync();

                    var ncr = await _context.Ncrs.FirstOrDefaultAsync(n => n.NcrId == ncrOperation.NcrId);
                    ncr.NcrLastUpdated = DateTime.Now;
                    if (!isDraft) ncr.NcrPhase = NcrPhase.Procurement;
                    _context.Update(ncr);
                    await _context.SaveChangesAsync();
                    
                    int ncrOpId = ncrOperation.NcrOpId;

                    if(isDraft)
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was edited as draft successfully!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " edited successfully!";

                        //include supplier name in the email
                        var ncrOp = await _context.NcrOperations
                            .Include(n => n.Ncr)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                            .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                            .FirstOrDefaultAsync(n => n.NcrOpId == ncrOpId);

                        // Send notification email to Procurement
                        var subject = "NCR Edited " + ncr.NcrNumber + "  from Operations";
                        var emailContent = "A NCR has been edited :<br><br>Ncr #: " + ncr.NcrNumber + "<br>Supplier: " + ncr.NcrQa.Supplier.SupplierName;
                        await NotificationEdit(ncrOpId, subject, emailContent);
                    }                    

                    return RedirectToAction("Details", new { id = ncrOpId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NcrOperationExists(ncrOperationDTO.NcrOpId))
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
            return View(ncrOperationDTO);
        }

        private bool NcrOperationExists(int id)
        {
            return _context.NcrOperations.Any(e => e.NcrOpId == id);
        }

        private SelectList OpDispositionTypeSelectList(int? selectedId)
        {
            return new SelectList(_context.OpDispositionTypes
                .OrderBy(s => s.OpDispositionTypeName), "OpDispositionTypeId", "OpDispositionTypeName", selectedId);
        }

        private SelectList FollowUpTypeSelectList(int? selectedId)
        {
            return new SelectList(_context.FollowUpTypes
                .OrderBy(s => s.FollowUpTypeName), "FollowUpTypeId", "FollowUpTypeName", selectedId);
        }

        public JsonResult GetNcrs()
        {
            //List<Ncr> pendings = _context.Ncrs
            //    .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
            //    .Include(n => n.NcrEng)
            //    .Where(n => n.NcrPhase == NcrPhase.Operations)
            //    .ToList();

            List<Ncr> pendings = _context.Ncrs
                .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.NcrEng)
                .Where(n => n.NcrPhase == NcrPhase.Operations && n.NcrOperation.NcrOpStatusFlag != true)
                .ToList();

            // Extract relevant data for the client-side
            var ncrs = pendings.Select(ncr => new
            {
                NcrId = ncr.NcrId,
                NcrNumber = ncr.NcrNumber,
                SupplierName = ncr.NcrQa.Supplier.SupplierName,
                Created = ncr.NcrEng?.NcrEngCompleteDate ?? ncr.NcrQa?.NcrQacreationDate
            }).ToList();

            return Json(ncrs);
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.OpDefectPhotos
                .Include(d => d.OpFileContent)
                .Where(f => f.OpDefectPhotoId == id)
                .FirstOrDefaultAsync();
            return File(theFile.OpDefectPhotoContent, theFile.OpDefectPhotoMimeType, theFile.FileName);
        }

        private async Task AddPictures(NcrOperationDTO ncrOperationDTO, List<IFormFile> pictures)
        {
            if (pictures != null && pictures.Any())
            {
                ncrOperationDTO.OpDefectPhotos = new List<OpDefectPhoto>();

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

                            ncrOperationDTO.OpDefectPhotos.Add(new OpDefectPhoto
                            {
                                OpDefectPhotoContent = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                OpDefectPhotoMimeType = "image/webp",
                                FileName = picture.FileName
                            });
                        }

                    }
                }
            }
        }

        //Pending NCRs count
        public JsonResult GetPendingCount()
        {
            //int pendingCount = _context.Ncrs
            //    .Where(n => n.NcrPhase == NcrPhase.Operations)
            //    .Count();

            int pendingCount = _context.Ncrs
                .Where(n => n.NcrPhase == NcrPhase.Operations && n.NcrOperation.NcrOpStatusFlag != true)
                .Count();

            return Json(pendingCount);
        }

        private void PopulateDropDownLists(NcrOperation ncrOperation = null)
        {
            if ((ncrOperation?.OpDispositionTypeId).HasValue)
            {
                if (ncrOperation.OpDispositionType == null)
                {
                    ncrOperation.OpDispositionType = _context.OpDispositionTypes.Find(ncrOperation.OpDispositionTypeId);
                }
                ViewData["OpDispositionTypeId"] = OpDispositionTypeSelectList(ncrOperation?.OpDispositionTypeId);
                ViewData["FollowUpTypeId"] = FollowUpTypeSelectList(ncrOperation?.FollowUpTypeId);
            }
            else
            {
                ViewData["OpDispositionTypeId"] = OpDispositionTypeSelectList(null);
                ViewData["FollowUpTypeId"] = FollowUpTypeSelectList(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.OpDefectPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.OpDefectPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Photo deleted successfully." });
            }
            return Json(new { success = false, message = "Photo not found." });
        }

        //// CREATE/POST: Operation/Notification/5
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
                var procurementUsers = await _userManager.GetUsersInRoleAsync("Procurement");
                var emailAddresses = procurementUsers.Select(u => new EmailAddress
                {
                    Name = u.UserName,
                    Address = u.Email
                }).ToList();

                if (emailAddresses.Any())
                {
                    string link = "https://haverv2team3.azurewebsites.net/ncroperation/details/" + id;
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
                ViewData["Message"] = $"Error: Could not send email message to Procurement users. Error: {errMsg}";
            }

            return View();
        }

        //// EDIT/POST: Operation/Notification/5
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
                var procurementUsers = await _userManager.GetUsersInRoleAsync("Procurement");
                var qualityUsers = await _userManager.GetUsersInRoleAsync("Quality");
                var allUsers = procurementUsers.Concat(qualityUsers).Distinct();
                var emailAddresses = allUsers.Select(u => new EmailAddress
                {
                    Name = u.UserName,
                    Address = u.Email
                }).ToList();

                if (emailAddresses.Any())
                {
                    string link = "https://haverv2team3.azurewebsites.net/ncroperation/details/" + id;
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
                ViewData["Message"] = $"Error: Could not send email message to Procurement users. Error: {errMsg}";
            }

            return View();
        }

        //24 Hour Notification
        public async Task CheckAndSendEmailNotifications()
        {
            // Get pending NCRs from Engineering that are older than 24 hours
            var pendingNCRs = await _context.NcrEngs
                .Where(n => n.NcrPhase == NcrPhase.Engineer && n.NcrEngCreationDate <= DateTime.Now.AddHours(-24))
                .ToListAsync();

            foreach (var ncr in pendingNCRs)
            {
                // Check if an NCR has not been created in Operations
                var ncrInOps = await _context.NcrOperations.FirstOrDefaultAsync(op => op.NcrId == ncr.NcrId);
                if (ncrInOps == null)
                {
                    // Check if it's exactly 24 hours since the NCR was created in Engineering
                    if (DateTime.Now.Subtract(ncr.NcrEngCreationDate).TotalHours == 24)
                    {
                        // Send notification email to Operations or Admin role
                        var subject = "NCR Pending in Operations";
                        var emailContent = "An NCR from Engineering is pending in Operations and has not been created yet.";
                        await NotificationCreate(ncr.NcrId, subject, emailContent);
                    }
                }
            }
        }

        //Log report excel
        public IActionResult ExportToExcel()
        {
            var ncrOperation = _context.NcrOperations
                .Include(n => n.NcrEng)
                .Include(n => n.Ncr)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.OpDispositionType)
                .Include(n => n.FollowUpType)
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
                worksheet.Cells[2, 3].Value = "Disposition Type";
                worksheet.Cells[2, 4].Value = "Phase";
                worksheet.Cells[2, 5].Value = "Created";
                worksheet.Cells[2, 6].Value = "Last Update";

                // Apply center alignment to the header cells
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the header with col span over all columns
                worksheet.Cells[1, 1, 1, 6].Merge = true;  // Merge 6 columns starting from A1
                worksheet.Cells[1, 1].Value = "NCR Operation Log";

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
                    worksheet.Cells[row, 3].Value = item.OpDispositionType.OpDispositionTypeName;
                    worksheet.Cells[row, 4].Value = item.Ncr.NcrPhase.ToString();
                    worksheet.Cells[row, 5].Value = item.NcrOpCreationDate.ToString();
                    worksheet.Cells[row, 6].Value = item.Ncr.NcrLastUpdated.ToString();

                    row++;
                }

                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set content type and filename for the Excel file
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "NCR_Operation_Data.xlsx";

                // Return the Excel file as a file download
                return File(content, contentType, fileName);
            }
        }

    }
}