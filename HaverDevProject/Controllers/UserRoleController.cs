using HaverDevProject.CustomControllers;
using HaverDevProject.Data;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaverDevProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using HaverDevProject.Utilities;
using Elfie.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace HaverDevProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [ActiveUserOnly]
    public class UserRoleController : CognizantController
    {
        private readonly IMyEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LinkGenerator _linkGenerator;
        public UserRoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMyEmailSender emailSender, LinkGenerator linkGenerator)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _linkGenerator = linkGenerator;
        }
        // GET: User
        public async Task<IActionResult> Index(
            string SearchUser, 
            string SearchRole,
            int? page, 
            int? pageSizeID,
            string actionButton, 
            string sortDirection = "asc", 
            string sortField = "FirstName", 
            string filter = "Active")
        {
            ViewData["Filtering"] = "btn-block invisible";
            int numberFilters = 0;
            
            string[] sortOptions = new[] { "FirstName", "LastName", "Email", "Role" };

            var users = await (from u in _context.Users
                               .OrderBy(u => u.UserName)
                               select new UserVM
                               {
                                   ID = u.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Status = u.Status,
                                   Email = u.Email
                               }).ToListAsync();
            foreach (var u in users)
            {
                var _user = await _userManager.FindByIdAsync(u.ID);
                var roles = (List<string>)await _userManager.GetRolesAsync(_user);
                u.SelectedRole = roles[0]; 
                //Note: we needed the explicit cast above because GetRolesAsync() returns an IList<string>
            };

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
                    users = users.Where(u => u.Status == true).ToList();
                    ViewData["filterApplied:ButtonActive"] = "btn-success";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger custom-opacity";
                }
                else //(filter == "Closed")
                {
                    users = users.Where(u => u.Status == false).ToList();
                    ViewData["filterApplied:ButtonClosed"] = "btn-danger";
                    ViewData["filterApplied:ButtonAll"] = "btn-primary custom-opacity";
                    ViewData["filterApplied:ButtonActive"] = "btn-success custom-opacity";
                }
            }

            //Filterig values                       
            if (!String.IsNullOrEmpty(SearchUser))
            {
                users = users.Where(u =>
                    u.FirstName.ToUpper().Contains(SearchUser.ToUpper()) ||
                    u.LastName.ToUpper().Contains(SearchUser.ToUpper()))
                    .ToList();
                numberFilters++;
            }

            if (!String.IsNullOrEmpty(SearchRole))
            {
                users = users.Where(u =>
                    u.SelectedRole.ToUpper().Contains(SearchRole.ToUpper()))
                    .ToList();
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
            if (sortField == "FirstName")
            {
                if (sortDirection == "asc")
                {
                    users = users.OrderBy(p => p.FirstName).ToList();
                    ViewData["filterApplied:UserFirstName"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    users = users.OrderByDescending(p => p.FirstName).ToList();
                    ViewData["filterApplied:UserFirstName"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "LastName")
            {
                if (sortDirection == "asc")
                {
                    users = users.OrderBy(p => p.LastName).ToList();
                    ViewData["filterApplied:UserLastName"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    users = users.OrderByDescending(p => p.LastName).ToList();
                    ViewData["filterApplied:UserLastName"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else if (sortField == "Email") 
            {
                if (sortDirection == "asc")
                {
                    users = users.OrderBy(p => p.Email).ToList();
                    ViewData["filterApplied:UserEmail"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    users = users.OrderByDescending(p => p.Email).ToList();
                    ViewData["filterApplied:UserEmail"] = "<i class='bi bi-sort-down'></i>";
                }
            }
            else 
            {
                if (sortDirection == "asc")
                {
                    users = users.OrderBy(s => s.SelectedRole).ToList();
                    ViewData["filterApplied:UserRole"] = "<i class='bi bi-sort-up'></i>";
                }
                else
                {
                    users = users.OrderByDescending(s => s.SelectedRole).ToList();
                    ViewData["filterApplied:UserRole"] = "<i class='bi bi-sort-down'></i>";
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["filter"] = filter;

            // Retrieve all roles
            var rolesDropdown = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.Roles = new SelectList(rolesDropdown, "Name", "Name");

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pageIndex = page ?? 1; 

            var count = users.Count();
            var items = users.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            if (items.Count() == 0 && count > 0 && pageIndex > 1)
            {
                pageIndex--;
                items = users.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            var pageData = new PaginatedList<UserVM>(items, count, pageIndex, pageSize);

            return View(pageData);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault(); 

            var userDetailsViewModel = new UserDetailsVM
            {
                User = user,
                Role = role 
            };

            return View(userDetailsViewModel);
        }


        //GET: Users/Create
        [HttpGet]
        public IActionResult Create()
        {
            PopulateRoles();
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Status = model.Status,
                    EmailConfirmed = true
                };

                string defaultPassword = "Pa55w@rd";

                var createResult = await _userManager.CreateAsync(user, defaultPassword);                

                if (createResult.Succeeded && !string.IsNullOrWhiteSpace(model.SelectedRole))
                {                   
                    await NotificationCreate(user.Id, "create");

                    var roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    if (!roleResult.Succeeded)
                    {
                        // Handle error in role assignment
                        ModelState.AddModelError("", "Failed to assign role.");
                        PopulateRoles();
                        return View(model);
                    }

                    TempData["SuccessMessage"] = "User created successfully! A password reset link has been sent to the email.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle user creation failure
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            PopulateRoles();
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new UserVM
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SelectedRole = userRoles.FirstOrDefault(),
                Status = user.Status
            };

            PopulateRoles();
            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserVM model)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool emailChanged = !user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase);

                user.Email = model.Email;
                user.UserName = model.Email; 
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Status = model.Status;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    PopulateRoles();
                    return View(model);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (currentRoles.Contains("Admin") && model.SelectedRole != "Admin")
                {
                    ModelState.AddModelError("", "Admins cannot change their own role.");
                    PopulateRoles();
                    return View(model);
                }

                var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    foreach (var error in removeRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    PopulateRoles();
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    if (!addRoleResult.Succeeded)
                    {
                        foreach (var error in addRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        PopulateRoles();
                        return View(model);
                    }
                }

                if (emailChanged)
                {
                    await NotificationCreate(user.Id, "edit");
                    TempData["SuccessMessage"] = "User edited successfully! An email has been sent to reset the password.";
                }
                else
                {
                    TempData["SuccessMessage"] = "User edited successfully!";
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateRoles(); 
            return View(model);
        }

        private void PopulateRoles()
        {
            var roles = _context.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
        }

        public async Task<IActionResult> NotificationCreate(string? id, string operationType)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            ViewData["id"] = id;

            try
            {
                if (user != null)
                {
                    var emailAddress = new EmailAddress
                    {
                        Name = user.UserName,
                        Address = user.Email
                    };

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);

                    var subject = operationType == "create" ? "New User Created" : "User Edited";

                    string logo = "https://haverniagara.com/wp-content/themes/haver/images/logo-haver.png";
                    var msg = new EmailMessage()
                    {
                        ToAddresses = new List<EmailAddress> { emailAddress},
                        Subject = subject,
                        Content = $"<p>{subject}.<br></p>" +
                                  "<p>Please reset your password. <a href=\"" + callbackUrl + "\">Reset Password</a></p>" +
                                  $"<img src=\"{logo}\">" +
                                  "<p>This is an automated email. Please do not reply.</p>",
                };
                    await _emailSender.SendToManyAsync(msg);
                }
                else
                {
                    ViewData["Message"] = "Message NOT sent! No users found.";
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = $"Error: Could not send email message to users. Error: {errMsg}";
            }

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

