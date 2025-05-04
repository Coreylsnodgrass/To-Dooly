using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.ViewModels;
using ToDooly.Services;

namespace ToDooly.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public DashboardController(
            ApplicationDbContext db,
            UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index()
        {
            var uid = _um.GetUserId(User);

            var totalProjects = await _db.Projects
                                         .CountAsync(p => p.OwnerId == uid);

            var totalTasks = await _db.TaskItems
                                         .Include(t => t.Project)
                                         .Where(t => t.Project.OwnerId == uid)
                                         .CountAsync();

            var completedTasks = await _db.TaskItems
                                         .Include(t => t.Project)
                                         .Where(t => t.Project.OwnerId == uid && t.IsComplete)
                                         .CountAsync();

            var vm = new DashboardViewModel
            {
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks
            };

            return View(vm);
        }
    }
}
