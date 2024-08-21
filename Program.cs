using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Molla.Models;
using Molla.Services;
using MyApplication.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    //custom Password
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = true;
}
).AddEntityFrameworkStores<ApplicationDbContext>();

// Configure the Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    // If the LoginPath isn't set, ASP.NET Core defaults the path to /Account/Login.
    options.LoginPath = "/login"; // Set your login path here
});
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        googleOptions.ClientId = builder.Configuration["ExternalLogin:Google:ClientId"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
        googleOptions.ClientSecret = builder.Configuration["ExternalLogin:Google:ClientSecret"];
#pragma warning restore CS8601 // Possible null reference assignment.
    });
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISendMailService, SendMailService>();
builder.Services.Configure<MailSettingOptions>(builder.Configuration.GetSection(MailSettingOptions.MailSettings));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
