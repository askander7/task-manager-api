namespace WebApplication1.Models.DTOs
{
	public class TaskCreateDTO
	{
		public string Title { get; set; }
		public string? Description { get; set; }
		public bool IsCompleted { get; set; }
	}
}
