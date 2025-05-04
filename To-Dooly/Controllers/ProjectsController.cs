using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.Entities;
using ToDooly.Services;

namespace ToDooly.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _um;

        public ProjectsController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _context = db;
            _um = um;
        }

        // GET: /Projects
        public async Task<IActionResult> Index()
        {
            var uid = _um.GetUserId(User);
            var projects = await _context.Projects
                                    .Where(p => p.OwnerId == uid)
                                    .ToListAsync();
            return View(projects);
        }

        // GET: /Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TaskLabels)
                        .ThenInclude(tl => tl.Label)
                .Where(p => p.Id == id && p.OwnerId == uid)   
                .FirstOrDefaultAsync();

            if (project == null) return NotFound();

            var owner = await _um.FindByIdAsync(project.OwnerId);
            ViewBag.OwnerName = owner?.UserName ?? "(unknown)";
            return View(project);
        }

        // GET: /Projects/Create
        public IActionResult Create() => View();

        // POST: /Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Project project)
        {
            if (!ModelState.IsValid)
                return View(project);

            // set to the current User
            project.OwnerId = _um.GetUserId(User);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _context.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project == null) return NotFound();

            return View(project);
        }

        // POST: /Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Project updated)
        {
            if (id != updated.Id) return BadRequest();
            if (!ModelState.IsValid) return View(updated);

            var uid = _um.GetUserId(User);
            var project = await _context.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project == null) return NotFound();

            project.Title = updated.Title;
            project.Description = updated.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);

            if (project == null) return NotFound();

            var owner = await _um.FindByIdAsync(project.OwnerId);
            ViewBag.OwnerName = owner?.UserName ?? "(unknown)";

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _um.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);

            if (project != null)
            {
                // If you don't have cascade‐delete on Tasks, remove them manually:
                _context.TaskItems.RemoveRange(project.Tasks);

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/TaskList?projectId=5
        [HttpGet]
        public async Task<PartialViewResult> TaskList(int projectId)
        {
            var uid = _um.GetUserId(User);
            var tasks = await _context.TaskItems
                                 .Where(t => t.ProjectId == projectId
                                             && t.Project.OwnerId == uid
                                             && !t.IsComplete)
                                 .Include(t => t.TaskLabels)
                                   .ThenInclude(tl => tl.Label)
                                 .ToListAsync();
            return PartialView("_TaskList", tasks);
        }
        [HttpGet]
        public async Task<PartialViewResult> CompletedTaskList(int projectId)
        {
            var uid = _um.GetUserId(User);
            var tasks = await _context.TaskItems
                                 .Where(t => t.ProjectId == projectId
                                             && t.Project.OwnerId == uid
                                             && t.IsComplete)
                                 .Include(t => t.TaskLabels)
                                   .ThenInclude(tl => tl.Label)
                                 .ToListAsync();
            return PartialView("_CompletedTaskList", tasks);
        }
    }
}
