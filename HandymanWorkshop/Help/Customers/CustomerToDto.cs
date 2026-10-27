using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Help.WorkOrders;
using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Customer;

namespace HandymanWorkshop.Help.Customers
{
    internal static class CustomerToDto
    {
        internal static CustomerDto ToDto(Customer customer)
        {
            return new CustomerDto
            {
                Uuid = customer.Uuid,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber ?? "",
                Address = customer.Address,
                Postcode = customer.Postcode,
                WorkOrders = null

            };
        }

        // Detailed Customer information
        internal static CustomerDto ToDetailedDto(Customer customer)
        {
            return ToAddDto(
                customer,
                addWorkOrders: true);
        }

        internal static CustomerDto ToAddDto(
            Customer customer,
            bool addWorkOrders = false)
        {
            return new CustomerDto
            {
                Uuid = customer.Uuid,
                Name = customer.Name,

                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber ?? "",
                Address = customer.Address,
                Postcode = customer.Postcode,
                WorkOrders = addWorkOrders && customer.WorkOrders != null
                    ? customer.WorkOrders
                        .Select(WorkOrderToDto.ToDto)
                        .ToList()
                    : null


            };
        }

        internal static SelectOptionDto ToSelectOptionDto(Customer customer)
        {
            return new SelectOptionDto
            {
                Text = customer.Name ?? "",
                Value = customer.Uuid.ToString()
            };
        }
    }
}
