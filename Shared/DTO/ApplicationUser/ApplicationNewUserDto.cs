using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTO.ApplicationUser
{
    public class ApplicationNewUserDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordCheck { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
