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
using Microsoft.AspNetCore.Authorization;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class EngDispositionTypeController : LookupsController
    {
        private readonly HaverNiagaraContext _context;

        public EngDispositionTypeController(HaverNiagaraContext context)
        {
            _context = context;
        }

        // GET: EngDispositionTypes
        public IActionResult Index()
        {
            return Redirect(ViewData["returnURL"].ToString());
        }

        // GET: EngDispositionTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EngDispositionTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EngDispositionTypeName")] EngDispositionType engDispositionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(engDispositionType);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException) 
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(engDispositionType);
        }

        // GET: EngDispositionTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EngDispositionTypes == null)
            {
                return NotFound();
            }

            var engDispositionType = await _context.EngDispositionTypes.FindAsync(id);
            if (engDispositionType == null)
            {
                return NotFound();
            }
            return View(engDispositionType);
        }

        // POST: EngDispositionTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var engDispositionTypeToUpdate = await _context.EngDispositionTypes.FirstOrDefaultAsync(dt => dt.EngDispositionTypeId == id);

            if (engDispositionTypeToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<EngDispositionType>(engDispositionTypeToUpdate, "",
                    dt => dt.EngDispositionTypeName))
            {
                try
                {                    
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EngDispositionTypeExists(engDispositionTypeToUpdate.EngDispositionTypeId))
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
            return View(engDispositionTypeToUpdate);
        }

        // GET: EngDispositionTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EngDispositionTypes == null)
            {
                return NotFound();
            }

            var engDispositionType = await _context.EngDispositionTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.EngDispositionTypeId == id);
            if (engDispositionType == null)
            {
                return NotFound();
            }

            return View(engDispositionType);
        }

        // POST: EngDispositionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EngDispositionTypes == null)
            {
                return Problem("There are no Disposition Types to delete.");
            }
            var engDispositionType = await _context.EngDispositionTypes.FindAsync(id);
            try
            {
                if (engDispositionType != null)
                {
                    _context.EngDispositionTypes.Remove(engDispositionType);
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
            return View(engDispositionType);
        }

        private bool EngDispositionTypeExists(int id)
        {
          return _context.EngDispositionTypes.Any(e => e.EngDispositionTypeId == id);
        }
    }
}
