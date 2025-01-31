using HaverDevProject.Data;
using HaverDevProject.Utilities;
using HaverDevProject.ViewModels;
using HaverDevProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OfficeOpenXml;
using System.Text.Json.Serialization;
using HaverDevProject.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using HaverDevProject.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<HaverNiagaraContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

Spire.Xls.License.LicenseProvider.SetLicenseFileName("license.elic.xml");
//Spire.Xls.License.LicenseProvider.SetLicenseKey("0z6VtQEAqbBQ7aVtAx1CM4Cg72V1k1NePBlWhtFJtMI7Cm4C1o/DaJ2WCX2ZTXliCa2mmBGEgkrs985sAFnQmC2+rqzGEcfdYHmBY3tDvA4sfUbu7pozwz1dbhJMCZhLrnrk7axmayOqLj1rbqK63EjPy6hS3P5Yevr5mq9MFcZDadkmg1vDzeGKLFw/fbLFRJYq3Pg+Uqmm9Qj/DW24CBc12HOgZJRCG92ZcHR23VGtwQVb/XM/ohFDVaHiNGFixFDp3Cuwd3TxHpp2YEBUGf34svtmbyaUP67Kfv4gvBFTIcR8EK0PEORXniiLbq/4HYzKIr4PqUlw3cMyPMtGnP2IbLKrIxTwY+nGre95WgjKeqxKNpDfBgaHtaV9/Er+LySGbVu3Lfz4/Z1aagpFr5m39sj16LWsa5sh+ob1O24w3aYOj4SDtRBWjkEzbnERQ64XjvNniyS65BFp56E6lgV2X3BU03u5LwzDWHEXOD1CvJS7KgEPMBvZ4mkaicZQhTq3dFoitK/SaUh8kKey3je+STZ5gopRbwHfNBwk3UdtI9wXl8sv229X92+achidmpiF3H5i/czOVhnP74fy0i5AtJmlT+csAUY+DRBzNVU9DJjD9mXJHPo6bbHifxetCyijG2uOw6vayOTFsQEQOelPZ+pBQWaoQ8sjPX85p3aqafmeqPdsxU1+ok3qT8C+8Am4tXhW7trd5lb+TwefQxDKbmUcDvmtv86w4eKftlTnq1UPy43QKLkLUCN09gd0NMQsUHJiLopViRVL3j1vQVX4KdcHHYr2MjcGMwExyaHoydQx1+PrhZXYtTa4WwnJHhHc8ogPQApbgi5Ql/CLauwL2i+c+8bqrKkI2OdECx4Cp+8QO5R9J2Zc+41cjAfwGjN1JdmkW6XPHMxJTTv//+vvd7+xni9WvpI/hH0bOGF0Y/O9TPyi0nFxmreb1ANDQcyadYEb8bw481Li5YOv6e1QFl3Z5GSl8D6Ec3OnLdMgkRO7CRlrNj1LXfCSk4GHaiDny8fdhmNa+r+DBxhWmLOw0iJN0wd2oVSnp8k7vRXSjM+GBCG+5jV8uDi9VMbtam+XK7suF+BBjoE2oHqYZlr1ecxYNQiFmrXJ5kmC++cwKC/qNVShZWt4ONA50pcDUwZx/sc0z2Ow+C9ulCVDhRy6F4hsY0TBcPftBy5AUxC+A83q5x0JYBt/805lNIU9b0Vvh+IrNepOlLMlB+4y88jp3qY/qN4OaBEXAH9MDbWJKHuzJmBl62BrhBkJJ2maDl9lnVu1LDjzVhp1lyOOCZV6LN5EBGiGxKeKdmqNB+lzta1YSE76YyPtgl1FXDjbOOyuUNZ53KSykNoINhUwmGH8/psAMtC9JQLusMLMLFTkr3GQxOHr53JQmcqG47JmjTyS8H343dUgevNVfYhvGua2hXD6I3BBCsPITlS12ZjGb+SUmKLL0KBDFeamIkl3Aq+2NBHuXmPqasRx+kKRXNwGvAlL4vPpVtg6MYEy8rL8QehKc3ybw5IpgrenYCp8YE+py+hDx/tOW7E53n90ovX5IZEaLw+ahM1P6F0RfFhA3ccL9ts3GHBL2Xwp/Eb8ovJ8cg3S/3o=");

//To give access to IHttpContextAccesor for Audit Data with IAuditable
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//For email service configuration
builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration
    .GetSection("EmailConfiguration").Get<EmailConfiguration>());

//Configuration Registration
builder.Services.AddSingleton<INumYearsService, NumYearsService>();

//NcrArchivingService for Manual Archiving
builder.Services.AddScoped<NcrArchivingService>();

//Background Service for Archiving
builder.Services.AddHostedService<AutomaticNcrArchivingService>();

//For the Identity System
builder.Services.AddTransient<IEmailSender, EmailSender>();

//Email with added methods for production use.
builder.Services.AddTransient<IMyEmailSender, MyEmailSender>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts Jorge.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    bool isApiPath = path.StartsWithSegments("/api");
    bool isIdentityPath = path.StartsWithSegments("/Identity");

    if (!context.User.Identity.IsAuthenticated && !isIdentityPath && !isApiPath)
    {
        // Redirect to login page
        context.Response.Redirect("/Identity/Account/Login");
    }
    else
    {
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//Seed Sata
HaverInitializer.Seed(app);
ApplicationDbInitializer.Seed(app);

app.Run();