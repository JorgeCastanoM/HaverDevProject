using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.CustomControllers;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Ocsp;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Quality, Admin")]
    [ActiveUserOnly]
    public class NcrQaController : ElephantController
    {
        //for sending email
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HaverNiagaraContext _context;

        public NcrQaController(HaverNiagaraContext context, IMyEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        
        // GET: NcrQa
        public async Task<IActionResult> Index(string SearchCode, string SearchSupplier, DateTime StartDate, DateTime EndDate,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "desc", string sortField = "Created", string filter = "Active")
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

            //List of sort options.
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "Defect", "PO Number", "Phase", "Last Updated" };

            var ncrQa = _context.NcrQas
                //.Include(n => n.Item).ThenInclude(n => n.ItemDefects).ThenInclude(n => n.Defect)
                .Include(n => n.Supplier)
                .Include(n => n.Defect)
                .Include(n => n.Ncr)
                .Where(n => n.Ncr.NcrPhase != NcrPhase.Archive)
                .AsNoTracking();                       

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
                    ncrQa = ncrQa.Where(n => n.Ncr.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    ncrQa = ncrQa.Where(n => n.Ncr.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncrQa = ncrQa.Where(s => s.Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper() ) 
                || s.Ncr.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                ncrQa = ncrQa.Where(n => n.Supplier.SupplierName == SearchSupplier);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncrQa = ncrQa.Where(n => n.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncrQa = ncrQa.Where(n => n.NcrQacreationDate >= StartDate && 
                         n.NcrQacreationDate <= EndDate);   
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
                    ncrQa = ncrQa
                        .OrderBy(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.Ncr.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Defect")
            {
                if (sortDirection == "asc")
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.Defect.DefectName); 
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.Defect.DefectName);  
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.Supplier.SupplierName); 
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "asc") //asc by default
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.NcrQacreationDate); 

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.Ncr.NcrPhase); 
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.Ncr.NcrPhase);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.Ncr.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.Ncr.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "PO Number")
            {
                if (sortDirection == "asc")
                {
                    ncrQa = ncrQa
                        .OrderBy(p => p.NcrQaOrderNumber); 
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncrQa = ncrQa
                        .OrderByDescending(p => p.NcrQaOrderNumber); 
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NcrQa>.CreateAsync(ncrQa.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);            
        }

        // GET: NcrQa/Details/5
        public async Task<IActionResult> Details(int? id, string referrer)
        {
            if (id == null || _context.NcrQas == null)
            {
                return NotFound();
            }
            var ncrQa = await _context.NcrQas
                .Include(n => n.Ncr)
                .Include(i => i.Supplier)
                .Include(i => i.Item)
                .Include(n => n.Defect)
                .Include(n => n.ItemDefectPhotos)
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
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrReInspect).ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(n => n.NcrQaId == id);

            if (ncrQa == null)
            {
                return NotFound();
            }

            ViewBag.IsNCRQaView = true;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.NCRSectionId = id;

            return View(ncrQa);
        }

        // GET: NcrQa/Create
        public IActionResult Create()
        {            
            NcrQaDTO ncrQaDTO;

            // Check if there are info in cookies
            if (Request.Cookies.TryGetValue("DraftNCRQa", out string draftJson))
            {
                // Convert the data from json file
                ncrQaDTO = JsonConvert.DeserializeObject<NcrQaDTO>(draftJson);
                TempData["SuccessMessage"] = "Draft successfully retrieved";
            }
            else
            {                
                ncrQaDTO = new NcrQaDTO
                {
                    NcrNumber = GetNcrNumber(),
                    NcrQacreationDate = DateTime.Today,
                    NcrStatus = true, //Active
                    NcrQaProcessApplicable = true, //Supplier or Rec-Insp
                    NcrQaItemMarNonConforming = true, //Yes
                    NcrQaEngDispositionRequired = true //Yes
                };
            }
            PopulateDropDownLists();
            return View(ncrQaDTO);            
        }

        // POST: NcrQa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int NcrQaId, int NcrId, NcrQaDTO ncrQaDTO, List<IFormFile> Photos, bool isDraft = false) 
        {    
            // validate if there are cookies available
            //if (isDraft)
            //{
            //    // convert the object to json format
            //    var json = JsonConvert.SerializeObject(ncrQaDTO);

            //    // Save the object in a cookie with name "DraftData"
            //    Response.Cookies.Append("DraftNCRQa", json, new CookieOptions
            //    {
            //        // Define time for cookies
            //        Expires = DateTime.Now.AddMinutes(2880) // Cookies will expire in 48 hrs
            //    });

            //    return Ok(new { success = true, message = "Draft saved successfully.\nNote: This draft will be available for the next 48 hours." });                
            //}

            if (ModelState.IsValid)
            {
                
                var user = await _userManager.GetUserAsync(User);
                string NcrNewNumberValidated = GetNcrNumber();
                bool engReq = ncrQaDTO.NcrQaEngDispositionRequired == true ? true : false;
                                
                Ncr ncr = new Ncr
                {
                    NcrNumber = NcrNewNumberValidated,
                    NcrLastUpdated = DateTime.Now,
                    NcrStatus = ncrQaDTO.NcrStatus,
                    NcrPhase = ncrQaDTO.NcrQaEngDispositionRequired == true ? NcrPhase.Engineer : NcrPhase.Operations,
                    ParentId = ncrQaDTO.ParentId,
                };

                if (isDraft) ncr.NcrPhase = NcrPhase.QualityInspector;

                _context.Add(ncr);
                await _context.SaveChangesAsync();

                //getting the ncrId through the NcrNumber 
                int ncrIdObt = _context.Ncrs
                    .Where(n => n.NcrNumber == NcrNewNumberValidated)
                    .Select(n => n.NcrId)
                    .FirstOrDefault();

                await AddPictures(ncrQaDTO, Photos);
                
                NcrQa ncrQa = new NcrQa
                {
                    NcrQaItemMarNonConforming = ncrQaDTO.NcrQaItemMarNonConforming,
                    NcrQaProcessApplicable = ncrQaDTO.NcrQaProcessApplicable,
                    NcrQacreationDate = ncrQaDTO.NcrQacreationDate,
                    NcrQaOrderNumber = ncrQaDTO.NcrQaOrderNumber,
                    NcrQaSalesOrder = ncrQaDTO.NcrQaSalesOrder,
                    NcrQaQuanReceived = ncrQaDTO.NcrQaQuanReceived,
                    NcrQaQuanDefective = ncrQaDTO.NcrQaQuanDefective,
                    NcrQaDescriptionOfDefect = ncrQaDTO.NcrQaDescriptionOfDefect,
                    NcrQaDefectVideo = ncrQaDTO.NcrQaDefectVideo,
                    ItemDefectPhotos = ncrQaDTO.ItemDefectPhotos,
                    NcrQaUserId = user.Id,  
                    NcrId = ncrIdObt,
                    SupplierId = ncrQaDTO.SupplierId,
                    ItemId = ncrQaDTO.ItemId,
                    DefectId = ncrQaDTO.DefectId,
                    NcrQaEngDispositionRequired = ncrQaDTO.NcrQaEngDispositionRequired
                };
                ncrQa.NcrQaStatusFlag = isDraft? true : false;

                _context.NcrQas.Add(ncrQa);
                await _context.SaveChangesAsync();

                if (ncrQaDTO.ParentId.HasValue)
                {
                    var ncrReInspect = await _context.NcrReInspects.FirstOrDefaultAsync(n => n.NcrId == ncrQaDTO.ParentId);
                    if (ncrReInspect != null)
                    {
                        ncrReInspect.NcrReInspectNewNcrNumber = ncr.NcrNumber;
                        _context.Update(ncrReInspect);
                        await _context.SaveChangesAsync();
                    }
                }
                int ncrQaId = ncrQa.NcrQaId;
                if (isDraft)
                {
                    TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " was saved as draft successfully!";
                }
                else
                {
                    TempData["SuccessMessage"] = "NCR " + ncr.NcrNumber + " saved successfully!";
                    //Delete cookies
                    //Response.Cookies.Delete("DraftNCRQa");                

                    var ncrEmail = await _context.Ncrs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == NcrId);

                    //include supplier name in the email
                    var Supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(n => n.SupplierId == ncrQa.SupplierId);

                    // Send notification email to Eng or Ops
                    var subject = "New NCR Created " + ncr.NcrNumber + "  from Quality";
                    var emailContent = "A new NCR has been created:<br><br>Ncr #: " + ncr.NcrNumber + "<br>Supplier: " + Supplier.SupplierName;
                    await NotificationCreate(NcrQaId, subject, emailContent);
                }                              
                
                return RedirectToAction("Details", new { id = ncrQaId, referrer = "Create" });
            }

            PopulateDropDownLists();
            return View(ncrQaDTO);
        }

        // GET: NcrQa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null || _context.NcrQas == null)
            {
                return NotFound();
            }

            var ncrQa = await _context.NcrQas
                .Include(n =>n.Ncr)
                .Include(n => n.Supplier)
                .Include(n =>n.Defect)
                .Include(n =>n.ItemDefectPhotos)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NcrQaId == id);

            if (ncrQa == null)
            {
                return NotFound();
            }

            var ncrQaDTO = new NcrQaDTO
            {
                NcrQaId = ncrQa.NcrQaId,
                NcrQaItemMarNonConforming = ncrQa.NcrQaItemMarNonConforming,
                NcrQaProcessApplicable = ncrQa.NcrQaProcessApplicable,
                NcrQacreationDate = ncrQa.NcrQacreationDate,
                NcrQaOrderNumber = ncrQa.NcrQaOrderNumber,
                NcrQaSalesOrder = ncrQa.NcrQaSalesOrder,
                NcrQaQuanReceived = ncrQa.NcrQaQuanReceived,
                NcrQaQuanDefective = ncrQa.NcrQaQuanDefective,
                NcrQaDescriptionOfDefect = ncrQa.NcrQaDescriptionOfDefect,
                NcrQaUserId = user.Id,
                NcrId = ncrQa.NcrId,                
                SupplierId = ncrQa.SupplierId,
                NcrNumber = ncrQa.Ncr.NcrNumber,
                NcrPhase = ncrQa.Ncr.NcrPhase,
                ItemId = ncrQa.ItemId,
                DefectId = ncrQa.DefectId,
                NcrQaEngDispositionRequired = ncrQa.NcrQaEngDispositionRequired,
                NcrQaDefectVideo = ncrQa.NcrQaDefectVideo,
                ItemDefectPhotos = ncrQa.ItemDefectPhotos,
                NcrQaStatusFlag = ncrQa.NcrQaStatusFlag
            };

            PopulateDropDownLists();

            var readOnlyDetails = await _context.Ncrs
                .Include(n=>n.NcrProcurement)
                .Include(n => n.NcrProcurement)
                     .ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.NcrReInspect)
                .Include(n=>n.NcrReInspect)
                    .ThenInclude(n=>n.NcrReInspectPhotos)
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
                .FirstOrDefaultAsync(n => n.NcrId == id);

            ViewBag.IsNCRQaView = false;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.ncrDetails = readOnlyDetails;   

            return View(ncrQaDTO);
        }

        // POST: NcrQa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int NcrQaId, int NcrId, NcrQaDTO ncrQaDTO, List<IFormFile> Photos, bool isDraft = false)
        {
            if (isDraft) ncrQaDTO.NcrQaStatusFlag = true;

            if (ModelState.IsValid)
            {
                var ncrToUpdate = await _context.Ncrs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n =>n.NcrId == NcrId);

                var ncrOperations = await _context.NcrOperations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == NcrId);
                bool ncrOperationExist = ncrOperations != null;

                var ncrEng = await _context.NcrEngs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == NcrId);                
                bool ncrEngExist = ncrEng != null;                

                if (ncrToUpdate == null)
                {
                    return NotFound();
                }                

                else
                {
                    if (isDraft == false)
                    {
                        if (ncrQaDTO.NcrQaEngDispositionRequired == true && ncrEngExist == false)
                        {
                            ncrToUpdate.NcrPhase = NcrPhase.Engineer;
                        }
                        if (ncrQaDTO.NcrQaEngDispositionRequired == false && ncrOperationExist == false)
                        {
                            ncrToUpdate.NcrPhase = NcrPhase.Operations;
                        }
                    }

                    ncrToUpdate.NcrLastUpdated = DateTime.Now;

                    _context.Ncrs.Update(ncrToUpdate);
                    await _context.SaveChangesAsync();
                }

                // Go get the ncrQa to update
                var ncrQaToUpdate = await _context.NcrQas
                    .Include(n => n.Item)
                    .Include(n => n.Supplier)
                    .Include(n => n.Defect)
                    .Include(n => n.ItemDefectPhotos)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrQaId == NcrQaId);

                // Check that we got the function or exit with a not found error
                if (ncrQaToUpdate == null)
                {
                    return NotFound();
                }
                else
                {
                    await AddPictures(ncrQaDTO, Photos);
                    try
                    {
                        ncrQaToUpdate.NcrQaItemMarNonConforming = ncrQaDTO.NcrQaItemMarNonConforming;
                        ncrQaToUpdate.NcrQaProcessApplicable = ncrQaDTO.NcrQaProcessApplicable;
                        ncrQaToUpdate.NcrQacreationDate = ncrQaDTO.NcrQacreationDate; 
                        ncrQaToUpdate.NcrQaOrderNumber = ncrQaDTO.NcrQaOrderNumber;
                        ncrQaToUpdate.NcrQaSalesOrder = ncrQaDTO.NcrQaSalesOrder;
                        ncrQaToUpdate.NcrQaQuanReceived = ncrQaDTO.NcrQaQuanReceived;
                        ncrQaToUpdate.NcrQaQuanDefective = ncrQaDTO.NcrQaQuanDefective;
                        ncrQaToUpdate.NcrQaDescriptionOfDefect = ncrQaDTO.NcrQaDescriptionOfDefect;
                        ncrQaToUpdate.NcrQaDefectVideo = ncrQaDTO.NcrQaDefectVideo;                        
                        ncrQaToUpdate.ItemId = ncrQaDTO.ItemId;
                        ncrQaToUpdate.Item = null;
                        ncrQaToUpdate.DefectId = ncrQaDTO.DefectId;
                        ncrQaToUpdate.Defect = null;
                        ncrQaToUpdate.SupplierId = ncrQaDTO.SupplierId;
                        ncrQaToUpdate.Supplier = null;
                        ncrQaToUpdate.NcrQaEngDispositionRequired = ncrQaDTO.NcrQaEngDispositionRequired;
                        ncrQaToUpdate.ItemDefectPhotos = ncrQaDTO.ItemDefectPhotos;
                        ncrQaToUpdate.NcrQaStatusFlag = ncrQaDTO.NcrQaStatusFlag;

                        _context.NcrQas.Update(ncrQaToUpdate);
                        await _context.SaveChangesAsync();

                        int updateNcrQa = ncrQaToUpdate.NcrQaId;
                        if (isDraft)
                        {
                            TempData["SuccessMessage"] = "NCR " + ncrQaDTO.NcrNumber + " was edited as draft successfully!";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "NCR " + ncrQaDTO.NcrNumber + " edited successfully!";

                            //include supplier name in the email
                            var ncrQa = await _context.NcrQas
                                .Include(n => n.Ncr)
                                .Include(n => n.Supplier)
                                .FirstOrDefaultAsync(n => n.NcrQaId == NcrQaId);

                            // Send notification email to Eng or Ops
                            var subject = "NCR Edited " + ncrToUpdate.NcrNumber + "  from Quality";
                            var emailContent = "A NCR has been edited :<br><br>Ncr #: " + ncrToUpdate.NcrNumber + "<br>Supplier: " + ncrToUpdate.NcrQa.Supplier.SupplierName;
                            await NotificationEdit(NcrQaId, subject, emailContent);
                        }       
                        
                        return RedirectToAction("Details", new { id = updateNcrQa, referrer = "Edit" });
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
            }
            PopulateDropDownLists();return View(ncrQaDTO);            
        }
                
        public string GetNcrNumber()
        {
            string lastNcrNumber = _context.Ncrs
                .OrderByDescending(n => n.NcrNumber)
                .Select(n => n.NcrNumber)
                .FirstOrDefault();

            if(lastNcrNumber != null)
            {
                string lastYear = lastNcrNumber.Substring(0, 4);
                string lastConsecutiveNumber = lastNcrNumber.Substring(5);

                if(lastYear == DateTime.Today.Year.ToString())
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

        private SelectList SupplierSelectList(int? selectedId)
        {
            return new SelectList(_context.Suppliers
                .Where(s => s.SupplierStatus == true && s.SupplierName != "NO SUPPLIER PROVIDED")
                .OrderBy(s => s.SupplierName), "SupplierId", "Summary", selectedId);
        }
        
        private SelectList ItemSelectList()
        {
            return new SelectList(_context.Items
                .OrderBy(s => s.ItemName), "ItemId", "Summary");            
        }

        private SelectList DefectSelectList()
        {
            return new SelectList(_context.Defects
                .OrderBy(s => s.DefectName), "DefectId", "DefectName");                       
        }        

        private void PopulateDropDownLists()
        {            
            ViewData["SupplierId"] = SupplierSelectList(null);
            ViewData["ItemId"] = ItemSelectList();
            ViewData["DefectId"] = DefectSelectList();
        }

        [HttpGet]
        public JsonResult GetSuppliers(int? id)
        {
            return Json(SupplierSelectList(id));
        }       

        [HttpGet]
        public JsonResult GetItems()
        {
            return Json(ItemSelectList());
        }

        [HttpGet]
        public JsonResult GetDefects()
        {
            return Json(DefectSelectList());
        }

        public JsonResult GetSuppliersAuto(string term)
        {
            var result = from s in _context.Suppliers
                         where s.SupplierName.ToUpper().Contains(term.ToUpper())
                         orderby s.SupplierName
                         select new { value = s.SupplierName};
            return Json(result);
        }

        private async Task AddPictures(NcrQaDTO ncrQaDTO, List<IFormFile> pictures)
        {
            if (pictures != null && pictures.Any())
            {
                ncrQaDTO.ItemDefectPhotos = new List<ItemDefectPhoto>();

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

                            ncrQaDTO.ItemDefectPhotos.Add(new ItemDefectPhoto
                            {
                                ItemDefectPhotoContent = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                ItemDefectPhotoMimeType = "image/webp",
                                FileName = picture.FileName                                
                            });
                        }
                    }
                }
            }
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.ItemDefectPhotos
                .Include(d => d.FileContent)
                .Where(f => f.ItemDefectPhotoId == id)
                .FirstOrDefaultAsync();
            return File(theFile.ItemDefectPhotoContent, theFile.ItemDefectPhotoMimeType, theFile.FileName);
        }

        #region Archive funtionality
        public async Task<IActionResult> ArchiveNcr(int id)
        {
            var ncrToUpdate = await _context.Ncrs                    
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == id);

            if (ncrToUpdate != null) 
            {
                //Update the phase
                ncrToUpdate.NcrPhase = NcrPhase.Archive;

                //saving the values
                _context.Ncrs.Update(ncrToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "NCR Archive successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "NCR not found for archiving.";
                return RedirectToAction("Index");
            }      
        }

        public async Task<IActionResult> RestoreNcr(int id)
        {
            var ncrToUpdate = await _context.Ncrs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == id);

            if (ncrToUpdate != null)
            {
                //Update the phase
                ncrToUpdate.NcrPhase = NcrPhase.Closed;

                //saving the values
                _context.Ncrs.Update(ncrToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "NCR Restore successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "NCR not found for archiving.";
                return RedirectToAction("Index");
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.ItemDefectPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.ItemDefectPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Photo deleted successfully." });
            }
            return Json(new { success = false, message = "Photo not found." });
        }

        private bool NcrQaExists(int id)
        {
          return _context.NcrQas.Any(e => e.NcrQaId == id);
        }

        #region Email Notifications

        //// Create - Email Notification
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
                var engUsers = await _userManager.GetUsersInRoleAsync("Engineer");
                var emailAddresses = engUsers.Select(u => new EmailAddress
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
                    ViewData["Message"] = "Message NOT sent! No users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to users. Error: {errMsg}";
            }
            return View();
        }

        //// Edit - Email Notification
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
                var engineeringUsers = await _userManager.GetUsersInRoleAsync("Engineer");
                var operationsUsers = await _userManager.GetUsersInRoleAsync("Operations");
                var procurementUsers = await _userManager.GetUsersInRoleAsync("Procurement");

                var allUsers = procurementUsers
                    .Concat(engineeringUsers)
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
                    ViewData["Message"] = "Message NOT sent! No users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to users. Error: {errMsg}";
            }
            return View();
        }

        #endregion
        public IActionResult ExportToExcel()
        {
            var ncrOperation = _context.NcrQas
                .Include(n => n.Supplier)
                .Include(n => n.Defect)
                .Include(n => n.Ncr)
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
                worksheet.Cells[2, 3].Value = "Defect";
                worksheet.Cells[2, 4].Value = "PO Number";
                worksheet.Cells[2, 5].Value = "Phase";
                worksheet.Cells[2, 6].Value = "Created";
                worksheet.Cells[2, 7].Value = "Last Update";

                // Apply center alignment to the header cells
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the header with col span over all columns
                worksheet.Cells[1, 1, 1, 7].Merge = true;  // Merge 6 columns starting from A1
                worksheet.Cells[1, 1].Value = "NCR Quality Log";

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
                    worksheet.Cells[row, 2].Value = item.Supplier.SupplierName;
                    worksheet.Cells[row, 3].Value = item.Defect.DefectName;
                    worksheet.Cells[row, 4].Value = item.NcrQaOrderNumber;
                    worksheet.Cells[row, 5].Value = item.Ncr.NcrPhase.ToString();
                    worksheet.Cells[row, 6].Value = item.NcrQacreationDate.ToString();
                    worksheet.Cells[row, 7].Value = item.Ncr.NcrLastUpdated.ToString();
                    row++;
                }
                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set content type and filename for the Excel file
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "NCR_Quality_Data.xlsx";

                // Return the Excel file as a file download
                return File(content, contentType, fileName);
            }
        }
    }
}
