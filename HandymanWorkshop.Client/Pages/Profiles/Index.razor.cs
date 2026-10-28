
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTO.ApplicationUser;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace HandymanWorkshop.Client.Pages.Profiles
{
    public partial class Index
    {
        private List<ApplicationUserDto> profiles = new();
        private bool isLoading = true;
        private bool isWorking;
        private string? errorMessage;
        private string? successMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadProfiles();
        }

        private async Task LoadProfiles()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                profiles = await Http.GetFromJsonAsync<
                    List<ApplicationUserDto>>("api/Profiles")
                    ?? new List<ApplicationUserDto>();
            }
            catch (Exception)
            {
                errorMessage = "Could not load profiles.";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ResendPassword(
            ApplicationUserDto profile)
        {
            errorMessage = null;
            successMessage = null;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Send a new password to {profile.Email}?");

            if (!confirmed)
                return;

            isWorking = true;

            try
            {
                var response = await Http.PostAsync(
                    $"api/Profiles/{Uri.EscapeDataString(profile.Id)}/resend-password",
                    null);

                if (response.IsSuccessStatusCode)
                {
                    successMessage =
                        $"Password operation completed for {profile.Email}.";
                }
                else
                {
                    errorMessage =
                        await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(errorMessage))
                        errorMessage = "Could not send a new password.";
                }
            }
            catch (Exception)
            {
                errorMessage = "An error occurred while processing the request.";
            }
            finally
            {
                isWorking = false;
            }
        }

        private async Task DeleteProfile(
            ApplicationUserDto profile)
        {
            errorMessage = null;
            successMessage = null;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Are you sure you want to delete {profile.Email}?");

            if (!confirmed)
                return;

            isWorking = true;

            try
            {
                var response = await Http.DeleteAsync(
                    $"api/Profiles/{Uri.EscapeDataString(profile.Id)}");

                if (response.IsSuccessStatusCode)
                {
                    profiles.RemoveAll(
                        p => p.Id == profile.Id);

                    successMessage =
                        $"{profile.Email} was deleted.";
                }
                else
                {
                    errorMessage =
                        await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(errorMessage))
                        errorMessage = "Could not delete the profile.";
                }
            }
            catch (Exception)
            {
                errorMessage = "An error occurred while deleting the profile.";
            }
            finally
            {
                isWorking = false;
            }
        }
    }
}
