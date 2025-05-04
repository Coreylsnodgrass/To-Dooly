using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.ViewModels
{
    public class TaskUpdateDto
    {
        [Required] public int Id { get; set; }
        [Required] public bool IsComplete { get; set; }
    }
}