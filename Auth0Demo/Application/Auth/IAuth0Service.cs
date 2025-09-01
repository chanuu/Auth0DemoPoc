using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;

namespace Auth0Demo.Application.Auth
{
    public interface IAuth0Service
    {
        Task<ManagementApiClient> GetManagementApiClient();

        Task<User> CreateAuth0UserAsync(UserCreateRequest userRequest);

        Task UpdateAuth0UserMetadata(string userId, object metadata);
    }
}
