using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Customers;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Customer;

namespace HandymanWorkshop.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly ApplicationDbContext _context;

        private readonly IApplicationUserService _applicationUserService;


        public CustomersService(ApplicationDbContext context, IApplicationUserService applicationUserService)
        {
            _context = context;
            _applicationUserService = applicationUserService;
        }

        public async Task<List<CustomerDto>> ByApplicationUserId(string ApplicationUserId)
        {
            var customer = await _context.Customers
                .Where(c => c.ApplicationUserId == ApplicationUserId)
                  .AsNoTracking()
                  .ToListAsync();

            return customer
                .Select(CustomerToDto.ToDto)
                .ToList();
        }

        public async Task<CustomerDto> CreateAsync(CustomerDto dto)
        {
            ApplicationUser? user = await _applicationUserService.GetAsync(dto.Email);

            if (user == null)
                user = await _applicationUserService.CreateAsync(
                dto.Email,
                "Customer");

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Postcode = dto.Postcode,

                ApplicationUserId = user.Id
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return CustomerToDto.ToDto(customer);
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var customer = await _context.Customers
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customer = await _context.Customers
                  .AsNoTracking()
                  .ToListAsync();

            return customer
                .Select(CustomerToDto.ToDto)
                .ToList();
        }

        public async Task<CustomerDto?> GetByUuidAsync(Guid uuid)
        {
            var customer = await _context.Customers
              .AsNoTracking()
              .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return customer == null
                ? null
                : CustomerToDto.ToDto(customer);
        }

        public async Task<CustomerDto?> GetDetailedByUuidAsync(Guid uuid)
        {
            var customer = await _context.Customers
             .AsNoTracking()
             .Include(d => d.WorkOrders)
             .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return customer == null
                ? null
                : CustomerToDto.ToDetailedDto(customer);
        }

        public async Task<Customer?> GetModelByUuidAsync(Guid uuid)
        {
             var customer = await _context.Customers
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return customer == null
                ? null
                : customer;
        }

        public async Task<List<SelectOptionDto>> GetSelectOption()
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            return customer
                .Select(CustomerToDto.ToSelectOptionDto)
                .ToList();
        }

        public async Task<List<SelectOptionDto>> GetSelectOption(string ApplicationUserId)
        {
            var customer = await _context.Customers
               .Where(c => c.ApplicationUserId == ApplicationUserId)
                 .AsNoTracking()
                 .ToListAsync();

            return customer
              .Select(CustomerToDto.ToSelectOptionDto)
              .ToList();
        }

        public async Task<bool> UpdateAsync(Guid uuid, CustomerDto dto)
        {
            var customer = await _context.Customers
           .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (customer == null)
                return false;

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Address = dto.Address;
            customer.Postcode = dto.Postcode;


            await _context.SaveChangesAsync();

            return true;
        }
    }
}
