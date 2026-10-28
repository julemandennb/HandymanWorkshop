using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Customer;
using Shared.DTO.Departments;
using Shared.DTO.Employee;
using Shared.DTO.WorkOrder;
using Shared.DTO.Pivot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HandymanWorkshop.Client.Pages.WorkOrder
{
    public partial class Create
    {
        private WorkOrderDto workOrder { get; set; } = new WorkOrderDto();

        private List<SelectOptionDto>? DepartmentOptions { get; set; }
            = new List<SelectOptionDto>();

        private List<SelectOptionDto>? CustomerOptions { get; set; }
            = new List<SelectOptionDto>();

        private List<SelectOptionDto>? EmployeeOptions { get; set; }
            = new List<SelectOptionDto>();

        private List<SelectOptionDto>? ProductOptions { get; set; }
            = new List<SelectOptionDto>();

        private List<SelectedWorkOrderProduct> SelectedProducts { get; set; }
            = new List<SelectedWorkOrderProduct>();

        private string SelectedProductUuid { get; set; } = string.Empty;

        private bool isLoadingProducts { get; set; }

        private bool isSaving { get; set; }

        private string ErrorMessage { get; set; } = string.Empty;

        [Inject]
        protected NavigationManager NavMan { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                DepartmentOptions =
                    await Http.GetFromJsonAsync<List<SelectOptionDto>>(
                        "api/Departments/getselectoption")
                    ?? new List<SelectOptionDto>();

                CustomerOptions =
                    await Http.GetFromJsonAsync<List<SelectOptionDto>>(
                        "api/Customers/getselectoption")
                    ?? new List<SelectOptionDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not load form options: {ex.Message}";
            }
        }

        protected async Task GetItemFromDepartment()
        {
            // Reset dependent selections when the department changes.
            EmployeeOptions = new List<SelectOptionDto>();
            ProductOptions = new List<SelectOptionDto>();
            SelectedProducts.Clear();
            SelectedProductUuid = string.Empty;

            if (workOrder.Department == null ||
                workOrder.Department.Uuid == Guid.Empty)
            {
                return;
            }

            isLoadingProducts = true;
            ErrorMessage = string.Empty;

            try
            {
                var departmentUuid = workOrder.Department.Uuid;

                EmployeeOptions =
                    await Http.GetFromJsonAsync<List<SelectOptionDto>>(
                        $"api/Employees/getSelectoptionByDepartment/{departmentUuid}")
                    ?? new List<SelectOptionDto>();

                ProductOptions =
                    await Http.GetFromJsonAsync<List<SelectOptionDto>>(
                        $"api/Products/getSelectoptionByDepartment/{departmentUuid}")
                    ?? new List<SelectOptionDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    $"Could not load employees or products: {ex.Message}";
            }
            finally
            {
                isLoadingProducts = false;
            }
        }

        private IEnumerable<SelectOptionDto> AvailableProductOptions =>
            (ProductOptions ?? new List<SelectOptionDto>())
                .Where(option =>
                    Guid.TryParse(option.Value, out var productUuid) &&
                    !SelectedProducts.Any(
                        selected => selected.ProductUuid == productUuid));

        private void AddProduct()
        {
            if (!Guid.TryParse(SelectedProductUuid, out var productUuid))
            {
                ErrorMessage = "Please select a valid product.";
                return;
            }

            var option = ProductOptions?.FirstOrDefault(
                p => p.Value == SelectedProductUuid);

            if (option == null)
            {
                ErrorMessage = "The selected product is not available.";
                return;
            }

            if (SelectedProducts.Any(p => p.ProductUuid == productUuid))
            {
                ErrorMessage = "This product has already been added.";
                return;
            }

            SelectedProducts.Add(new SelectedWorkOrderProduct
            {
                ProductUuid = productUuid,
                ProductName = option.Text,
                Quantity = 1
            });

            SelectedProductUuid = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void RemoveProduct(Guid productUuid)
        {
            SelectedProducts.RemoveAll(
                p => p.ProductUuid == productUuid);
        }

        protected async Task AddWorkOrder()
        {
            if (isSaving)
            {
                return;
            }

            ErrorMessage = string.Empty;

            if (SelectedProducts.Any(p => p.Quantity < 1))
            {
                ErrorMessage =
                    "Every selected product must have a quantity of at least 1.";
                return;
            }

            try
            {
                isSaving = true;

                workOrder.WorkOrderProducts = SelectedProducts
                    .Select(p => new WorkOrderProductDto
                    {
                        ProductUuid = p.ProductUuid,
                        Quantity = p.Quantity
                    })
                    .ToList();

                var response = await Http.PostAsJsonAsync(
                    "api/WorkOrders", workOrder);

                if (response.IsSuccessStatusCode)
                {
                    NavMan.NavigateTo("/WorkOrder");
                }
                else
                {
                    var details =
                        await response.Content.ReadAsStringAsync();

                    ErrorMessage =
                        $"Could not create work order. " +
                        $"Status: {(int)response.StatusCode}. {details}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }
    }

    public class SelectedWorkOrderProduct
    {
        public Guid ProductUuid { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;
    }
}

