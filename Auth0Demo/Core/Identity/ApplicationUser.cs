using Microsoft.AspNetCore.Identity;

namespace Auth0Demo.Core.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Auth0Id { get; set; }
        public string Name { get; set; }
    }
}
