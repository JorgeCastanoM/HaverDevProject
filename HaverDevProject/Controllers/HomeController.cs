using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using HaverDevProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace HaverDevProject.Controllers
{
    [ActiveUserOnly]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            //for getting the user full name and role in the dashboard
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;

                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.Role = roles.FirstOrDefault();
            }
            else
            {
                ViewBag.FirstName = "Guest";
                ViewBag.LastName = "";
                ViewBag.Role = "None";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult KpiDashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
