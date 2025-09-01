using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Core.Identity.Auth0.Setings
{
    public class Auth0Settings
    {
        [Required(ErrorMessage = "Auth0 Domain is required")]
        [Url(ErrorMessage = "Auth0 Domain must be a valid URL")]
        public string Domain { get; set; } = null!;

        [Required]
        public ManagementApiSettings ManagementApi { get; set; } = new();

        [Required]
        public AuthenticationApiSettings AuthenticationApi { get; set; } = new();
    }
}
