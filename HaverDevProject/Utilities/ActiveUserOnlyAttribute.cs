using HaverDevProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class ActiveUserOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
        {
            var user = userManager.FindByIdAsync(userId).Result;
            if (user == null || !user.Status)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
