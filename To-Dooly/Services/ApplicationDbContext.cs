using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDooly.Models.Entities;

namespace ToDooly.Services
{
    public class ApplicationDbContext
      : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<TaskLabel> TaskLabels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskLabel>()
                   .HasKey(tl => new { tl.TaskItemId, tl.LabelId });

            builder.Entity<TaskLabel>()
                   .HasOne(tl => tl.TaskItem)
                   .WithMany(t => t.TaskLabels)
                   .HasForeignKey(tl => tl.TaskItemId);

            builder.Entity<TaskLabel>()
                   .HasOne(tl => tl.Label)
                   .WithMany(l => l.TaskLabels)
                   .HasForeignKey(tl => tl.LabelId);
        }
    }
}
