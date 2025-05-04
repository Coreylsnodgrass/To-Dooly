using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public int Priority { get; set; }

        public bool IsComplete { get; set; }

        public int ProjectId { get; set; }

        // remove `required`
        public Project Project { get; set; } = null!;

        public List<TaskLabel> TaskLabels { get; set; } = new();
    }
}
