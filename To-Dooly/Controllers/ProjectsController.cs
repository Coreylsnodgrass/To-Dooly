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
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public ProjectsController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        // GET: /Projects
        public async Task<IActionResult> Index()
        {
            var uid = _um.GetUserId(User);
            var projects = await _db.Projects
                                    .Where(p => p.OwnerId == uid)
                                    .ToListAsync();
            return View(projects);
        }

        // GET: /Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project == null) return NotFound();

            return View(project);
        }

        // GET: /Projects/Create
        public IActionResult Create() => View();

        // POST: /Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Project project)
        {
            if (!ModelState.IsValid) return View(project);

            project.OwnerId = _um.GetUserId(User);
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _db.Projects
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
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project == null) return NotFound();

            project.Title = updated.Title;
            project.Description = updated.Description;
            _db.Projects.Update(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uid = _um.GetUserId(User);
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project == null) return NotFound();

            return View(project);
        }

        // POST: /Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _um.GetUserId(User);
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
            if (project != null)
            {
                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Projects/TaskList?projectId=5
        [HttpGet]
        public async Task<PartialViewResult> TaskList(int projectId)
        {
            var uid = _um.GetUserId(User);
            var tasks = await _db.TaskItems
                                 .Where(t => t.ProjectId == projectId
                                          && t.Project.OwnerId == uid)
                                 .ToListAsync();
            return PartialView("_TaskList", tasks);
        }
    }
}
