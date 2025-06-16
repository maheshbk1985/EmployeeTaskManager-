using EmployeeTaskManager.API.DTOs;

namespace EmployeeTaskManager.API.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();          // <-- NEW
        Task<IEnumerable<TaskDto>> GetTasksByEmployeeIdAsync(int employeeId);
        Task<TaskDto> GetTaskByIdAsync(int taskId);
        Task<TaskDto> CreateTaskAsync(TaskDto taskDto);
        Task<TaskDto> UpdateTaskAsync(int taskId, TaskDto taskDto);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}
