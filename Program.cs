

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpLogging(opts => {});
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options => 
    {
        options.LoginPath = "/auth/login";
        options.Cookie.Name = "auth";
    });
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Page}/{Id?}");
app.Run();
