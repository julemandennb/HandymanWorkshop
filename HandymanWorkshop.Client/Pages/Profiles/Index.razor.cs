
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


        private string searchTerm = string.Empty;
        private string sortBy = "Name";
        private bool sortDescending;

        private List<ApplicationUserDto> FilteredProfiles
        {
            get
            {
                IEnumerable<ApplicationUserDto> query = profiles;

                // Search by name, email, role or ID
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var search = searchTerm.Trim();

                    query = query.Where(p =>
                        (p.Name?.Contains(
                            search, StringComparison.OrdinalIgnoreCase) ?? false)
                        ||
                        (p.Email?.Contains(
                            search, StringComparison.OrdinalIgnoreCase) ?? false)
                        ||
                        (p.Role?.Contains(
                            search, StringComparison.OrdinalIgnoreCase) ?? false)
                        ||
                        (p.Id?.Contains(
                            search, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                // Sort profiles
                switch (sortBy)
                {
                    case "Email":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Email)
                            : query.OrderBy(p => p.Email);
                        break;

                    case "Role":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Role)
                            : query.OrderBy(p => p.Role);
                        break;

                    case "Id":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Id)
                            : query.OrderBy(p => p.Id);
                        break;

                    case "Name":
                    default:
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                }

                return query.ToList();
            }
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            sortBy = "Name";
            sortDescending = false;
        }

    }
}
