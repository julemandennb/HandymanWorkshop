using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Customer;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomersService _customersService;
    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
    }

    // GET: api/Customer
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomer()
    {
        bool canViewAllDetails =
           User.IsInRole("Admin") ||
           User.IsInRole("SuperUser") ||
           User.IsInRole("User");

        if (canViewAllDetails)
        {
            var customer = await _customersService.GetAllAsync();
            return Ok(customer);
        }

        string? currentUserId =
           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
            //return Unauthorized();
            return Ok(new List<CustomerDto>());

        var thisCustomer = await _customersService.ByApplicationUserId(currentUserId);
        return Ok(thisCustomer);
    }

    // GET: api/Customer/5
    [HttpGet("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,User,Customer")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid uuid)
    {

        if (!await CanAccessCustomerAsync(uuid))
        {
            return NotFound();
        }


        CustomerDto? customerDto =
            await _customersService.GetDetailedByUuidAsync(uuid);

        if (customerDto == null)
        {
            return NotFound();
        }

        return Ok(customerDto);
    }

    // PUT: api/Customer/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,Customer")]

    public async Task<IActionResult> PutCustomer(Guid uuid, CustomerDto customerDto)
    {
        if (uuid != customerDto.Uuid)
        {
            return BadRequest("UUID in the URL does not match the DTO.");
        }

        if (!await CanAccessCustomerAsync(uuid))
        {
            return NotFound();

        }

        var updated = await _customersService.UpdateAsync(uuid, customerDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Customer
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerDto customerDto)
    {
        var result = await _customersService.CreateAsync(customerDto);

        return CreatedAtAction(
            nameof(GetCustomer),
            new { uuid = result.Uuid },
            result);
    }

    // DELETE: api/Customer/5
    [HttpDelete("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser,Customer")]

    public async Task<IActionResult> DeleteCustomer(Guid uuid)
    {
        if (!await CanAccessCustomerAsync(uuid))
        {
            return NotFound();

        }

        var deleted = await _customersService.DeleteAsync(uuid);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("getselectoption")]
    [Authorize(Roles = "Admin,SuperUser,User,Customer")]
    public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetSelectOption()
    {
        bool IsCustomer =
        User.IsInRole("Customer");

        if(IsCustomer)
        {
            string? currentUserId =
           User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(currentUserId == null)
                return Ok(new List<SelectOptionDto>());

            var localcustom = await _customersService.GetSelectOption(currentUserId);
            return Ok(localcustom);
        }


        var customers = await _customersService.GetSelectOption();
        return Ok(customers);
    }


    private async Task<bool> CanAccessCustomerAsync(Guid uuid)
    {
        bool canViewAllDetails =
            User.IsInRole("Admin") ||
            User.IsInRole("SuperUser") ||
            User.IsInRole("User");

        Customer? customer =
            await _customersService.GetModelByUuidAsync(uuid);

        if (customer == null)
        {
            return false;
        }

        if (canViewAllDetails)
        {
            return true;
        }

        string? currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        return currentUserId != null &&
               customer.ApplicationUserId == currentUserId;
    }

}
