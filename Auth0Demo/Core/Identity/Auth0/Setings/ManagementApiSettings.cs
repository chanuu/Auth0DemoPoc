using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Core.Identity.Auth0.Setings
{
    public class ManagementApiSettings
    {
        [Required]
        public string ClientId { get; set; } = null!;

        [Required]
        public string ClientSecret { get; set; } = null!;

        [Required]
        [Url]
        public string Audience { get; set; } = null!;
    }
}
