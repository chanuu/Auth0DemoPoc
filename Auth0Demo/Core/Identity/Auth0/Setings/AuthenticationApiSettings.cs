using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Core.Identity.Auth0.Setings
{
    public class AuthenticationApiSettings
    {
        [Required]
        public string ClientId { get; set; } = null!;

        [Required]
        public string ClientSecret { get; set; } = null!;
    }
}
