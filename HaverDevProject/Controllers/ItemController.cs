
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
    [Authorize(Roles = "Admin, Quality")]
    [ActiveUserOnly]
    public class ItemController : ElephantController
    {
        private readonly HaverNiagaraContext _context;

        public ItemController(HaverNiagaraContext context)
        {
            _context = context;
        }

        // GET: Item
        public async Task<IActionResult> Index(string SearchItem, string SearchCode,  int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Code")
        {

            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;

            //List of sort options.
            string[] sortOptions = new[] { "Code", "Item", "Description", "Supplier", "Defect" };

           var items = _context.Items
                .AsNoTracking();

            //Filterig values                       
            if (!String.IsNullOrEmpty(SearchItem))
            {
                items = items.Where(i => i.ItemName.ToUpper().Contains(SearchItem.ToUpper()));
                numberFilters++;
            }

            if (!String.IsNullOrEmpty(SearchCode))
            {
                items = items.Where(i => i.ItemNumber.ToString().ToUpper().Contains(SearchCode.ToUpper()));
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
            if (sortField == "Code")
            {
                if (sortDirection == "asc")
                {
                    items = items
                        .OrderBy(i => i.ItemNumber);
                    ViewData["filterApplied:ItemNumber"] = "<i class='bi bi-sort-up'></i>";

                }
                else
                {
                    items = items
                        .OrderByDescending(i => i.ItemNumber);
                    ViewData["filterApplied:ItemNumber"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else //if (sortField == "Item")
            {
                if (sortDirection == "asc")
                {
                    items = items
                        .OrderBy(i => i.ItemName);
                    ViewData["filterApplied:ItemName"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    items = items
                        .OrderByDescending(i => i.ItemName);
                    ViewData["filterApplied:ItemName"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        // GET: Item/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Item/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemNumber,ItemName")] Item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item created successfully!";
                    int newItemId = item.ItemId;
                    return RedirectToAction("Details", new { id = newItemId });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    ModelState.AddModelError("ItemNumber", "Unable to save changes. Remember, you cannot have duplicate SAP Number.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            //send the Validation Errors directly to the client
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
            return View(item);
        }

        // GET: Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }
            var item = await _context.Items
                .FirstOrDefaultAsync(d => d.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var itemToUpdate = await _context.Items

                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }
            if (await TryUpdateModelAsync<Item>(itemToUpdate, "",
                    i => i.ItemNumber, i => i.ItemName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item updated successfully!";
                    int updateItemId = itemToUpdate.ItemId;
                    return RedirectToAction("Details", new { id = updateItemId });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE"))
                    {
                        ModelState.AddModelError("ItemNumber", "Unable to save changes. Remember, you cannot have duplicate SAP Number.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }

            }
            return View(itemToUpdate);
        }

        // GET: Item/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'HaverNiagaraContext.Items' is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            var itemDictionary = new Dictionary<int, string>();
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
                        var itemNumberStr = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        var itemName = worksheet.Cells[row, 2].Value?.ToString().Trim();

                        if (string.IsNullOrEmpty(itemNumberStr))
                        {
                            errorMessages.Add($"Row {row}: Item SAP Number is required.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(itemName))
                        {
                            errorMessages.Add($"Row {row}: Item Name is required.");
                            continue;
                        }

                        if (!int.TryParse(itemNumberStr, out int itemNumber))
                        {
                            errorMessages.Add($"Row {row}: Item SAP Number must be a number.");
                            continue;
                        }

                        if (itemDictionary.ContainsKey(itemNumber))
                        {
                            errorMessages.Add($"Row {row}: Duplicate Item SAP Number {itemNumber} found in the Excel file.");
                            continue;
                        }

                        itemDictionary.Add(itemNumber, itemName);
                    }
                }
            }

            // Handling error feedback based on the count of errors
            if (errorMessages.Count > 10)
            {
                TempData["ErrorMessage"] = $"There are errors in {errorMessages.Count} rows. Please review and fix the errors before uploading again.";
            }
            else if (errorMessages.Any())
            {
                TempData["ErrorMessage"] = $"Errors found: <br><li>{string.Join("<li>", errorMessages)}";
            }
            else
            {
                // Process items if there are no errors
                foreach (var (itemNumber, itemName) in itemDictionary)
                {
                    var existingItem = await _context.Items
                        .FirstOrDefaultAsync(i => i.ItemNumber == itemNumber);

                    if (existingItem != null)
                    {
                        if (existingItem.ItemName != itemName)
                        {
                            existingItem.ItemName = itemName;
                            _context.Items.Update(existingItem);
                            successCount++;
                        }
                    }
                    else
                    {
                        var newItem = new Item
                        {
                            ItemNumber = itemNumber,
                            ItemName = itemName
                        };
                        _context.Items.Add(newItem);
                        successCount++;
                    }
                }

                await _context.SaveChangesAsync();
            }

            if (successCount > 0 && errorMessages.Count <= 10)
            {
                TempData["SuccessMessage"] = $"File processed successfully. {successCount} items have been added or updated.";
            }
            else if (!errorMessages.Any())
            {
                TempData["ErrorMessage"] = "No items were added or updated. Please check the file for errors or duplicates.";
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Upload()
        {
            return View("UploadExcel");
        }        
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
