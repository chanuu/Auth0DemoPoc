namespace Auth0Demo.Core.Identity.Auth0.Dtos
{
    public class SignupResultDto
    {
        public string Auth0UserId { get; set; }
        public string LocalUserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailVerified { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
