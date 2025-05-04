using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.Entities
{
    public class Label
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;

        public List<TaskLabel> TaskLabels { get; set; } = new();
    }
}
