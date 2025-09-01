namespace Auth0Demo.Core.Identity.Auth0.Dtos
{
    // Models/DTOs/Auth0UserResponse.cs
    public class Auth0UserResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public Dictionary<string, object> UserMetadata { get; set; }
    }
}
