using HandymanWorkshop.Data;
using HandymanWorkshop.Help.WorkOrderProducts;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Pivot;

namespace HandymanWorkshop.Services
{
    public class WorkOrderProductService : IWorkOrderProductService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<WorkOrderProductDto>> GetAllAsync()
        {
            var items = await _context.WorkOrderProducts
                .AsNoTracking()
                .Include(x => x.WorkOrder)
                .Include(x => x.Product)
                .Include(x => x.AddedByUser)
                .ToListAsync();

            return items.Select(WorkOrderProductToDto.ToDto).ToList();
        }

        // GET BY UUID
        public async Task<WorkOrderProductDto?> GetByUuidAsync(Guid uuid)
        {
            var item = await _context.WorkOrderProducts
                .AsNoTracking()
                .Include(x => x.WorkOrder)
                .Include(x => x.Product)
                .Include(x => x.AddedByUser)
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            return item == null
                ? null
                : WorkOrderProductToDto.ToDto(item);
        }

        // GET ALL PRODUCTS FROM A WORK ORDER
        public async Task<IEnumerable<Product>>
            GetAllProductsFromWorkOrderAsync(Guid workOrderUuid)
        {
            var workOrder = await _context.WorkOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == workOrderUuid);

            if (workOrder == null)
                return Enumerable.Empty<Product>();

            return await _context.WorkOrderProducts
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrder.Id)
                .Select(x => x.Product)
                .ToListAsync();
        }

        // GET ALL WORK ORDERS FROM A PRODUCT
        public async Task<IEnumerable<WorkOrder>>
            GetAllWorkOrdersFromProductAsync(Guid productUuid)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == productUuid);

            if (product == null)
                return Enumerable.Empty<WorkOrder>();

            return await _context.WorkOrderProducts
                .AsNoTracking()
                .Where(x => x.ProductId == product.Id)
                .Select(x => x.WorkOrder)
                .ToListAsync();
        }

        // ADD, UPDATE AND DELETE PRODUCTS FOR A WORK ORDER
        public async Task SyncAsync(
         Guid workOrderUuid,
         ICollection<WorkOrderProductDto> dtoList,
         string addedByUserId)
        {
            var workOrder = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.Uuid == workOrderUuid);

            if (workOrder == null)
                throw new KeyNotFoundException("Work order not found.");

            if (string.IsNullOrWhiteSpace(addedByUserId))
                throw new ArgumentException("User ID is required.");

            if (dtoList.Any(x => x.Quantity < 1))
                throw new ArgumentException(
                    "Product quantity must be at least 1.");

            if (dtoList.Any(x => !x.ProductUuid.HasValue))
                throw new ArgumentException(
                    "ProductUuid is required for every product.");

            var productUuids = dtoList
                .Select(x => x.ProductUuid!.Value)
                .ToList();

            if (productUuids.Distinct().Count() != productUuids.Count)
                throw new ArgumentException(
                    "A product cannot appear more than once.");

            var products = await _context.Products
                .Where(x => productUuids.Contains(x.Uuid))
                .ToDictionaryAsync(x => x.Uuid);

            if (products.Count != productUuids.Count)
                throw new KeyNotFoundException(
                    "One or more products were not found.");

            var existingItems = await _context.WorkOrderProducts
                .Where(x => x.WorkOrderId == workOrder.Id)
                .ToListAsync();

            var productIdsToKeep = products.Values
                .Select(x => x.Id)
                .ToHashSet();

            // Delete removed products.
            var itemsToDelete = existingItems
                .Where(x => !productIdsToKeep.Contains(x.ProductId))
                .ToList();

            _context.WorkOrderProducts.RemoveRange(itemsToDelete);

            // Add or update products.
            foreach (var dto in dtoList)
            {
                var product = products[dto.ProductUuid!.Value];

                // Use the product's database price if UnitPrice is not set.
                var unitPrice = dto.UnitPrice > 0
                    ? dto.UnitPrice
                    : product.Price;

                if (unitPrice < 0.01m)
                    throw new ArgumentException(
                        $"Product '{product.Name}' must have a unit price of at least 0.01.");

                var existing = existingItems.FirstOrDefault(
                    x => x.ProductId == product.Id);

                if (existing == null)
                {
                    _context.WorkOrderProducts.Add(new WorkOrderProduct
                    {
                        WorkOrderId = workOrder.Id,
                        ProductId = product.Id,
                        Quantity = dto.Quantity,
                        UnitPrice = unitPrice,
                        AddedByUserId = addedByUserId
                    });
                }
                else
                {
                    existing.Quantity = dto.Quantity;
                    existing.UnitPrice = unitPrice;

                    // Keep the original AddedByUserId.
                }
            }

            await _context.SaveChangesAsync();
        }

        // ADD, UPDATE AND DELETE WORK ORDERS FOR A PRODUCT
        public async Task SyncProductWorkOrdersAsync(
            Guid productUuid,
            ICollection<WorkOrderProductDto> dtoList,
            string addedByUserId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Uuid == productUuid);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (string.IsNullOrWhiteSpace(addedByUserId))
                throw new ArgumentException("User ID is required.");

            if (dtoList.Any(x => !x.WorkOrderUuid.HasValue))
                throw new ArgumentException(
                    "WorkOrderUuid is required for every item.");

            if (dtoList.Any(x => x.Quantity < 1))
                throw new ArgumentException(
                    "Product quantity must be at least 1.");

            if (dtoList.Any(x => x.UnitPrice < 0.01m))
                throw new ArgumentException(
                    "Unit price must be at least 0.01.");

            var workOrderUuids = dtoList
                .Select(x => x.WorkOrderUuid!.Value)
                .ToList();

            if (workOrderUuids.Distinct().Count() != workOrderUuids.Count)
                throw new ArgumentException(
                    "A work order cannot appear more than once.");

            var workOrders = await _context.WorkOrders
                .Where(x => workOrderUuids.Contains(x.Uuid))
                .ToDictionaryAsync(x => x.Uuid);

            if (workOrders.Count != workOrderUuids.Count)
                throw new KeyNotFoundException(
                    "One or more work orders were not found.");

            var existingItems = await _context.WorkOrderProducts
                .Where(x => x.ProductId == product.Id)
                .ToListAsync();

            var workOrderIdsToKeep = workOrders.Values
                .Select(x => x.Id)
                .ToHashSet();

            // DELETE removed work orders
            var itemsToDelete = existingItems
                .Where(x => !workOrderIdsToKeep.Contains(x.WorkOrderId))
                .ToList();

            _context.WorkOrderProducts.RemoveRange(itemsToDelete);

            // ADD OR UPDATE work orders
            foreach (var dto in dtoList)
            {
                var workOrder = workOrders[dto.WorkOrderUuid!.Value];

                var existing = existingItems.FirstOrDefault(
                    x => x.WorkOrderId == workOrder.Id);

                if (existing == null)
                {
                    _context.WorkOrderProducts.Add(new WorkOrderProduct
                    {
                        WorkOrderId = workOrder.Id,
                        ProductId = product.Id,
                        Quantity = dto.Quantity,
                        UnitPrice = dto.UnitPrice,
                        AddedByUserId = addedByUserId
                    });
                }
                else
                {
                    existing.Quantity = dto.Quantity;
                    existing.UnitPrice = dto.UnitPrice;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}