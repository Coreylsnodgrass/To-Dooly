using System.ComponentModel.DataAnnotations;

namespace ToDooly.Models.Entities
{
    public enum PriorityLevel
    {
        [Display(Name = "Top priority")]
        Top = 1,
        [Display(Name = "High priority")]
        High = 2,
        [Display(Name = "Medium priority")]
        Medium = 3,
        [Display(Name = "Low priority")]
        Low = 4,
        [Display(Name = "Bottom priority")]
        Bottom = 5
    }
}