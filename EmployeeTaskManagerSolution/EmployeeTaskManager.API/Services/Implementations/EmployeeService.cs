using EmployeeTaskManager.API.Data;
using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Models;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManager.API.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeTaskManagerContext _context;

        public EmployeeService(EmployeeTaskManagerContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            return employee == null ? null : new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Department = employee.Department,
                Designation = employee.Designation
            };
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Select(emp => new EmployeeDto
                {
                    EmployeeId = emp.EmployeeId,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Email = emp.Email,
                    Phone = emp.Phone,
                    Department = emp.Department,
                    Designation = emp.Designation
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Department = employeeDto.Department,
                Designation = employeeDto.Designation
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            employeeDto.EmployeeId = employee.EmployeeId; // ✅ Update with generated ID
            return employeeDto;
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, EmployeeDto employeeDto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return false;

            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.Phone = employeeDto.Phone;
            employee.Department = employeeDto.Department;
            employee.Designation = employeeDto.Designation;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
