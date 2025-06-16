using EmployeeTaskManager.API.Data;
using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Models;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManager.API.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly EmployeeTaskManagerContext _context;

        public TaskService(EmployeeTaskManagerContext context)
        {
            _context = context;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return null;

            return new TaskDto
            {
                TaskId = task.TaskId,
                EmployeeId = task.EmployeeId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                CreatedDate = task.CreatedDate ?? DateTime.MinValue
            };
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .Select(t => new TaskDto
                {
                    TaskId = t.TaskId,
                    EmployeeId = t.EmployeeId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                    CreatedDate = t.CreatedDate ?? DateTime.MinValue
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByEmployeeIdAsync(int employeeId)
        {
            return await _context.Tasks
                .Where(t => t.EmployeeId == employeeId)
                .Select(t => new TaskDto
                {
                    TaskId = t.TaskId,
                    EmployeeId = t.EmployeeId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                    CreatedDate = t.CreatedDate ?? DateTime.MinValue
                })
                .ToListAsync();
        }

        public async Task<TaskDto> CreateTaskAsync(TaskDto taskDto)
        {
            var task = new Models.Task
            {
                EmployeeId = taskDto.EmployeeId,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                DueDate = taskDto.DueDate.HasValue ? DateOnly.FromDateTime(taskDto.DueDate.Value) : null,
                CreatedDate = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                TaskId = task.TaskId,
                EmployeeId = task.EmployeeId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                CreatedDate = task.CreatedDate ?? DateTime.MinValue
            };
        }

        public async Task<TaskDto> UpdateTaskAsync(int taskId, TaskDto taskDto)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return null;

            task.EmployeeId = taskDto.EmployeeId;
            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.Status = taskDto.Status;
            task.DueDate = taskDto.DueDate.HasValue ? DateOnly.FromDateTime(taskDto.DueDate.Value) : null;

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                TaskId = task.TaskId,
                EmployeeId = task.EmployeeId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                CreatedDate = task.CreatedDate ?? DateTime.MinValue
            };
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
