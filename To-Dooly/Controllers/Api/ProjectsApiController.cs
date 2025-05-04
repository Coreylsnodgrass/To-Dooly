using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDooly.Models.Entities;
using ToDooly.Services;

namespace ToDooly.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsApiController : ControllerBase
    {
        public ApplicationDbContext Db { get; }
        public UserManager<IdentityUser> UserManager { get; }

        public ProjectsApiController(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager)
        {
            Db = dbContext;
            UserManager = userManager;
        }

        // GET api/projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var user = await UserManager.GetUserAsync(User);
            var projects = Db.Projects
                             .Where(p => p.OwnerId == user.Id)
                             .ToList();
            return Ok(projects);
        }

        // GET api/projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var user = await UserManager.GetUserAsync(User);
            var project = await Db.Projects.FindAsync(id);
            if (project == null || project.OwnerId != user.Id)
                return NotFound();
            return Ok(project);
        }

        // POST api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            var user = await UserManager.GetUserAsync(User);
            project.OwnerId = user.Id;
            Db.Projects.Add(project);
            await Db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT api/projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project updated)
        {
            if (id != updated.Id) return BadRequest();

            var user = await UserManager.GetUserAsync(User);
            var project = await Db.Projects.FindAsync(id);
            if (project == null || project.OwnerId != user.Id) return NotFound();

            project.Title = updated.Title;
            project.Description = updated.Description;
            Db.Projects.Update(project);
            await Db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var user = await UserManager.GetUserAsync(User);
            var project = await Db.Projects.FindAsync(id);
            if (project == null || project.OwnerId != user.Id) return NotFound();

            Db.Projects.Remove(project);
            await Db.SaveChangesAsync();
            return NoContent();
        }
    }
}
