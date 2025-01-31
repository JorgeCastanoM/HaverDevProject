using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.Utilities;
using HaverDevProject.CustomControllers;
using HaverDevProject.Services;
using Microsoft.AspNetCore.Authorization;
using Spire.Xls;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using HaverDevProject.Configurations;


namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class NcrController : ElephantController
    {
        private readonly HaverNiagaraContext _context;
        private readonly INumYearsService _numOfYearsService;

        public NcrController(HaverNiagaraContext context, INumYearsService numYearsService)
        {
            _context = context;
            _numOfYearsService = numYearsService;
        }

        // GET: Ncr
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
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "Defect", "PO Number", "Phase" };

            var ncr = _context.Ncrs
                //.Include(n => n.Item).ThenInclude(n => n.ItemDefects).ThenInclude(n => n.Defect)
                .Include(n => n.NcrQa.Supplier)
                .Include(n => n.NcrQa.Defect)
                .AsNoTracking();

            //foreach (var ncrItem in ncr)
            //{
            //    if (ncrItem.NcrQa.NcrQacreationDate.AddYears(5) <= DateTime.UtcNow)
            //    {
            //        // Call the ArchiveNcr method for this specific item
            //        await ArchiveDateNcr(ncrItem.NcrId);
            //    }
            //}

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
                    ncr = ncr.Where(n => n.NcrStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    ncr = ncr.Where(n => n.NcrStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncr = ncr.Where(s => s.NcrQa.Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) //(s => s.Item.ItemDefects.FirstOrDefault().Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) 
                || s.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                ncr = ncr.Where(n => n.NcrQa.Supplier.SupplierName == SearchSupplier);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate >= StartDate &&
                         n.NcrQa.NcrQacreationDate <= EndDate);
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
                    ncr = ncr
                        .OrderBy(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Defect")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrPhase); //.OrderBy(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrPhase); //.OrderByDescending(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "PO Number")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Ncr>.CreateAsync(ncr.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Ncr/Details/5
        public async Task<IActionResult> Details(int? id, NcrPhase section)
        {
            if (id == null || _context.Ncrs == null)
            {
                return NotFound();
            }

            var ncr = await _context.Ncrs
                .Include(n => n.NcrQa)
                .Include(n => n.NcrQa).ThenInclude(n => n.Item)
                .Include(n => n.NcrQa).ThenInclude(n => n.Defect)
                .Include(n => n.NcrQa).ThenInclude(n => n.Supplier)
                .Include(n => n.NcrQa).ThenInclude(n => n.ItemDefectPhotos)
                .Include(n => n.NcrEng)
                .Include(n => n.NcrEng).ThenInclude(n => n.EngDispositionType)
                .Include(n => n.NcrEng).ThenInclude(n => n.Drawing)
                .Include(n => n.NcrEng).ThenInclude(n => n.EngDefectPhotos)
                .Include(n => n.NcrOperation)
                .Include(n => n.NcrOperation).ThenInclude(n => n.OpDispositionType)
                .Include(n => n.NcrOperation).ThenInclude(n => n.FollowUpType)
                .Include(n => n.NcrOperation).ThenInclude(n => n.OpDefectPhotos)
                .Include(n => n.NcrProcurement)
                .Include(n => n.NcrProcurement).ThenInclude(n => n.ProcDefectPhotos)
                .Include(n => n.NcrReInspect)
                .Include(n => n.NcrReInspect).ThenInclude(n => n.NcrReInspectPhotos)
                .FirstOrDefaultAsync(m => m.NcrId == id);

            if (ncr == null)
            {
                return NotFound();
            }

            ViewBag.IsNCRQaView = true;
            ViewBag.IsNCREngView = false;
            ViewBag.IsNCROpView = false;
            ViewBag.IsNCRProcView = false;
            ViewBag.IsNCRReInspView = false;

            ViewBag.NCRSectionId = id;

            return View(ncr);
        }
        // GET: Ncr/Create
        public IActionResult Create()
        {
            //string example = "2024-0018";
            //ViewData["NCRNumber"] = example;
            Ncr ncr = new Ncr();
            return View(ncr);
        }

        // POST: Ncr/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NcrId,NcrNumber,NcrLastUpdated,NcrStatus")] Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ncr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ncr);
        }

        // GET: Ncr/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ncrs == null)
            {
                return NotFound();
            }

            var ncr = await _context.Ncrs.FindAsync(id);
            if (ncr == null)
            {
                return NotFound();
            }
            return View(ncr);
        }

        // POST: Ncr/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NcrId,NcrNumber,NcrLastUpdated,NcrStatus")] Ncr ncr)
        {
            if (id != ncr.NcrId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ncr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NcrExists(ncr.NcrId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ncr);
        }

        //ARCHIVE SECTION
        // GET: Archived Ncrs
        public async Task<IActionResult> Archived(string SearchCode, string SearchSupplier, DateTime StartDate, DateTime EndDate,
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
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "Defect", "PO Number", "Phase" };

            var ncr = _context.Ncrs
                //.Include(n => n.Item).ThenInclude(n => n.ItemDefects).ThenInclude(n => n.Defect)
                .Include(n => n.NcrQa.Supplier)
                .Include(n => n.NcrQa.Defect)
                .Where(ncr => ncr.NcrPhase == NcrPhase.Archive)
                .AsNoTracking();

            //Filterig values            
            //if (!String.IsNullOrEmpty(filter))
            //{
            //    if (filter == "Archived")
            //    {
            //        // Filter only archived records
            //        ncr = ncr.Where(ncr => ncr.NcrPhase == NcrPhase.Archive);
            //    }
            //}
            //    else if (filter == "Active")
            //    {
            //        ncr = ncr.Where(n => n.NcrStatus == true);
            //        ViewData["filterApplied:ButtonActive"] = "btn-success";
            //        ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
            //        ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
            //    }
            //    else //(filter == "Closed")
            //    {
            //        ncr = ncr.Where(n => n.NcrStatus == false);
            //        ViewData["filterApplied:ButtonClosed"] = "btn-danger";
            //        ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
            //        ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
            //    }
            //}
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncr = ncr.Where(s => s.NcrQa.Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) //(s => s.Item.ItemDefects.FirstOrDefault().Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) 
                || s.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                ncr = ncr.Where(n => n.NcrQa.Supplier.SupplierName == SearchSupplier);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate >= StartDate &&
                         n.NcrQa.NcrQacreationDate <= EndDate);
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
                    ncr = ncr
                        .OrderBy(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Defect")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrPhase); //.OrderBy(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrPhase); //.OrderByDescending(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "PO Number")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Ncr>.CreateAsync(ncr.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // Manually Archiving many NCRs
        [HttpPost]
        public async Task<IActionResult> ArchiveManyNcrs(int archiveYear, [FromServices] NcrArchivingService ncrArchivingService)
        {
            try
            {
                // Call the ArchiveNcrsByYear method from the injected NcrArchivingService
                var archivedCount = await ncrArchivingService.ArchiveNcrsByYear(archiveYear);

                // Set success message in TempData
                TempData["SuccessMessage"] = $"{archivedCount} NCRs Archived";

                // Redirect back to the previous page or any other desired page
                return RedirectToAction("Archived"); // Change "Archived" to the action name you want to redirect to
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during archiving
                TempData["ErrorMessage"] = $"Error occurred while Archiving NCR objects: {ex.Message}";

                // Redirect back to the previous page or any other desired page
                return RedirectToAction("Archived"); // Change "Archived" to the action name you want to redirect to
            }
        }

        [HttpPost]
        public IActionResult AutomaticArchiveYear(int numYears)
        {
            _numOfYearsService.NumOfYears = numYears;
            TempData["SuccessMessage"] = $"Archiving Service set to: {numYears} Years";
            return RedirectToAction("Archived"); // Redirect to a different action

        }

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

                TempData["SuccessMessage"] = "NCR Archived successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "NCR not found for Archiving.";
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

                TempData["SuccessMessage"] = "NCR Restored successfully!";
                return RedirectToAction("Archived");
            }
            else
            {
                TempData["ErrorMessage"] = "NCR not found for Archiving.";
                return RedirectToAction("Archived");
            }

        }


        //VOID SECTION
        // GET: Void Ncrs
        public async Task<IActionResult> Void(string SearchCode, string SearchSupplier, DateTime StartDate, DateTime EndDate,
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
            string[] sortOptions = new[] { "Created", "NCR #", "Supplier", "Defect", "PO Number", "Phase" };

            var ncr = _context.Ncrs
                //.Include(n => n.Item).ThenInclude(n => n.ItemDefects).ThenInclude(n => n.Defect)
                .Include(n => n.NcrQa.Supplier)
                .Include(n => n.NcrQa.Defect)
                .Where(ncr => ncr.NcrPhase == NcrPhase.Void)
                .AsNoTracking();

            //Filterig values            
            //if (!String.IsNullOrEmpty(filter))
            //{
            //    if (filter == "Archived")
            //    {
            //        // Filter only archived records
            //        ncr = ncr.Where(ncr => ncr.NcrPhase == NcrPhase.Archive);
            //    }
            //}
            //    else if (filter == "Active")
            //    {
            //        ncr = ncr.Where(n => n.NcrStatus == true);
            //        ViewData["filterApplied:ButtonActive"] = "btn-success";
            //        ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
            //        ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
            //    }
            //    else //(filter == "Closed")
            //    {
            //        ncr = ncr.Where(n => n.NcrStatus == false);
            //        ViewData["filterApplied:ButtonClosed"] = "btn-danger";
            //        ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
            //        ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
            //    }
            //}
            if (!String.IsNullOrEmpty(SearchCode))
            {
                ncr = ncr.Where(s => s.NcrQa.Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) //(s => s.Item.ItemDefects.FirstOrDefault().Defect.DefectName.ToUpper().Contains(SearchCode.ToUpper()) 
                || s.NcrNumber.ToUpper().Contains(SearchCode.ToUpper()));
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                ncr = ncr.Where(n => n.NcrQa.Supplier.SupplierName == SearchSupplier);
                numberFilters++;
            }
            if (StartDate == EndDate)
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate == StartDate);
                numberFilters++;
            }
            else
            {
                ncr = ncr.Where(n => n.NcrQa.NcrQacreationDate >= StartDate &&
                         n.NcrQa.NcrQacreationDate <= EndDate);
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
                    ncr = ncr
                        .OrderBy(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrNumber);
                    ViewData["filterApplied:NcrNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Defect")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Defect.DefectName);
                    ViewData["filterApplied:Defect"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Supplier")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.Supplier.SupplierName);
                    ViewData["filterApplied:Supplier"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Created")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQacreationDate);

                    ViewData["filterApplied:Created"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Phase")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrPhase); //.OrderBy(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrPhase); //.OrderByDescending(p => p.Ncr.NcrStatus);
                    ViewData["filterApplied:Phase"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Last Updated")
            {
                if (sortDirection == "desc") //desc by default
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrLastUpdated);
                    ViewData["filterApplied:Last Updated"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //(sortField == "PO Number")
            {
                if (sortDirection == "asc")
                {
                    ncr = ncr
                        .OrderBy(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    ncr = ncr
                        .OrderByDescending(p => p.NcrQa.NcrQaOrderNumber);
                    ViewData["filterApplied:PONumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Ncr>.CreateAsync(ncr.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }
        public async Task<IActionResult> VoidNcr(int id, string voidReason)
        {
            var ncrToUpdate = await _context.Ncrs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NcrId == id);

            if (ncrToUpdate != null)
            {
                // Update the phase and void reason
                ncrToUpdate.NcrStatus = false;
                ncrToUpdate.NcrPhase = NcrPhase.Void;
                ncrToUpdate.NcrVoidReason = voidReason;

                // Saving the values
                _context.Ncrs.Update(ncrToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "NCR Voided successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "NCR not found for Voiding.";
                return RedirectToAction("Index");
            }
        }


        //DROPDOWN LIST SECTION
        [HttpGet]
        public JsonResult GetSuppliers(int? id)
        {
            return Json(SupplierSelectList(id));
        }

        public JsonResult GetSuppliersAuto(string term)
        {
            var result = from s in _context.Suppliers
                         where s.SupplierName.ToUpper().Contains(term.ToUpper())
                         //|| d.FirstName.ToUpper().Contains(term.ToUpper())
                         orderby s.SupplierName
                         select new { value = s.SupplierName };
            return Json(result);
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
      

        private bool NcrExists(int id)
        {
            return _context.Ncrs.Any(e => e.NcrId == id);
        }
    }
}
