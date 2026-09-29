using System.Security.Claims;
using DiveCenterManager.Data;
using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddSingleton<SqlConnectionFactory>();

// Application Services
builder.Services.AddScoped<DiveSiteService>();
builder.Services.AddScoped<BoatService>();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<OperationalRoleService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<TransactionCategoryService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ReportsService>();

// Authentication Services
builder.Services.AddScoped<
    IPasswordHasher<AppUser>,
    PasswordHasher<AppUser>>();

builder.Services.AddScoped<AppUserService>();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        options.SlidingExpiration = true;

        options.Events.OnValidatePrincipal =
            async context =>
            {
                var userIdValue =
                    context.Principal?
                        .FindFirst(
                            ClaimTypes.NameIdentifier)?
                        .Value;

                if (!int.TryParse(
                        userIdValue,
                        out var userId))
                {
                    context.RejectPrincipal();

                    await context.HttpContext
                        .SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme);

                    return;
                }

                var appUserService =
                    context.HttpContext
                        .RequestServices
                        .GetRequiredService<AppUserService>();

                var user =
                    await appUserService
                        .GetAuthenticationUserByIdAsync(
                            userId);

                if (user is null ||
                    !user.IsActive)
                {
                    context.RejectPrincipal();

                    await context.HttpContext
                        .SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme);

                    return;
                }

                var cookieRole =
                    context.Principal?
                        .FindFirst(
                            ClaimTypes.Role)?
                        .Value;

                if (cookieRole != user.SystemRole)
                {
                    context.RejectPrincipal();

                    await context.HttpContext
                        .SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme);

                    return;
                }

                var securityVersionValue =
                    context.Principal?
                        .FindFirst(
                            "SecurityVersion")?
                        .Value;

                if (!int.TryParse(
                        securityVersionValue,
                        out var cookieSecurityVersion))
                {
                    context.RejectPrincipal();

                    await context.HttpContext
                        .SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme);

                    return;
                }

                if (cookieSecurityVersion !=
                    user.SecurityVersion)
                {
                    context.RejectPrincipal();

                    await context.HttpContext
                        .SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme);
                }
            };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();