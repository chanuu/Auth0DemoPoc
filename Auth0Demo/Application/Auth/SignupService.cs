using Auth0.ManagementApi.Models;
using Auth0Demo.Core.Identity;
using Auth0Demo.Core.Identity.Auth0.Dtos;
using Auth0Demo.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Application.Auth
{
    public class SignupService :ISignupService
    {
        private readonly IAuth0Service _auth0Service;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SignupService> _logger;

        public SignupService(
            IAuth0Service auth0Service,
            ApplicationDbContext dbContext,
            ILogger<SignupService> logger)
        {
            _auth0Service = auth0Service;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<SignupResultDto> SignupUserAsync(SignupRequestDto request)
        {
            ValidateRequest(request);
            await CheckEmailUniqueness(request.Email);

            try
            {
                var auth0User = await CreateAuth0User(request);
                var localUser = await CreateLocalUser(auth0User, request);
                await UpdateAuth0Metadata(auth0User.UserId,new Guid(localUser.Id));

                return MapToResultDto(auth0User, localUser, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User registration failed for {Email}", request.Email);
                throw new ApplicationException("User registration failed. Please try again.", ex);
            }
        }

        // Helper methods
        private void ValidateRequest(SignupRequestDto request)
        {
            var validationContext = new ValidationContext(request);
            Validator.ValidateObject(request, validationContext, validateAllProperties: true);
        }

        private async Task CheckEmailUniqueness(string email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == email))
            {
                throw new ValidationException("Email address is already registered");
            }
        }

        private async Task<User> CreateAuth0User(SignupRequestDto request)
        {
            var auth0UserRequest = new UserCreateRequest
            {
                Connection = "Username-Password-Authentication",
                Email = request.Email,
                
                UserName = request.UserName,
                Password = request.Password,
                EmailVerified = false,
                UserMetadata = new
                {
                    request.FullName,
                    request.PhoneNumber,
                    request.CompanyName,
                    SignupSource = "WebApplication"
                }
            };

            return await _auth0Service.CreateAuth0UserAsync(auth0UserRequest);
        }

        private async Task<ApplicationUser> CreateLocalUser(User auth0User, SignupRequestDto request)
        {
            var localUser = new ApplicationUser
            {
                Auth0Id = auth0User.UserId,
                Email = auth0User.Email,
                Name = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            await _dbContext.Users.AddAsync(localUser);
            await _dbContext.SaveChangesAsync();

            return localUser;
        }

        private async Task UpdateAuth0Metadata(string auth0UserId, Guid localUserId)
        {
            await _auth0Service.UpdateAuth0UserMetadata(auth0UserId, new
            {
                LocalUserId = localUserId,
                DbSyncVersion = 1,
                LastSync = DateTime.UtcNow
            });

          
        }

        private SignupResultDto MapToResultDto(User auth0User, ApplicationUser localUser, SignupRequestDto request)
        {
            return new SignupResultDto
            {
                Auth0UserId = auth0User.UserId,
                LocalUserId = localUser.Id,
                Email = auth0User.Email,
                EmailVerified = auth0User.EmailVerified ?? false,
                Metadata = new Dictionary<string, object>
        {
            { "FullName", request.FullName },
            { "PhoneNumber", request.PhoneNumber },
            { "CompanyName", request.CompanyName }
        }
            };
        }
    }
}
