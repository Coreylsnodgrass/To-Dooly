using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.Entities;
using ToDooly.Models.ViewModels;
using ToDooly.Services;

namespace ToDooly.Controllers
{
    [Authorize]
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public TaskItemsController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        // ──────────────────────────────────────────────────────────────────────
        // LIST  ( /TaskItems?projectId=# )
        // ──────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index(int? projectId)
        {
            var uid = _um.GetUserId(User);
            var query = _db.TaskItems
                           .Include(t => t.Project)
                           .Include(t => t.TaskLabels)
                              .ThenInclude(tl => tl.Label)
                           .Where(t => t.Project.OwnerId == uid);

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            return View(await query.ToListAsync());
        }

        // ──────────────────────────────────────────────────────────────────────
        // CREATE  (GET + POST)
        // ──────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Create(int projectId)
        {
            var uid = _um.GetUserId(User);
            var labels = await _db.Labels
                                  .Where(l => l.OwnerId == uid)
                                  .OrderBy(l => l.Name)
                                  .ToListAsync();

            var vm = new TaskItemEditViewModel
            {
                ProjectId = projectId,
                DueDate = DateTime.Today,
                AllLabels = new MultiSelectList(labels, "Id", "Name")
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItemEditViewModel vm)
        {
            var uid = _um.GetUserId(User);
            if (!ModelState.IsValid)
            {
                vm.AllLabels = new MultiSelectList(
                    await _db.Labels.Where(l => l.OwnerId == uid)
                                    .OrderBy(l => l.Name)
                                    .ToListAsync(),
                    "Id", "Name", vm.SelectedLabelIds);
                return View(vm);
            }

            var task = new TaskItem
            {
                ProjectId = vm.ProjectId,
                Title = vm.Title,
                Description = vm.Description,
                DueDate = vm.DueDate,
                Priority = vm.Priority,
                IsComplete = vm.IsComplete
            };
            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();

            foreach (var lid in vm.SelectedLabelIds)
                _db.TaskLabels.Add(new TaskLabel { TaskItemId = task.Id, LabelId = lid });
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Projects", new { id = vm.ProjectId });
        }

        // ──────────────────────────────────────────────────────────────────────
        // EDIT  (GET)
        // ──────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var uid = _um.GetUserId(User);

            var task = await _db.TaskItems
                                .Include(t => t.Project)
                                .Include(t => t.TaskLabels)
                                .FirstOrDefaultAsync(t => t.Id == id &&
                                                          t.Project.OwnerId == uid);
            if (task == null) return NotFound();

            var vm = new TaskItemEditViewModel
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                IsComplete = task.IsComplete,
                SelectedLabelIds = task.TaskLabels.Select(tl => tl.LabelId).ToList()
            };
            var labels = await _db.Labels.Where(l => l.OwnerId == uid)
                                         .OrderBy(l => l.Name)
                                         .ToListAsync();
            vm.AllLabels = new MultiSelectList(labels, "Id", "Name", vm.SelectedLabelIds);
            return View(vm);
        }

        // ──────────────────────────────────────────────────────────────────────
        // EDIT  (POST)
        // ──────────────────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskItemEditViewModel vm)
        {
            var uid = _um.GetUserId(User);

            if (!ModelState.IsValid)
            {
                vm.AllLabels = new MultiSelectList(
                    await _db.Labels.Where(l => l.OwnerId == uid)
                                    .OrderBy(l => l.Name)
                                    .ToListAsync(),
                    "Id", "Name", vm.SelectedLabelIds);
                return View(vm);
            }

            var task = await _db.TaskItems
                                .Include(t => t.Project)
                                .Include(t => t.TaskLabels)
                                .FirstOrDefaultAsync(t => t.Id == vm.Id &&
                                                          t.Project.OwnerId == uid);
            if (task == null) return NotFound();

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.DueDate = vm.DueDate;
            task.Priority = vm.Priority;
            task.IsComplete = vm.IsComplete;

            _db.TaskLabels.RemoveRange(task.TaskLabels);
            foreach (var lid in vm.SelectedLabelIds)
                _db.TaskLabels.Add(new TaskLabel { TaskItemId = task.Id, LabelId = lid });

            await _db.SaveChangesAsync();

            // back to the project that owns this task
            return RedirectToAction("Details", "Projects", new { id = task.ProjectId });
        }

        // ──────────────────────────────────────────────────────────────────────
        // DELETE  (POST – from stand‑alone TaskItems list)
        // ──────────────────────────────────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _um.GetUserId(User);

            var task = await _db.TaskItems
                                .Include(t => t.Project)
                                .FirstOrDefaultAsync(t => t.Id == id &&
                                                          t.Project.OwnerId == uid);
            if (task == null)            // silently ignore
                return RedirectToAction(nameof(Index));

            var projectId = task.ProjectId;
            _db.TaskItems.Remove(task);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}
