using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HaverDevProject.Models;

namespace HaverDevProject.Components
{
    public class UserNavigationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserNavigationViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user); 
        }
    }

}
