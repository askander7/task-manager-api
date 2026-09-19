namespace WebApplication1.Models.DTOs
{
	public class TaskUpdateDTO
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public bool IsCompleted { get; set; }
		public byte[] RowVersion { get; set; } = [];
	}
}
