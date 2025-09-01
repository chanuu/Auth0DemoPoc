using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Auth0Demo.Application.Auth;
using Auth0Demo.Core.Identity.Auth0.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Auth0Demo.Controllers
{
    [ApiController]
    [Route("api/auth")]
    //[Authorize(Roles = "Punter")]
    public class AuthController : ControllerBase
    {
        private readonly ISignupService _signupService;
        private readonly IAuth0Service auth0Service;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ISignupService signupService,
            ILogger<AuthController> logger,
            IAuth0Service auth0Service)
        {
            _signupService = signupService;
            _logger = logger;
            this.auth0Service = auth0Service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SignupRequestDto request)
        {
            try
            {
                var result = await _signupService.SignupUserAsync(request);
                return Ok(new
                {
                    success = true,
                    userId = result.LocalUserId,
                    auth0Id = result.Auth0UserId,
                    email = result.Email,
                    verified = result.EmailVerified
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {

             var userIdClaim = User.Claims.FirstOrDefault(p => p.Type == "sub");
            // Log all claims for debugging
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            _logger.LogInformation("All claims: {@Claims}", claims);

            // Specifically log the sub and uid claims
            var subClaim = User.FindFirst("sub");
            var uidClaim = User.FindFirst("https://identity.testbetss.com/uid");

            _logger.LogInformation("sub claim: {Sub}", subClaim?.Value);
            _logger.LogInformation("uid claim: {Uid}", uidClaim?.Value);

            return Ok(new
            {
                UserId = User.FindFirst("sub")?.Value,
                OriginalUid = uidClaim?.Value,
                AllClaims = claims
            });
        }

       


        [HttpGet("test-auth0")]
        public async Task<IActionResult> TestAuth0()
        {
            var client = await this.auth0Service.GetManagementApiClient();




            // Get first page of users (you can add pagination parameters)
            var users = await client.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());

            // Or if you want to get a specific user:
            // var user = await client.Users.GetAsync("auth0|userId123");

            return Ok(users);
        }
    }
}
