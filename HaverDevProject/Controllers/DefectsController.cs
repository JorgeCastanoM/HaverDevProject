using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.CustomControllers;
using HaverDevProject.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;


namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class DefectsController : ElephantController
    {
        private readonly HaverNiagaraContext _context;
        public DefectsController(HaverNiagaraContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]

        // GET: Defects
        public async Task<IActionResult> Index(string SearchName, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Defect")
        {

            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //List of sort options.
            string[] sortOptions = new[] { "Defect", "Description", "Item" };

            var defects = _context.Defects
                .AsNoTracking();

            //Filterig values                       
            if (!String.IsNullOrEmpty(SearchName))
            {
                defects = defects.Where(d => d.DefectName.ToUpper().Contains(SearchName.ToUpper()));
                numberFilters++;
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
            if (sortField == "Defect")
            {
                if (sortDirection == "asc")
                {
                    defects = defects
                        .OrderBy(d => d.DefectName);
                    ViewData["filterApplied:DefectName"] = "<i class='bi bi-sort-up'></i>";

                }
                else
                {
                    defects = defects
                        .OrderByDescending(d => d.DefectName);
                    ViewData["filterApplied:DefectName"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Defect>.CreateAsync(defects.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Defects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Defects == null)
            {
                return NotFound();
            }

            var defect = await _context.Defects
                .FirstOrDefaultAsync(m => m.DefectId == id);
            if (defect == null)
            {
                return NotFound();
            }

            return View(defect);
        }

        // GET: Defects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Defects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DefectId,DefectName")] Defect defect)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(defect);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Defect created successfully!";
                    int newDefectId = defect.DefectId;
                    return RedirectToAction("Details", new { id = newDefectId });
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

            ////Send the Validation Errors directly to the client
            if (!ModelState.IsValid && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //Was an AJAX request so build a message with all validation errors
                string errorMessage = "";
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errorMessage += error.ErrorMessage + "|";
                    }
                }
                //Note: returning a BadRequest results in HTTP Status code 400
                return BadRequest(errorMessage);
            }

            return View(defect);
        }

        // GET: Defects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Defects == null)
            {
                return NotFound();
            }

            var defect = await _context.Defects
               .FirstOrDefaultAsync(d => d.DefectId == id);

            if (defect == null)
            {
                return NotFound();
            }

            return View(defect);
        }

        // POST: Defects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            var defectToUpdate = await _context.Defects
               .FirstOrDefaultAsync(d => d.DefectId == id);

            if (defectToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Defect>(defectToUpdate, "",
                d => d.DefectName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Defect updated successfully!";
                    int updateDefectId = defectToUpdate.DefectId;
                    return RedirectToAction("Details", new { id = updateDefectId });
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
            return View(defectToUpdate);
        }

        // GET: Defects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Defects == null)
            {
                return NotFound();
            }

            var defect = await _context.Defects
                .FirstOrDefaultAsync(m => m.DefectId == id);
            if (defect == null)
            {
                return NotFound();
            }

            return View(defect);
        }

        // POST: Defects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Defects == null)
            {
                return Problem("Entity set 'HaverNiagaraContext.Defects' is null.");
            }
            var defect = await _context.Defects.FindAsync(id);
            if (defect != null)
            {
                _context.Defects.Remove(defect);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Excel Upload 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                TempData["ErrorMessage"] = "Invalid file type. Please upload an Excel file (.xlsx).";
                return RedirectToAction(nameof(Index));
            }

            var errorMessages = new List<string>();
            var defectsFromExcel = new HashSet<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var defectName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        if (string.IsNullOrWhiteSpace(defectName))
                        {
                            errorMessages.Add($"Row {row}: Defect Type Name is required.");
                            continue;
                        }
                        defectsFromExcel.Add(defectName);
                    }

                    if (errorMessages.Any())
                    {
                        TempData["ErrorMessage"] = $"Please fix the following errors and try again: {string.Join(" ", errorMessages)}";
                        return RedirectToAction(nameof(Index));
                    }

                    var existingDefectNames = await _context.Defects
                                            .Where(d => defectsFromExcel.Contains(d.DefectName))
                                            .Select(d => d.DefectName)
                                            .ToListAsync();
                    var newDefectNames = defectsFromExcel.Except(existingDefectNames);

                    int addedCount = 0;
                    foreach (var defectName in newDefectNames)
                    {
                        var newDefect = new Defect { DefectName = defectName };
                        _context.Defects.Add(newDefect);
                        addedCount++;
                    }

                    if (addedCount > 0)
                    {
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"File uploaded successfully. {addedCount} new defect(s) were added.";
                    }
                    else if (defectsFromExcel.Count > 0)
                    {
                        TempData["ErrorMessage"] = "No new defects were added because they already exist in the database.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The uploaded file contains no defect names or they are not correctly formatted.";
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Upload()
        {
            return View("UploadExcel");
        }

        private bool DefectExists(int id)
        {
            return _context.Defects.Any(e => e.DefectId == id);
        }
    }
}
