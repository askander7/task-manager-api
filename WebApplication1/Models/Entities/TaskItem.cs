using System.Collections.ObjectModel;

namespace WebApplication1.Models.Entities
{
	public class TaskItem
	{
		public int Id { get; set; }
		public string Title { get; set; } = "";
		public string? Description { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime CreatedAt { get; set; }
		public byte[] RowVersion { get; set; } = [];
	}
}
