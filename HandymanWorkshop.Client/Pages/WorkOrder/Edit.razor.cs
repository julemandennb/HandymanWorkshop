using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Pivot;
using Shared.DTO.Product;
using Shared.DTO.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HandymanWorkshop.Client.Pages.WorkOrder
{
    public partial class Edit
    {
        [Parameter]
        public string id { get; set; } = string.Empty;

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Inject]
        protected NavigationManager NavMan { get; set; } = default!;

        private WorkOrderDto? workOrder;

        private List<SelectOptionDto> ProductOptions { get; set; } = new();

        private List<EditableWorkOrderProduct> SelectedProducts { get; set; } = new();

        private Guid SelectedProductUuid { get; set; } = Guid.Empty;

        private bool isLoading = true;
        private bool isLoadingProducts;
        private bool isSaving;

        private string ErrorMessage { get; set; } = string.Empty;

        private IEnumerable<SelectOptionDto> AvailableProductOptions =>
            ProductOptions.Where(option =>
                Guid.TryParse(option.Value, out var productUuid) &&
                !SelectedProducts.Any(
                    selected => selected.ProductUuid == productUuid));

        private decimal ProductTotal =>
            SelectedProducts.Sum(
                product => product.Quantity * product.UnitPrice);

        protected override async Task OnInitializedAsync()
        {
            try
            {
                workOrder = await Http.GetFromJsonAsync<WorkOrderDto>(
                    $"api/WorkOrders/{id}");

                if (workOrder == null)
                {
                    ErrorMessage = "Work order not found.";
                    return;
                }

                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not load work order: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadProductsAsync()
        {
            if (workOrder == null)
                return;

            isLoadingProducts = true;

            try
            {
                var departmentUuid = workOrder.Department?.Uuid;

                if (departmentUuid == null ||
                    departmentUuid == Guid.Empty)
                {
                    ProductOptions = new List<SelectOptionDto>();
                }
                else
                {
                    ProductOptions =
                        await Http.GetFromJsonAsync<List<SelectOptionDto>>(
                            $"api/Products/getSelectoptionByDepartment/{departmentUuid}")
                        ?? new List<SelectOptionDto>();
                }

                SelectedProducts = workOrder.WorkOrderProducts?
                    .Select(product => new EditableWorkOrderProduct
                    {
                        ProductUuid = product.ProductUuid,
                        ProductName = product.ProductName,
                        Quantity = product.Quantity,
                        UnitPrice = product.UnitPrice
                    })
                    .ToList()
                    ?? new List<EditableWorkOrderProduct>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not load products: {ex.Message}";
            }
            finally
            {
                isLoadingProducts = false;
            }
        }

        private async Task AddProduct()
        {
            ErrorMessage = string.Empty;

            if (SelectedProducts.Any(p => p.ProductUuid == SelectedProductUuid))
            {
                ErrorMessage = "This product has already been added.";
                return;
            }

            var option = ProductOptions.FirstOrDefault(
                p => p.Value == SelectedProductUuid.ToString());

            if (option == null)
            {
                ErrorMessage = "The selected product is not available.";
                return;
            }

            try
            {
                // This assumes GET api/Products/{uuid} returns a ProductDto
                // with a Price property. Adjust if your endpoint differs.
                var product = await Http.GetFromJsonAsync<ProductDto>(
                    $"api/Products/{SelectedProductUuid}");

                if (product == null)
                {
                    ErrorMessage = "Could not retrieve the product price.";
                    return;
                }

                SelectedProducts.Add(new EditableWorkOrderProduct
                {
                    ProductUuid = SelectedProductUuid,
                    ProductName = option.Text,
                    Quantity = 1,
                    UnitPrice = product.Price
                });

                SelectedProductUuid = Guid.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not add product: {ex.Message}";
            }
        }

        private void RemoveProduct(Guid? productUuid)
        {
            SelectedProducts.RemoveAll(
                product => product.ProductUuid == productUuid);

            ErrorMessage = string.Empty;
        }

        private async Task SaveChanges()
        {
            if (workOrder == null || isSaving)
                return;

            ErrorMessage = string.Empty;

            if (SelectedProducts.Any(product => product.Quantity < 1))
            {
                ErrorMessage = "Every product quantity must be at least 1.";
                return;
            }

            try
            {
                isSaving = true;

                workOrder.WorkOrderProducts = SelectedProducts
                    .Select(product => new WorkOrderProductDto
                    {
                        WorkOrderUuid = workOrder.Uuid,
                        ProductUuid = product.ProductUuid,
                        ProductName = product.ProductName,
                        Quantity = product.Quantity,
                        UnitPrice = product.UnitPrice
                    })
                    .ToList();

                var response = await Http.PutAsJsonAsync(
                    $"api/WorkOrders/{id}",
                    workOrder);

                if (response.IsSuccessStatusCode)
                {
                    NavMan.NavigateTo($"/WorkOrder/Details/{id}");
                }
                else
                {
                    var details = await response.Content.ReadAsStringAsync();

                    ErrorMessage =
                        $"Could not update work order. " +
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

    public class EditableWorkOrderProduct
    {
        public Guid? ProductUuid { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }
    }
}
