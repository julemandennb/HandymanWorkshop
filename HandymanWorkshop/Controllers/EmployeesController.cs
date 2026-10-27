using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Employee;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesService _employeesService;

    public EmployeesController(IEmployeesService employeesService)
    {
        _employeesService = employeesService;
    }

    // GET: api/Employee
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployee()
    {
        var employees = await _employeesService.GetAllAsync();
        return Ok(employees);
    }

    // GET: api/Employee/5
    [HttpGet("{uuid:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid uuid)
    {
        bool canViewAllDetails =
            User.IsInRole("Admin") ||
            User.IsInRole("SuperUser") ||
            User.IsInRole("User");

        EmployeeDto? employee;

        if (canViewAllDetails)
        {
            employee = await _employeesService.GetDetailedByUuidAsync(uuid);
        }
        else
        {
            employee = await _employeesService.GetByUuidAsync(uuid);
        }

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    // PUT: api/Employee/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,User")]
    public async Task<IActionResult> PutEmployee(Guid uuid, EmployeeDto employeeDto)
    {
        if (uuid != employeeDto.Uuid)
        {
            return BadRequest("UUID in the URL does not match the DTO.");
        }

        bool canViewAllDetails =
           User.IsInRole("Admin") ||
           User.IsInRole("SuperUser");

        if (!canViewAllDetails)
            employeeDto.Department = null;


        var updated = await _employeesService.UpdateAsync(uuid, employeeDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Employee
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<ActionResult<EmployeeDto>> PostEmployee(EmployeeDto employeeDto)
    {
        var result = await _employeesService.CreateAsync(employeeDto);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { uuid = result.Uuid },
            result);
    }

    // DELETE: api/Employee/5
    [HttpDelete("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,User")]
    public async Task<IActionResult> DeleteEmployee(Guid uuid)
    {
        var deleted = await _employeesService.DeleteAsync(uuid);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
