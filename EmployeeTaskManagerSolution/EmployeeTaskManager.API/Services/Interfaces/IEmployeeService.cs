using EmployeeTaskManager.API.DTOs;

namespace EmployeeTaskManager.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto);  // ✅ Return the created object
        Task<bool> UpdateEmployeeAsync(int employeeId, EmployeeDto employeeDto); // ✅ Include employeeId
        Task<bool> DeleteEmployeeAsync(int employeeId);
    }
}

