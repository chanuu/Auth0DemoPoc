using Auth0Demo.Application.Roles.Dtos;

namespace Auth0Demo.Application.Roles
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(string name, string description);

        Task<IEnumerable<RoleDto>> GetAllRolesAsync();

        Task AssignRoleToUserAsync(AssignRoleDto request);


        Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId);


    }
}
