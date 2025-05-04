using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDooly.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── 1) Register MVC, Razor‐Pages, and API controllers ─────────
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();    // <-- register [ApiController]

// ─── 2) Register EF Core & Identity ───────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
       .AddDefaultIdentity<IdentityUser>(options =>
       {
           options.SignIn.RequireConfirmedAccount = false;
       })
       .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// ─── 3) Middleware ─────────────────────────────────────────────
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

// ─── 4) Routes ─────────────────────────────────────────────────
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllers();    // <-- map your attribute‐routed API controllers
app.MapRazorPages();     // <-- map Identity Razor‐Pages

app.Run();
