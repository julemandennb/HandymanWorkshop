using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Customers;
using HandymanWorkshop.Help.WorkOrders;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.WorkOrder;

namespace HandymanWorkshop.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;

        private readonly IApplicationUserService _applicationUserService;

        private readonly ICustomersService _customersService;
        private readonly IEmployeesService _employeesService;
        private readonly IDepartmentService _departmentService;

        public WorkOrderService(ApplicationDbContext context, 
            IApplicationUserService applicationUserService,
            ICustomersService customersService,
            IEmployeesService employeesService,
            IDepartmentService departmentService)
        {
            _context = context;
            _applicationUserService = applicationUserService;
            _customersService = customersService;
            _employeesService = employeesService;
            _departmentService = departmentService;
        }


        public async Task<List<WorkOrderDto>> ByApplicationUserId(string ApplicationUserId)
        {
            var workOrder = await _context.WorkOrders
                    //.Include(d => d.Employee)
                    //.Include(d => d.Customer)
                    .AsNoTracking()
                      .Where(w => (w.Customer.ApplicationUserId == ApplicationUserId) || (w.Employee.ApplicationUserId == ApplicationUserId))
                      .ToListAsync();

            return workOrder
                .Select(WorkOrderToDto.ToDto)
                .ToList();
        }

        public async Task<WorkOrderDto> CreateAsync(WorkOrderDto dto)
        {
            Customer? customer = await _customersService.GetModelByUuidAsync(dto.Customer.Uuid);
            if(customer == null)
            {
                //throw "ffff";
                return null;
            }

            Employee? employee = await _employeesService.GetModelByUuidAsync(dto.Employee.Uuid);
            if (employee == null)
            {
                //throw "ffff";
                return null;
            }

            Department? department = await _departmentService.GetModelByUuidAsync(dto.Department.Uuid);
            if (department == null)
            {
                //throw "ffff";
                return null;
            }

            var workOrder = new WorkOrder
            {
                Title = dto.Title,
                Description = dto.Description,
                ScheduledDate = dto.ScheduledDate,
                EstimatedHours = dto.EstimatedHours,
                HourlyRate = dto.HourlyRate,
                EmployeeId = employee.Id,
                DepartmentId = department.Id,
                CustomerId = customer.Id,
            };

            _context.WorkOrders.Add(workOrder);

            await _context.SaveChangesAsync();

            return WorkOrderToDto.ToDto(workOrder);

        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var workOrder = await _context.WorkOrders
              .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (workOrder == null)
                return false;

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<WorkOrderDto>> GetAllAsync()
        {
            var workOrder = await _context.WorkOrders
                 .AsNoTracking()
                 .ToListAsync();

            return workOrder
                .Select(WorkOrderToDto.ToDto)
                .ToList();
        }

        public async Task<WorkOrderDto?> GetByUuidAsync(Guid uuid)
        {
            var workOrder = await _context.WorkOrders
             .AsNoTracking()
             .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return workOrder == null
                ? null
                : WorkOrderToDto.ToDto(workOrder);
        }

        public async Task<WorkOrderDto?> GetDetailedByUuidAsync(Guid uuid)
        {
            var workOrder = await _context.WorkOrders
                .AsNoTracking()
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .Include(d => d.Customer)
                .Include(d => d.WorkOrderProducts)
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return workOrder == null
                ? null
                : WorkOrderToDto.ToDetailedDto(workOrder);
        }

        public async Task<WorkOrder?> GetModelByUuidAsync(Guid uuid)
        {
            var workOrder = await _context.WorkOrders
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return workOrder == null
                ? null
                : workOrder;
        }

        public async Task<bool> UpdateAsync(Guid uuid, WorkOrderDto dto)
        {
            Customer? customer = await _customersService.GetModelByUuidAsync(dto.Customer.Uuid);
            if (customer == null)
            {
                //throw "ffff";
                return false;
            }

            Employee? employee = await _employeesService.GetModelByUuidAsync(dto.Employee.Uuid);
            if (employee == null)
            {
                //throw "ffff";
                return false;
            }

            Department? department = await _departmentService.GetModelByUuidAsync(dto.Department.Uuid);
            if (department == null)
            {
                //throw "ffff";
                return false;
            }

            WorkOrder? workOrder = await GetModelByUuidAsync(uuid);

            if (workOrder == null)
                return false;


            workOrder.Title = dto.Title;
            workOrder.Description = dto.Description;
            workOrder.ScheduledDate = dto.ScheduledDate;
            workOrder.EstimatedHours = dto.EstimatedHours;
            workOrder.HourlyRate = dto.HourlyRate;
            workOrder.EmployeeId = employee.Id;
            workOrder.DepartmentId = department.Id;
            workOrder.CustomerId = customer.Id;

            await _context.SaveChangesAsync();

            return true;

        }
    }
}
