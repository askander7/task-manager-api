using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApplication1.Data;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[Controller]")]
	public class TasksController : ControllerBase
	{
		private readonly AppDbContext _dbContext;
		private readonly IMemoryCache _cache;
		private readonly ILogger<TasksController> _logger;

		private const string TasksCachePrefix = "tasks-";
		private const string TasksCacheVersionKey = "tasks-cache-version";
		public TasksController(AppDbContext dbContext, IMemoryCache cache, ILogger<TasksController> logger)
		{
			_dbContext = dbContext;
			_cache=cache;
			_logger = logger;
		}
		[HttpGet("allTasks")]
		public async Task<IActionResult> GetTasks()
		{
			var tasks = await _dbContext.TaskItems.AsNoTracking().ToListAsync();
			return Ok(tasks);
		}
		[HttpGet("tasksWithPaging")]
		public async Task<IActionResult> GetTasksWithPaging(int page=1, int pageSize=10,string? search=null)
		{
			_logger.LogInformation("Getting tasks. Page: {Page}, PageSize: {PageSize}, Search: {Search}",page,pageSize,search);

			var cacheKey = $"tasks";

			if (_cache.TryGetValue(cacheKey, out object? cachedResult))
			{
				return Ok(cachedResult);
			}

			var query = _dbContext.TaskItems.AsQueryable();
			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(a => a.Title.Contains(search) || (a.Description??"").Contains(search));
			}
			var totalCount = await query.CountAsync();
			var tasks = await query.AsNoTracking().OrderBy(a => a.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

			var result = new
			{
				tasks,
				totalCount,
				page,
				pageSize
			};

			_cache.Set(
		cacheKey,
		result,
		TimeSpan.FromMinutes(1));

			return Ok(result);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetTaskById(int id)
		{
			var task = await _dbContext.TaskItems.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}
			return Ok(task);
		}
		[HttpPost]
		public async Task<IActionResult> CreateTask([FromBody] TaskCreateDTO taskDto)
		{
			TaskItem taskItem = new TaskItem
			{
				Title = taskDto.Title,
				Description = taskDto.Description,
				IsCompleted = taskDto.IsCompleted,
				CreatedAt = DateTime.Now
			};
			 _dbContext.TaskItems.Add(taskItem);
			await _dbContext.SaveChangesAsync();
			_cache.Remove("tasks");
			return Ok(taskItem);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTask(int id,[FromBody] TaskUpdateDTO taskDto)
		{
			var task = await _dbContext.TaskItems.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}
			task.Title= taskDto.Title;
			task.Description= taskDto.Description;
			task.IsCompleted= taskDto.IsCompleted;

			_dbContext.Entry(task)
		.Property(x => x.RowVersion)
		.OriginalValue = taskDto.RowVersion;

			try
			{
				await _dbContext.SaveChangesAsync();
				_cache.Remove("tasks");
				return NoContent();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex,"Error creating task. Title: {Title}", taskDto.Title);

				return Conflict(new
				{
					message = "This task was modified by another user. Please reload the task."
				});
			}
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			var task = await _dbContext.TaskItems.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}
			_dbContext.TaskItems.Remove(task);
			await _dbContext.SaveChangesAsync();
			_cache.Remove("tasks");
			return NoContent();
		}
		[HttpGet("admin-test")]
		[Authorize(Roles ="Admin")]
		public IActionResult AdminTest()
		{
			return Ok(new {Message="you are an administrator!" });
		}
		[HttpGet("test-error")]
		public IActionResult TestError()
		{
			throw new Exception("THIS IS A TEST");
		}

		[AllowAnonymous]
		[HttpPost("transaction-test")]
		public async Task<IActionResult> TransactionTest()
		{
			await using var transaction =
				await _dbContext.Database.BeginTransactionAsync();

			try
			{
				var task1 = new TaskItem
				{
					Title = "Transaction Task 1",
					Description = "Test",
					IsCompleted = false
				};

				_dbContext.TaskItems.Add(task1);

				await _dbContext.SaveChangesAsync();
				var zero = 0;
				var n = 1 / zero;
				// Simulate another operation
				var task2 = new TaskItem
				{
					Title = "Transaction Task 2",
					Description = "Test",
					IsCompleted = false
				};

				_dbContext.TaskItems.Add(task2);

				await _dbContext.SaveChangesAsync();

				await transaction.CommitAsync();

				return Ok("Transaction committed");
			}
			catch
			{
				await transaction.RollbackAsync();

				throw;
			}
		}

	}
}
