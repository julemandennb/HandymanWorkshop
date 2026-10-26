using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Departments;

[Route("api/[controller]")]
[ApiController]

public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // GET: api/Department
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartment()
    {

        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    // GET: api/Department/5
    [HttpGet("{uuid:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid uuid)
    {
        bool canViewAllDetails =
            User.IsInRole("Admin") ||
            User.IsInRole("SuperUser");

        DepartmentDto? department;

        if (canViewAllDetails)
        {
            department = await _departmentService.GetDetailedByUuidAsync(uuid);
        }
        else
        {
            department = await _departmentService.GetByUuidAsync(uuid);
        }

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    // PUT: api/Department/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<IActionResult> PutDepartment(
        Guid uuid,
        DepartmentDto departmentDto)
    {
        if (uuid != departmentDto.Uuid)
        {
            return BadRequest("UUID in the URL does not match the DTO.");
        }

        var updated = await _departmentService.UpdateAsync(uuid, departmentDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Department
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<ActionResult<DepartmentDto>> PostDepartment(
        DepartmentDto departmentDto)
    {
        var result = await _departmentService.CreateAsync(departmentDto);

        return CreatedAtAction(
            nameof(GetDepartment),
            new { uuid = result.Uuid },
            result);
    }

    // DELETE: api/Department/5
    [HttpDelete("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<IActionResult> DeleteDepartment(Guid uuid)
    {
        var deleted = await _departmentService.DeleteAsync(uuid);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}
