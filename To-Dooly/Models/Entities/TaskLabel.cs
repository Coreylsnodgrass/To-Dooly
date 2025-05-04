namespace ToDooly.Models.Entities
{
    public class TaskLabel
    {
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        public int LabelId { get; set; }
        public Label Label { get; set; } = null!;
    }
}
