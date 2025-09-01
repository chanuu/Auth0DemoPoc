using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0Demo.Core.Identity.Auth0.Setings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Auth0Demo.Application.Auth
{
    public class Auth0Service : IAuth0Service
    {

        private readonly Auth0Settings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IManagementConnection _managementConnection;
        private readonly Uri _domainUri;
        private bool _disposed;
        private readonly ILogger<Auth0Service> _logger;

        public Auth0Service(IOptions<Auth0Settings> settings, IHttpClientFactory httpClientFactory, ILogger<Auth0Service> logger)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            // Validate and convert domain to Uri
            if (string.IsNullOrWhiteSpace(_settings.Domain))
            {
                throw new ArgumentException("Auth0 Domain must be configured");
            }

            try
            {
                _domainUri = new Uri($"https://{_settings.Domain.Trim()}");
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("Invalid Auth0 Domain format", ex);
            }

            // Create the management connection
            _managementConnection = new HttpClientManagementConnection(
                httpClientFactory.CreateClient("Auth0ManagementApi"));
            _logger = logger;
        }


        public async Task<User> CreateAuth0UserAsync(UserCreateRequest userRequest)
        {
            try
            {
                
                var managementClient = await GetManagementApiClient();
                return await managementClient.Users.CreateAsync(userRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Auth0 user for email {Email}", userRequest.Email);
                throw;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                //_managementConnection?.Dispose();
                _disposed = true;
            }
        }

        public async Task<ManagementApiClient> GetManagementApiClient()
        {
            try
            {
                var auth0Client = new AuthenticationApiClient(_domainUri);

                var token = await auth0Client.GetTokenAsync(new ClientCredentialsTokenRequest
                {
                    ClientId = _settings.ManagementApi.ClientId,
                    ClientSecret = _settings.ManagementApi.ClientSecret,
                    Audience = _settings.ManagementApi.Audience,

                });

                var managementApiUri = new Uri($"{_domainUri}api/v2/");

                return new ManagementApiClient(token.AccessToken, managementApiUri, _managementConnection);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialize Management API client", ex);
            }
        }

        public async Task UpdateAuth0UserMetadata(string userId, object metadata)
        {
            try
            {
                var managementClient = await GetManagementApiClient();
                await managementClient.Users.UpdateAsync(userId, new UserUpdateRequest
                {
                    UserMetadata = metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update metadata for user {UserId}", userId);
                throw;
            }
        }
    }
}
