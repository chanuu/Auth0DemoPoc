namespace Auth0Demo.Application.Roles.Dtos
{
    public class AssignRoleDto
    {
        public string UserId { get; set; }  // Auth0 User ID
        public string RoleId { get; set; }  // Auth0 Role ID
    }
}
