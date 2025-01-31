using Microsoft.AspNetCore.Mvc;
using HaverDevProject.CustomControllers;
using HaverDevProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class LookupController : CognizantController
    {
        private readonly HaverNiagaraContext _context;
        public LookupController(HaverNiagaraContext context)
        {
            _context = context;
        }
        public IActionResult Index(string Tab = "EngDispositionType-Tab")
        {
            ViewData["Tab"] = Tab;
            return View();
        }
        public PartialViewResult EngDispositionType()
        {
            ViewData["engDispositionTypeId"] = new
                SelectList(_context.EngDispositionTypes
                .OrderBy(a => a.EngDispositionTypeName), "EngDispositionTypeId", "EngDispositionTypeName");
            return PartialView("_EngDispositionType");
        }
        public PartialViewResult FollowUpType()
        {
            ViewData["FollowUpTypeId"] = new
                SelectList(_context.FollowUpTypes
                .OrderBy(a => a.FollowUpTypeName), "FollowUpTypeId", "FollowUpTypeName");
            return PartialView("_FollowUpType");
        }

        public PartialViewResult OpDispositionType()
        {
            ViewData["OpDispositionTypeId"] = new
                SelectList(_context.OpDispositionTypes
                .OrderBy(a => a.OpDispositionTypeName), "OpDispositionTypeId", "OpDispositionTypeName");
            return PartialView("_OpDispositionType");
        }
    }
}
