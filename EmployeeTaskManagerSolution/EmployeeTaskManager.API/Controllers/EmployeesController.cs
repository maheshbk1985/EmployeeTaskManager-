using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            _logger.LogInformation("CreateEmployee called with data: {@EmployeeDto}", employeeDto);

            try
            {
                var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeDto);
                _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", createdEmployee.EmployeeId);

                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee.");
                return StatusCode(500, "An error occurred while creating the employee.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            _logger.LogInformation("Fetching employee with ID: {EmployeeId}", id);

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Returning employee details for ID: {EmployeeId}", id);
            return Ok(employee);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllEmployees()
        {
            _logger.LogInformation("Fetching all employees.");

            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();

                _logger.LogInformation("Successfully fetched {Count} employees.", employees.Count());

                return Ok(new
                {
                    Message = "Fetched employees successfully.",
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees.");
                return StatusCode(500, "An error occurred while fetching employees.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employeeDto)
        {
            _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

            try
            {
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
                if (updatedEmployee == null)
                {
                    _logger.LogWarning("Employee with ID {EmployeeId} not found for update.", id);
                    return NotFound();
                }

                _logger.LogInformation("Employee with ID {EmployeeId} updated successfully.", id);
                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                return StatusCode(500, "An error occurred while updating the employee.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Employee with ID {EmployeeId} not found for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
                return StatusCode(500, "An error occurred while deleting the employee.");
            }
        }
    }
}
