using HaverDevProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaverDevProject.Views.Shared.Components
{
    public class UserDetailsViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserDetailsViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userEmail, string section)
        {
            if (userEmail == "Seed Data")
            {
                userEmail = GetEmailPerSection(section);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            return View(user); 
        }

        private string GetEmailPerSection(string section)
        {
            return section switch
            {
                "qa" => "cspirleanu@hotmail.com",
                "eng" => "cspirleanu@gmail.com",
                "op" => "ispirleanu1@ncstudents.niagaracollege.ca",
                "proc" => "catalinmctest@gmail.com",                
                "reinsp" => "cspirleanu@hotmail.com",
                _ => "admin@outlook.com" 
            };
        }
    }
}
