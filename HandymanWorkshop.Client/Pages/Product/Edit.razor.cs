using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Shared.DTO.Departments;
using Shared.DTO.Pivot;
using Shared.DTO.Product;

namespace HandymanWorkshop.Client.Pages.Product
{
    public partial class Edit
    {
        [Parameter]
        public Guid id { get; set; }

        [Inject]
        public HttpClient Http { get; set; } = default!;

        [Inject]
        public NavigationManager NavMan { get; set; } = default!;

        private ProductDto? product;

        private List<DepartmentProductDto> departmentStocks = new();

        private bool isLoading = true;
        private bool isSaving;

        private string? loadError;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            await LoadProduct();
        }

        private async Task LoadProduct()
        {
            isLoading = true;
            loadError = null;
            errorMessage = null;

            try
            {
                product = await Http.GetFromJsonAsync<ProductDto>(
                    $"api/Products/{id}");

                if (product == null)
                {
                    loadError = "Product not found.";
                    return;
                }

                var departments =
                    await Http.GetFromJsonAsync<List<DepartmentDto>>(
                        "api/Departments") ?? new();

                var existingAssignments = product.DepartmentProducts?
                    .Where(item => item.DepartmentUuid.HasValue)
                    .ToDictionary(
                        item => item.DepartmentUuid!.Value,
                        item => item);

                departmentStocks = departments
                    .Select(department =>
                    {
                        DepartmentProductDto? assignment = null;

                        var hasAssignment =
                            existingAssignments != null &&
                            existingAssignments.TryGetValue(
                                department.Uuid,
                                out assignment);

                        return new DepartmentProductDto
                        {
                            DepartmentUuid = department.Uuid,
                            DepartmentName = department.Name,
                            Selected = hasAssignment,
                            Total = hasAssignment
                                ? assignment!.Total
                                : 0
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                loadError = $"Could not load product: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task UpdateProduct()
        {
            if (isSaving || product == null)
                return;

            errorMessage = null;

            var selectedStocks = departmentStocks
                .Where(item => item.Selected)
                .ToList();

            if (selectedStocks.Any(item => item.Total < 0))
            {
                errorMessage =
                    "Department stock totals cannot be negative.";
                return;
            }

            product.DepartmentProducts = selectedStocks
                .Select(item => new DepartmentProductDto
                {
                    DepartmentUuid = item.DepartmentUuid,
                    Total = item.Total
                })
                .ToList();

            try
            {
                isSaving = true;

                var response = await Http.PutAsJsonAsync(
                    $"api/Products/{id}",
                    product);

                if (response.IsSuccessStatusCode)
                {
                    NavMan.NavigateTo("/Product");
                }
                else
                {
                    var responseMessage =
                        await response.Content.ReadAsStringAsync();

                    errorMessage = string.IsNullOrWhiteSpace(responseMessage)
                        ? $"Could not update product. Status: {(int)response.StatusCode}"
                        : responseMessage;
                }
            }
            catch (Exception ex)
            {
                errorMessage =
                    $"An error occurred while updating the product: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }
    }
}