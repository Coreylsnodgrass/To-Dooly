using System;
using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.ViewModels
{
    public class CreateTaskDto
    {
        [Required] public int ProjectId { get; set; }
        [Required] public string Title { get; set; } = "";
        public string? Description { get; set; }
        [Required] public DateTime DueDate { get; set; }


        [Range(1, 5)]
        public int Priority { get; set; }
    }
}