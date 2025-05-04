// Models/ViewModels/TaskItemEditViewModel.cs
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDooly.Models.Entities;

namespace ToDooly.Models.ViewModels
{
    public class TaskItemEditViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public PriorityLevel Priority { get; set; }
        public bool IsComplete { get; set; }
        public List<int> SelectedLabelIds { get; set; } = new();

        [BindNever]
        [ValidateNever]
        public MultiSelectList AllLabels { get; set; } = null!;
    }
}
