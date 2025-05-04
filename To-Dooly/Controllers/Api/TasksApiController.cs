using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.Entities;
using ToDooly.Models.ViewModels;
using ToDooly.Services;

namespace ToDooly.Controllers.Api
{
    [Route("api/tasks")]
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
        public async Task<IActionResult> Post([FromBody] CreateTaskDto dto)
        {
            var uid = _um.GetUserId(User);
            var project = await _db.Projects
                                   .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.OwnerId == uid);
            if (project == null)
                return BadRequest("Invalid project ID");

            var task = new TaskItem
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                IsComplete = false
            };

            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var uid = _um.GetUserId(User);
            var existing = await _db.TaskItems
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);
            if (existing == null) return NotFound();

            existing.IsComplete = dto.IsComplete;
            // keep existing.IsComplete unchanged here

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
