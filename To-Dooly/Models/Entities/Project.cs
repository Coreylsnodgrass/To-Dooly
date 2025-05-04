using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string OwnerId { get; set; } = string.Empty;

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
