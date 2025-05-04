using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDooly.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC + Razor-Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 2) EF Core + Identity
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Mapped the controller here.
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
