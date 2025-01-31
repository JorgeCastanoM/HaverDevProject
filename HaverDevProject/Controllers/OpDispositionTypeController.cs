using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Data;
using HaverDevProject.Models;
using HaverDevProject.CustomControllers;
using Microsoft.AspNetCore.Authorization;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class OpDispositionTypeController : LookupsController
    {
        private readonly HaverNiagaraContext _context;

        public OpDispositionTypeController(HaverNiagaraContext context)
        {
            _context = context;
        }

        // GET: OpDispositionType
        public IActionResult Index()
        {
            return Redirect(ViewData["returnURL"].ToString());
        }

        // GET: OpDispositionType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OpDispositionType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OpDispositionTypeName")] OpDispositionType opDispositionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(opDispositionType);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(opDispositionType);
        }

        // GET: OpDispositionType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OpDispositionTypes == null)
            {
                return NotFound();
            }

            var opDispositionType = await _context.OpDispositionTypes.FindAsync(id);
            if (opDispositionType == null)
            {
                return NotFound();
            }
            return View(opDispositionType);
        }

        // POST: OpDispositionType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var opDispositionTypeToUpdate = await _context.OpDispositionTypes.FirstOrDefaultAsync(dt => dt.OpDispositionTypeId == id);
            if (opDispositionTypeToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<OpDispositionType>(opDispositionTypeToUpdate, "",
                    dt => dt.OpDispositionTypeName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpDispositionTypeExists(opDispositionTypeToUpdate.OpDispositionTypeId))
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
            return View(opDispositionTypeToUpdate);
        }

        // GET: OpDispositionType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OpDispositionTypes == null)
            {
                return NotFound();
            }

            var opDispositionType = await _context.OpDispositionTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(dt => dt.OpDispositionTypeId == id);
            if (opDispositionType == null)
            {
                return NotFound();
            }

            return View(opDispositionType);
        }

        // POST: OpDispositionType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OpDispositionTypes == null)
            {
                return Problem("There are no Disposition Types to delete.");
            }
            var opDispositionType = await _context.OpDispositionTypes.FindAsync(id);
            try 
            {
                if (opDispositionType != null)
                {
                    _context.OpDispositionTypes.Remove(opDispositionType);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Disposition Type. Remember, you cannot delete a Disposition Type that is used in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(opDispositionType);
        }

        private bool OpDispositionTypeExists(int id)
        {
          return _context.OpDispositionTypes.Any(dt => dt.OpDispositionTypeId == id);
        }
    }
}
