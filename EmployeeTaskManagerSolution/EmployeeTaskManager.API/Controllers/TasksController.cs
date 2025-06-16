using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
        {
            // If DueDate is provided, validate it
            if (taskDto.DueDate.HasValue && taskDto.DueDate.Value < DateTime.UtcNow)
                return BadRequest("Due date cannot be in the past.");

            var createdTask = await _taskService.CreateTaskAsync(taskDto);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            if (taskDto.DueDate.HasValue && taskDto.DueDate.Value < DateTime.UtcNow)
                return BadRequest("Due date cannot be in the past.");

            var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto);
            if (updatedTask == null)
                return NotFound();

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
