using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Employees;
using HandymanWorkshop.Help.Products;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Pivot;
using Shared.DTO.Product;

namespace HandymanWorkshop.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        private readonly IDepartmentProductService _departmentProductService;

        public ProductService(ApplicationDbContext context,
            IDepartmentProductService departmentProductService)
        {
            _context = context;
            _departmentProductService = departmentProductService;
        }

        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (dto.DepartmentProducts != null)
            {
                await _departmentProductService.SyncProductDepartmentsAsync(
                    product.Uuid,
                    dto.DepartmentProducts);
            }

            return ProductToDto.ToDto(product);
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var product = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            return product
                .Select(ProductToDto.ToDto)
                .ToList();
        }

        public async Task<ProductDto?> GetByUuidAsync(Guid uuid)
        {
            var product = await _context.Products
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return product == null
                ? null
                : ProductToDto.ToDto(product);
        }

        public async Task<ProductDto?> GetDetailedByUuidAsync(Guid uuid)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(d => d.DepartmentProducts).ThenInclude(de => de.Department)
                .Include(d => d.WorkOrderProducts).ThenInclude(de => de.WorkOrder)
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return product == null
                ? null
                : ProductToDto.ToDetailedDto(product);
        }

        public async Task<Product?> GetModelByUuidAsync(Guid uuid)
        {
            var product = await _context.Products
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return product == null
                ? null
                : product;
        }

        public async Task<List<SelectOptionDto>> GetSelectOption()
        {
            var product = await _context.Products
               .AsNoTracking()
               .ToListAsync();

            return product
                .Select(ProductToDto.ToSelectOptionDto)
                .ToList();
        }

        public async Task<List<SelectOptionDto>> getSelectoptionByDepartment(Guid uuid)
        {
            IEnumerable<Product> products = await _departmentProductService.GetAllProductFromDepartmentModelAsync(uuid);

            return products
              .Select(ProductToDto.ToSelectOptionDto)
              .ToList();
        }

        public async Task<bool> UpdateAsync(Guid uuid, ProductDto dto)
        {
            var product = await _context.Products
            .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (product == null)
                return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;

            
            await _context.SaveChangesAsync();

            await _departmentProductService.SyncProductDepartmentsAsync(
                uuid,
                dto.DepartmentProducts ?? new List<DepartmentProductDto>());

            return true;
        }
    }
}
