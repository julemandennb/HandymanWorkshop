using Microsoft.AspNetCore.Identity;

namespace HandymanWorkshop.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }

}
