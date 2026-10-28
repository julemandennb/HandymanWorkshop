using Microsoft.AspNetCore.Components;
using Shared.DTO.WorkOrder;
using System.Net.Http.Json;

namespace HandymanWorkshop.Client.Pages.WorkOrder
{
    public partial class Delete
    {
        [Parameter]
        public string id { get; set; } = string.Empty;

        private WorkOrderDto? workOrder;

        private bool isDeleting;
        private string? errorMessage;

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Inject]
        protected NavigationManager NavMan { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                workOrder = await Http.GetFromJsonAsync<WorkOrderDto>(
                    $"api/WorkOrders/{id}");
            }
            catch (HttpRequestException)
            {
                errorMessage = "Unable to load the work order.";
            }
            catch (System.Text.Json.JsonException)
            {
                errorMessage = "The work order response could not be read.";
            }
        }

        protected async Task DeleteWorkOrder()
        {
            if (isDeleting)
                return;

            isDeleting = true;
            errorMessage = null;

            try
            {
                var response = await Http.DeleteAsync(
                    $"api/WorkOrders/{id}");

                if (response.IsSuccessStatusCode)
                {
                    NavMan.NavigateTo("/WorkOrder");
                }
                else
                {
                    errorMessage =
                        $"Unable to delete work order. {response.StatusCode}: " +
                        await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException)
            {
                errorMessage = "A network error occurred while deleting the work order.";
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}