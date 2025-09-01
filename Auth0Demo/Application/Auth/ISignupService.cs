using Auth0Demo.Core.Identity.Auth0.Dtos;

namespace Auth0Demo.Application.Auth
{
    public interface ISignupService
    {
        Task<SignupResultDto> SignupUserAsync(SignupRequestDto request);
    }
}
