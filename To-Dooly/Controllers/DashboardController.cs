using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDooly.Services;

namespace ToDooly.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext db) => _context = db;

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var tasks = _context.TaskItems.Where(t => t.Project.OwnerId == userId);
            var total = await tasks.CountAsync();
            var completed = await tasks.CountAsync(t => t.IsComplete);
            var dueSoon = await tasks
                .Where(t => !t.IsComplete && t.DueDate <= DateTime.Today.AddDays(3))
                .OrderBy(t => t.DueDate).Take(5).ToListAsync();

            ViewData["Total"] = total;
            ViewData["Completed"] = completed;
            ViewData["DueSoon"] = dueSoon;

            // build weekly chart data…
            return View();
        }
    }
}
