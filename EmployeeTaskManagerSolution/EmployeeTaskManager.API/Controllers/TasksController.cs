using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
        {
            _logger.LogInformation("CreateTask called with data: {@TaskDto}", taskDto);

            if (taskDto.DueDate.HasValue && taskDto.DueDate.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Attempt to create task with past DueDate: {DueDate}", taskDto.DueDate);
                return BadRequest("Due date cannot be in the past.");
            }

            try
            {
                var createdTask = await _taskService.CreateTaskAsync(taskDto);
                _logger.LogInformation("Task created successfully with ID: {TaskId}", createdTask.TaskId);

                return CreatedAtAction(nameof(GetTask), new { id = createdTask.TaskId }, createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task.");
                return StatusCode(500, "An error occurred while creating the task.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetTask(int id)
        {
            _logger.LogInformation("Fetching task with ID: {TaskId}", id);

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Returning task details for ID: {TaskId}", id);
            return Ok(task);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("Fetching all tasks.");

            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                _logger.LogInformation("Fetched {Count} tasks successfully.", tasks.Count());

                return Ok(new
                {
                    Message = "Fetched tasks successfully.",
                    Data = tasks
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks.");
                return StatusCode(500, "An error occurred while fetching tasks.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", id);

            if (taskDto.DueDate.HasValue && taskDto.DueDate.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Attempt to update task with past DueDate: {DueDate}", taskDto.DueDate);
                return BadRequest("Due date cannot be in the past.");
            }

            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto);
                if (updatedTask == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found for update.", id);
                    return NotFound();
                }

                _logger.LogInformation("Task with ID {TaskId} updated successfully.", id);
                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task with ID: {TaskId}", id);
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);

            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("Task with ID {TaskId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID: {TaskId}", id);
                return StatusCode(500, "An error occurred while deleting the task.");
            }
        }
    }
}
