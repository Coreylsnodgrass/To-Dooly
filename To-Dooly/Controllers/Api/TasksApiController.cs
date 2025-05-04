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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _um;

        public TasksApiController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _context = db;
            _um = um;
        }

        // GET: api/tasks?projectId=123
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? projectId)
        {
            var uid = _um.GetUserId(User);
            var query = _context.TaskItems
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
            var task = await _context.TaskItems
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
            var project = await _context.Projects
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

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaskUpdateDto dto)
        {
            var uid = _um.GetUserId(User);
            var existing = await _context.TaskItems
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);
            if (existing == null) return NotFound();

            existing.IsComplete = dto.IsComplete;
            _context.TaskItems.Update(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = _um.GetUserId(User);
            var existing = await _context.TaskItems
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == id && t.Project.OwnerId == uid);
            if (existing == null) return NotFound();

            _context.TaskItems.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
