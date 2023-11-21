global using CDSS.Models;
global using CDSS.Services;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/MedicalStaffs/Login/";
                    options.AccessDeniedPath = "/MedicalStaffs/Forbidden/";
                });
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<CDSS.Services.AppDbContext>(
   options => options.UseSqlServer(
       builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MedicalStaffs}/{action=Login}/{id?}");

app.Run();
