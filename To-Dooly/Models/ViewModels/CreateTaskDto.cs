namespace ToDooly.Models.ViewModels
{
    public class CreateTaskDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
    }
}