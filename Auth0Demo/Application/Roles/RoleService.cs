using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Auth0Demo.Application.Auth;
using Auth0Demo.Application.Roles.Dtos;

namespace Auth0Demo.Application.Roles
{
    public class RoleService : IRoleService
    {
        private IAuth0Service _auth0Service;

        private readonly ILogger<RoleService> _logger;

        public RoleService(IAuth0Service auth0Service, ILogger<RoleService> logger)
        {
            _auth0Service = auth0Service;
            _logger = logger;
        }

        public async Task<RoleDto> CreateRoleAsync(string name, string description)
        {
            try
            {
                var client = await _auth0Service.GetManagementApiClient();
                var role = await client.Roles.CreateAsync(new RoleCreateRequest
                {
                    Name = name,
                    Description = description
                });

                return new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create role {RoleName}", name);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var client = await _auth0Service.GetManagementApiClient();
                var roles = await client.Roles.GetAllAsync(new GetRolesRequest(), new PaginationInfo());

                return roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get roles");
                throw;
            }
        }

        public async Task AssignRoleToUserAsync(AssignRoleDto request)
        {
            try
            {
                var client = await _auth0Service.GetManagementApiClient();
                await client.Roles.AssignUsersAsync(request.RoleId, new AssignUsersRequest
                {
                    Users = new[] { request.UserId }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign role {RoleId} to user {UserId}",
                    request.RoleId, request.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId)
        {
            try
            {
                var client = await _auth0Service.GetManagementApiClient();
                var roles = await client.Users.GetRolesAsync(userId, new PaginationInfo());

                return roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get roles for user {UserId}", userId);
                throw;
            }
        }
    }
}
