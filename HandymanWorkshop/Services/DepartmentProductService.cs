using HandymanWorkshop.Data;
using HandymanWorkshop.Help.DepartmentProducts;
using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Help.Products;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Pivot;

namespace HandymanWorkshop.Services
{
    public class DepartmentProductService : IDepartmentProductService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<DepartmentProductDto>> GetAllAsync()
        {
           var departmentProducts =  await _context.DepartmentProducts
                .AsNoTracking()
                .ToListAsync();

            return departmentProducts
                .Select(DepartmentProductToDto.ToDto)
                .ToList();
        }

        // GET BY UUID
        public async Task<DepartmentProductDto?> GetByUuidAsync(Guid uuid)
        {
           var departmentProducts =  await _context.DepartmentProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(dp => dp.Uuid == uuid);

            return departmentProducts == null
                ? null
                : DepartmentProductToDto.ToDto(departmentProducts);
        }

        public async Task<List<SelectOptionDto>> GetSelectOption()
        {
            var departmentProducts = await _context.DepartmentProducts
               .AsNoTracking()
               .ToListAsync();

            return departmentProducts
                .Select(DepartmentProductToDto.ToSelectOptionDto)
                .ToList();
        }

        // ADD, UPDATE AND DELETE
        public async Task SyncAsync(
            Guid departmentUuid,
            ICollection<DepartmentProductDto> dtoList)
        {
            // Find department
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Uuid == departmentUuid);

            if (department == null)
                throw new KeyNotFoundException("Department not found.");

            // Validate quantities
            if (dtoList.Any(x => x.Total < 0))
                throw new ArgumentException(
                    "Product total cannot be negative.");

            // Every product must have a UUID
            if (dtoList.Any(x => !x.ProductUuid.HasValue))
                throw new ArgumentException(
                    "ProductUuid is required for every product.");

            var productUuids = dtoList
                .Select(x => x.ProductUuid!.Value)
                .ToList();

            // Prevent duplicate products in the submitted list
            if (productUuids.Distinct().Count() != productUuids.Count)
                throw new ArgumentException(
                    "A product cannot appear more than once.");

            // Get all submitted products
            var products = await _context.Products
                .Where(p => productUuids.Contains(p.Uuid))
                .ToDictionaryAsync(p => p.Uuid);

            if (products.Count != productUuids.Distinct().Count())
                throw new KeyNotFoundException(
                    "One or more products were not found.");

            // Get existing department-product relationships
            var existingItems = await _context.DepartmentProducts
                .Where(dp => dp.DepartmentId == department.Id)
                .ToListAsync();

            var submittedProductUuids = productUuids.ToHashSet();

            // DELETE: Remove relationships absent from the submitted list
            var productIdsToKeep = products.Values
                .Select(p => p.Id)
                .ToHashSet();

            var itemsToDelete = existingItems
                .Where(dp => !productIdsToKeep.Contains(dp.ProductId))
                .ToList();

            _context.DepartmentProducts.RemoveRange(itemsToDelete);

            // ADD or UPDATE
            foreach (var dto in dtoList)
            {
                var product = products[dto.ProductUuid!.Value];

                var existingItem = existingItems.FirstOrDefault(
                    dp => dp.ProductId == product.Id);

                if (existingItem == null)
                {
                    // ADD new relationship
                    var newItem = new DepartmentProduct
                    {
                        DepartmentId = department.Id,
                        ProductId = product.Id,
                        Total = dto.Total
                    };

                    _context.DepartmentProducts.Add(newItem);
                }
                else
                {
                    // UPDATE existing quantity
                    existingItem.Total = dto.Total;
                }
            }

            // Save all changes
            await _context.SaveChangesAsync();
        }

        public async Task SyncProductDepartmentsAsync(
    Guid productUuid,
    ICollection<DepartmentProductDto> dtoList)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Uuid == productUuid);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (dtoList.Any(x => !x.DepartmentUuid.HasValue))
                throw new ArgumentException("DepartmentUuid is required.");

            if (dtoList.Any(x => x.Total < 0))
                throw new ArgumentException("Total cannot be negative.");

            var departmentUuids = dtoList
                .Select(x => x.DepartmentUuid!.Value)
                .ToList();

            if (departmentUuids.Distinct().Count() != departmentUuids.Count)
                throw new ArgumentException(
                    "A department cannot appear more than once.");

            var departments = await _context.Departments
                .Where(d => departmentUuids.Contains(d.Uuid))
                .ToDictionaryAsync(d => d.Uuid);

            if (departments.Count != departmentUuids.Distinct().Count())
                throw new KeyNotFoundException(
                    "One or more departments were not found.");

            var existingItems = await _context.DepartmentProducts
                .Where(dp => dp.ProductId == product.Id)
                .ToListAsync();

            var submittedDepartmentIds = departments.Values
                .Select(d => d.Id)
                .ToHashSet();

            // DELETE assignments removed from the submitted list
            var itemsToDelete = existingItems
                .Where(dp => !submittedDepartmentIds.Contains(dp.DepartmentId))
                .ToList();

            _context.DepartmentProducts.RemoveRange(itemsToDelete);

            // ADD or UPDATE assignments
            foreach (var item in dtoList)
            {
                var department = departments[item.DepartmentUuid!.Value];

                var existing = existingItems.FirstOrDefault(
                    dp => dp.DepartmentId == department.Id);

                if (existing == null)
                {
                    _context.DepartmentProducts.Add(new DepartmentProduct
                    {
                        DepartmentId = department.Id,
                        ProductId = product.Id,
                        Total = item.Total
                    });
                }
                else
                {
                    existing.Total = item.Total;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}