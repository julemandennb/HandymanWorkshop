using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Customers;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Customer;
using Shared.DTO.WorkOrder;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;
    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    // GET: api/WorkOrder
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrder()
    {
        bool canViewAllDetails =
          User.IsInRole("Admin") ||
          User.IsInRole("SuperUser");

        if (canViewAllDetails)
        {
            var customer = await _workOrderService.GetAllAsync();
            return Ok(customer);
        }

        string? currentUserId =
          User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
            //return Unauthorized();
            return Ok(new List<CustomerDto>());

        var thisCustomer = await _workOrderService.ByApplicationUserId(currentUserId);
        return Ok(thisCustomer);

    }

    // GET: api/WorkOrder/5
    [HttpGet("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,User,Customer")]
    public async Task<ActionResult<WorkOrderDto>> GetWorkOrder(Guid uuid)
    {
        if (!await CanAccessWorkOrderAsync(uuid))
        {
            return NotFound();
        }

        WorkOrderDto? workOrderDto =
            await _workOrderService.GetDetailedByUuidAsync(uuid);

        if (workOrderDto == null)
        {
            return NotFound();
        }

        return Ok(workOrderDto);
    }

    // PUT: api/WorkOrder/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,User,Customer")]
    public async Task<IActionResult> PutWorkOrder(Guid uuid, WorkOrderDto workorderDto)
    {
        if (uuid != workorderDto.Uuid)
        {
            return BadRequest("UUID in the URL does not match the DTO.");
        }

        if (!await CanAccessWorkOrderAsync(uuid))
        {
            return NotFound();

        }

        var updated = await _workOrderService.UpdateAsync(uuid, workorderDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/WorkOrder
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Admin,SuperUser,User,Customer")]
    public async Task<ActionResult<WorkOrderDto>> PostWorkOrder(WorkOrderDto workorderDto)
    {
        var result = await _workOrderService.CreateAsync(workorderDto);

        return CreatedAtAction(
            nameof(GetWorkOrder),
            new { uuid = result.Uuid },
            result);
    }

    // DELETE: api/WorkOrder/5
    [HttpDelete("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<IActionResult> DeleteWorkOrder(Guid uuid)
    {

        var deleted = await _workOrderService.DeleteAsync(uuid);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<bool> CanAccessWorkOrderAsync(Guid uuid)
    {
        bool canViewAllDetails =
            User.IsInRole("Admin") ||
            User.IsInRole("SuperUser") ||
            User.IsInRole("User");

        WorkOrder? workOrder =
            await _workOrderService.GetModelByUuidAsync(uuid);

        if (workOrder == null)
        {
            return false;
        }

        if (canViewAllDetails)
        {
            return true;
        }

        string? currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool isOk = currentUserId != null &&  (workOrder.Customer?.ApplicationUserId == currentUserId || workOrder.Employee?.ApplicationUserId == currentUserId);

        return isOk;
    }
}
