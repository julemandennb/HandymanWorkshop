
using Microsoft.AspNetCore.Components;
using Shared.DTO.WorkOrder;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HandymanWorkshop.Client.Pages.WorkOrder
{
    public partial class Details
    {
        [Parameter]
        public string id { get; set; } = string.Empty;

        private WorkOrderDto? workOrder;

        [Inject]
        protected NavigationManager? NavMan { get; set; }

        protected decimal price { get; set; }

        protected decimal productTotal =>
            workOrder?.WorkOrderProducts?.Sum(
                item => item.Quantity * item.UnitPrice) ?? 0m;

        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                workOrder = await Http.GetFromJsonAsync<WorkOrderDto>(
                    $"api/WorkOrders/{id}");

                if (workOrder != null)
                {
                    price = workOrder.HourlyRate * workOrder.EstimatedHours;
                }
            }
            catch (HttpRequestException)
            {
                workOrder = null;
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
