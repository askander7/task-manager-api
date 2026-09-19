namespace WebApplication1.Models.Validators
{
	public class TaskValidator
	{
		public bool IsValidTitle(string? title)
		{
			return !string.IsNullOrWhiteSpace(title)
				   && title.Length <= 100;
		}
	}
}
