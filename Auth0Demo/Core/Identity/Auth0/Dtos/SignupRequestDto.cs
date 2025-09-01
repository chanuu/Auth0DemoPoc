using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Core.Identity.Auth0.Dtos
{
    public class SignupRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        // Add other custom fields with validation as needed
        [StringLength(100)]
        public string CompanyName { get; set; }

        public string  UserName { get; set; }
    }
}
