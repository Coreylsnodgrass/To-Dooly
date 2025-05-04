using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public PriorityLevel Priority { get; set; }

        public bool IsComplete { get; set; }

        public int ProjectId { get; set; }

        [JsonIgnore]                    // ← Ignore in JSON
        public Project Project { get; set; } = null!;

        [JsonIgnore]                    // ← Ignore in JSON
        public List<TaskLabel> TaskLabels { get; set; } = new();
    }
}
