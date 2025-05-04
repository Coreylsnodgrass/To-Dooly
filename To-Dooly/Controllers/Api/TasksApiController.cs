using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.Entities;
using ToDooly.Services;

namespace ToDooly.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public TasksApiController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        // GET: api/tasks?projectId=123
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? projectId)
        {
            var uid = _um.GetUserId(User);
            var query = _db.TaskItems
                           .Include(t => t.Project)
                           .Where(t => t.Project.OwnerId == uid);

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            var list = await query.ToListAsync();
            return Ok(list);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var uid = _um.GetUserId(User);
            var task = await _db.TaskItems
                                .Include(t => t.Project)
                                .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);

            if (task == null) return NotFound();
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskItem model)
        {
            var uid = _um.GetUserId(User);
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == model.ProjectId && p.OwnerId == uid);
            if (project == null) return BadRequest("Invalid project ID");

            model.IsComplete = false;
            _db.TaskItems.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaskItem updated)
        {
            if (id != updated.Id) return BadRequest();

            var uid = _um.GetUserId(User);
            var existing = await _db.TaskItems
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);
            if (existing == null) return NotFound();

            // Basic field updates
            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.DueDate = updated.DueDate;
            existing.Priority = updated.Priority;
            existing.IsComplete = updated.IsComplete;

            // Optionally allow moving between projects you own
            if (updated.ProjectId != existing.ProjectId)
            {
                var newProj = await _db.Projects
                                       .FirstOrDefaultAsync(p => p.Id == updated.ProjectId && p.OwnerId == uid);
                if (newProj == null) return BadRequest("Invalid new project ID");
                existing.ProjectId = updated.ProjectId;
            }

            _db.TaskItems.Update(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = _um.GetUserId(User);
            var existing = await _db.TaskItems
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);
            if (existing == null) return NotFound();

            _db.TaskItems.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
