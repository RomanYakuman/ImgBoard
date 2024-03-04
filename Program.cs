using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using MvcApp.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("Cookies").
    AddCookie(options => options.LoginPath = "/auth/login");
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Page}/{id?}");
app.Run();