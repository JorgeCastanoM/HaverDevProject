using HaverDevProject.CustomControllers;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin, Quality")]
    [ActiveUserOnly]
    public class SupplierController : ElephantController
    {
        private readonly HaverNiagaraContext _context;

        public SupplierController(HaverNiagaraContext context)
        {
            _context = context;
        }

        // GET: Supplier
        public async Task<IActionResult> Index(
            string SearchSupplier,
            string SearchContact,
            int? page,
            int? pageSizeID,
            string actionButton,
            string sortDirection = "asc",
            string sortField = "Code",
            string filter = "Active"
        )
        {

            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //List of sort options.
            string[] sortOptions = new[] { "Code", "Name", "Email", "Contact" };

            var suppliers = _context.Suppliers.AsNoTracking();

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
                    suppliers = suppliers.Where(s => s.SupplierStatus == true);
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    suppliers = suppliers.Where(s => s.SupplierStatus == false);
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }

            if (!String.IsNullOrEmpty(SearchSupplier))
            {
                suppliers = suppliers.Where(s =>
                     s.SupplierName.ToUpper().Contains(SearchSupplier.ToUpper())
                );
                numberFilters++;
            }

            if (!String.IsNullOrEmpty(SearchContact))
            {
                suppliers = suppliers.Where(s =>
                    s.SupplierContactName.ToUpper().Contains(SearchContact.ToUpper())
                );
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
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Code")
            {
                if (sortDirection == "asc")
                {
                    suppliers = suppliers.OrderBy(p => p.SupplierCode);
                    ViewData["filterApplied:SupplierCode"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(p => p.SupplierCode);
                    ViewData["filterApplied:SupplierCode"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Name")
            {
                if (sortDirection == "asc")
                {
                    suppliers = suppliers.OrderBy(p => p.SupplierName);
                    ViewData["filterApplied:SupplierName"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(p => p.SupplierName);
                    ViewData["filterApplied:SupplierName"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Email") 
            {
                if (sortDirection == "asc")
                {
                    suppliers = suppliers.OrderBy(p => p.SupplierEmail);
                    ViewData["filterApplied:SupplierEmail"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(p => p.SupplierEmail);
                    ViewData["filterApplied:SupplierEmail"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Status")
            {
                if (sortDirection == "asc")
                {
                    suppliers = suppliers.OrderBy(s => s.SupplierStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(s => s.SupplierStatus);
                    ViewData["filterApplied:Status"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //Sorting by Contact
            {
                if (sortDirection == "asc")
                {
                    suppliers = suppliers.OrderBy(s => s.SupplierContactName);
                    ViewData["filterApplied:SupplierContact"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(s => s.SupplierContactName);
                    ViewData["filterApplied:SupplierContact"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Supplier>.CreateAsync(
                suppliers.AsNoTracking(),
                page ?? 1,
                pageSize
            );

            return View(pagedData);
        }

        // GET: Supplier/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.NcrQas).ThenInclude(s => s.Item)
                .Include(s => s.NcrQas).ThenInclude(s => s.Ncr)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            var supplierVM = new SupplierDetailsVM
            {
                Supplier = supplier,
                RelatedNCRs =
                supplier.NcrQas.Select(nqa => nqa.Ncr).ToList()
                    ?? new List<Ncr>()
            };

            return View(supplierVM);
        }

        // GET: Supplier/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("SupplierId,SupplierCode,SupplierName,SupplierContactName,SupplierEmail")]
                Supplier supplier
        )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(supplier);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Supplier created successfully!";
                    int newSupplierId = supplier.SupplierId;
                    return RedirectToAction("Details", new { id = newSupplierId });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator."
                );
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    ModelState.AddModelError(
                        "SupplierCode",
                        "Unable to save changes. Remember, you cannot have duplicate Supplier Code."
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to save changes. Try again, and if the problem persists see your system administrator."
                    );
                }
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

            return View(supplier);
        }

        // GET: Supplier/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Supplier/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "SupplierId,SupplierCode,SupplierName,SupplierContactName,SupplierEmail,SupplierStatus"
            )]
                Supplier supplier
        )
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Supplier updated successfully!";
                    int updateSupplierId = supplier.SupplierId;
                    return RedirectToAction("Details", new { id = updateSupplierId });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator."
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
                    {
                        ModelState.AddModelError(
                            "",
                            "Unable to save changes. The Supplier was deleted by another user."
                        );
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE"))
                    {
                        ModelState.AddModelError(
                            "SupplierCode",
                            "Unable to save changes. Remember, you cannot have duplicate Supplier Code."
                        );
                    }
                    else
                    {
                        ModelState.AddModelError(
                            "",
                            "Unable to save changes. Try again, and if the problem persists see your system administrator."
                        );
                    }
                }
            }
            return View(supplier);
        }

        // GET: Supplier/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("There are no Suppliers to delete");
            }
            var supplier = await _context.Suppliers.FindAsync(id);

            try
            {
                if (supplier != null)
                {
                    _context.Suppliers.Remove(supplier);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to Delete Supplier. Remember, you cannot delete a Supplier that has a NCR in the system."
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to save changes. Try again, and if the problem persists see your system administrator."
                    );
                }
            }
            return View(supplier);
        }

        public async Task<IActionResult> SupplierReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ncrData = await _context
                .Ncrs.Include(n => n.NcrQa)
                .ThenInclude(qa => qa.Item)
                .Include(n => n.NcrQa) 
                .ThenInclude(i => i.Supplier)
                .Include(n => n.NcrQa)
                .ThenInclude(qa => qa.Defect)
                .Include(n => n.NcrEng)
                .ThenInclude(e => e.EngDispositionType)
                .Include(n => n.NcrOperation)
                .ThenInclude(o => o.FollowUpType)
                .FirstOrDefaultAsync(n => n.NcrId == id);

            if (ncrData == null)
            {
                return NotFound();
            }

            NcrSupplierReportDTO reportDto = new NcrSupplierReportDTO
            {
                NcrNumber = ncrData.NcrNumber,
                NcrStatus = ncrData.NcrStatus,
                SupplierName = ncrData.NcrQa?.Supplier?.SupplierName ?? "Not Available",
                NcrQaOrderNumber = ncrData.NcrQa?.NcrQaOrderNumber ?? "Not Available",
                ItemSAP = ncrData.NcrQa?.Item?.ItemNumber ?? 0,
                ItemName = ncrData.NcrQa?.Item?.ItemName ?? "Not Available",
                NcrQaQuanReceived = ncrData.NcrQa?.NcrQaQuanReceived ?? 0,
                NcrQaQuanDefective = ncrData.NcrQa?.NcrQaQuanDefective ?? 0,
                NcrQaDefect = ncrData.NcrQa?.Defect.DefectName ?? "Not Available",
                NcrQaDescriptionOfDefect =
                    ncrData.NcrQa?.NcrQaDescriptionOfDefect ?? "Not Available",
                EngDispositionType =
                    ncrData.NcrEng?.EngDispositionType?.EngDispositionTypeName ?? "Not Available",
                EngDispositionDescription =
                    ncrData.NcrEng?.NcrEngDispositionDescription ?? "Not Available",
                OpDispositionType =
                    ncrData.NcrOperation?.OpDispositionType?.OpDispositionTypeName ?? "Not Available",
                OperationDescription =
                    ncrData.NcrOperation?.NcrPurchasingDescription ?? "Not Available",
            };

            return View("SupplierReport", reportDto);
        }

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
            var supplierDictionary = new Dictionary<int, (string SupplierName, string ContactName, string Email)>();
            int successCount = 0;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var supplierCodeStr = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        var supplierName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        var contactName = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var email = worksheet.Cells[row, 4].Value?.ToString().Trim();

                        if (string.IsNullOrWhiteSpace(supplierCodeStr))
                        {
                            errorMessages.Add($"Row {row}: Supplier Code is required.");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(supplierName))
                        {
                            errorMessages.Add($"Row {row}: Supplier Name is required.");
                            continue;
                        }

                        if (!int.TryParse(supplierCodeStr, out int supplierCode))
                        {
                            errorMessages.Add($"Row {row}: Supplier Code must be a number.");
                            continue;
                        }

                        if (supplierDictionary.ContainsKey(supplierCode))
                        {
                            TempData["ErrorMessage"] = $"Duplicate Supplier Code {supplierCode} found in the Excel file.";
                            return RedirectToAction(nameof(Index));
                        }

                        supplierDictionary[supplierCode] = (supplierName, contactName, email);
                    }
                }
            }

            if (errorMessages.Count > 10)
            {
                TempData["ErrorMessage"] = $"There are errors in {errorMessages.Count} rows. Please review and fix the errors before uploading again.";
                return RedirectToAction(nameof(Index));
            }
            else if (errorMessages.Any())
            {
                TempData["ErrorMessage"] = $"Errors found: <br><li>{string.Join("<li>", errorMessages)}";
                return RedirectToAction(nameof(Index));
            }

            foreach (var (supplierCode, supplierInfo) in supplierDictionary)
            {
                var existingSupplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierCode == supplierCode.ToString());

                if (existingSupplier != null)
                {
                    bool updated = false;
                    if (existingSupplier.SupplierName != supplierInfo.SupplierName)
                    {
                        existingSupplier.SupplierName = supplierInfo.SupplierName;
                        updated = true;
                    }
                    if (existingSupplier.SupplierContactName != supplierInfo.ContactName) // Check if contact name has changed
                    {
                        existingSupplier.SupplierContactName = supplierInfo.ContactName;
                        updated = true;
                    }
                    if (existingSupplier.SupplierEmail != supplierInfo.Email) // Check if email has changed
                    {
                        existingSupplier.SupplierEmail = supplierInfo.Email;
                        updated = true;
                    }

                    if (updated)
                    {
                        _context.Suppliers.Update(existingSupplier);
                        successCount++; 
                    }
                }
                else
                {
                    var newSupplier = new Supplier
                    {
                        SupplierCode = supplierCode.ToString(),
                        SupplierName = supplierInfo.SupplierName,
                        SupplierContactName = !string.IsNullOrWhiteSpace(supplierInfo.ContactName) ? supplierInfo.ContactName : null,
                        SupplierEmail = !string.IsNullOrWhiteSpace(supplierInfo.Email) ? supplierInfo.Email : null,
                    };
                    _context.Suppliers.Add(newSupplier);
                    successCount++; 
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"File processed successfully. {successCount} suppliers have been added or updated.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Upload()
        {
            return View("UploadExcel");
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
