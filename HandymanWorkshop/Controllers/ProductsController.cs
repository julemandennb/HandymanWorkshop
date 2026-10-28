using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Departments;
using Shared.DTO.Product;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/Product
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    // GET: api/Product/5
    [HttpGet("{uuid:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid uuid)
    {
        bool canViewAllDetails =
         User.IsInRole("Admin") ||
         User.IsInRole("SuperUser") ||
          User.IsInRole("User");

        ProductDto? product;

        if (canViewAllDetails)
        {
            product = await _productService.GetDetailedByUuidAsync(uuid);
        }
        else
        {
            product = await _productService.GetByUuidAsync(uuid);
        }

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // PUT: api/Product/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<IActionResult> PutProduct(Guid uuid, ProductDto productDto)
    {
        if (uuid != productDto.Uuid)
        {
            return BadRequest("UUID in the URL does not match the DTO.");
        }

        var updated = await _productService.UpdateAsync(uuid, productDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Product
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
    {
        var result = await _productService.CreateAsync(productDto);

        return CreatedAtAction(
            nameof(GetProduct),
            new { uuid = result.Uuid },
            result);
    }

    // DELETE: api/Product/5
    [HttpDelete("{uuid:guid}")]
    [Authorize(Roles = "Admin,SuperUser")]
    public async Task<IActionResult> DeleteProduct(Guid uuid)
    {
        var deleted = await _productService.DeleteAsync(uuid);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("getselectoption")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetSelectOption()
    {
        var product = await _productService.GetSelectOption();
        return Ok(product);
    }

    [HttpGet("getSelectoptionByDepartment/{uuid:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetSelectOption(Guid uuid)
    {
        var products = await _productService.getSelectoptionByDepartment(uuid);
        return Ok(products);
    }
}
