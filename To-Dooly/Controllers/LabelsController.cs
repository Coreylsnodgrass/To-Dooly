using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;        
using ToDooly.Models.Entities;
using ToDooly.Models.ViewModels;
using ToDooly.Services;

[Authorize]
public class LabelsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _um;

    public LabelsController(ApplicationDbContext db, UserManager<IdentityUser> um)
    {
        _db = db;
        _um = um;
    }

    // GET: Labels
    public async Task<IActionResult> Index()
    {
        var uid = _um.GetUserId(User);
        var labels = await _db.Labels
                              .Where(l => l.OwnerId == uid)
                              .ToListAsync();

        var vm = new List<LabelViewModel>(labels.Count);
        foreach (var l in labels)
        {
            // Lookup the user once per label
            var user = await _um.FindByIdAsync(l.OwnerId);
            vm.Add(new LabelViewModel
            {
                Id = l.Id,
                Name = l.Name,
                OwnerName = user?.UserName ?? user?.Email ?? "(unknown)"
            });
        }

        return View(vm);
    }

    // GET: Labels/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var uid = _um.GetUserId(User);
        var label = await _db.Labels.FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == uid);
        if (label == null) return NotFound();

        // owner lookup
        var user = await _um.FindByIdAsync(label.OwnerId);
        var vm = new LabelViewModel
        {
            Id = label.Id,
            Name = label.Name,
            OwnerName = user?.UserName ?? user?.Email ?? "(unknown)"
        };
        return View(vm);
    }

    // GET: Labels/Create
    public IActionResult Create() => View();

    // POST: Labels/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Label label)
    {
        if (!ModelState.IsValid) return View(label);
        label.OwnerId = _um.GetUserId(User);
        _db.Labels.Add(label);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Labels/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var uid = _um.GetUserId(User);
        var label = await _db.Labels
                             .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == uid);
        if (label == null) return NotFound();
        return View(label);
    }

    // POST: Labels/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Label label)
    {
        if (id != label.Id) return BadRequest();
        if (!ModelState.IsValid) return View(label);

        var uid = _um.GetUserId(User);
        var existing = await _db.Labels
                                .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == uid);
        if (existing == null) return NotFound();

        existing.Name = label.Name;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Labels/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var uid = _um.GetUserId(User);
        var label = await _db.Labels
                             .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == uid);
        if (label == null) return NotFound();
        return View(label);
    }

    // POST: Labels/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var uid = _um.GetUserId(User);
        var label = await _db.Labels
                             .Include(l => l.TaskLabels)
                             .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == uid);
        if (label != null)
        {
            _db.TaskLabels.RemoveRange(label.TaskLabels);
            _db.Labels.Remove(label);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
