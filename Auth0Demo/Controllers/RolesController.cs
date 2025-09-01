using Auth0Demo.Application.Roles;
using Auth0Demo.Application.Roles.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Auth0Demo.Controllers
{
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }


        [HttpPost("roles")]
        
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(request.Name, request.Description);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create role {RoleName}", request.Name);
                return StatusCode(500, new { message = "Failed to create role" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            try
            {
                var role = (await _roleService.GetAllRolesAsync())
                    .FirstOrDefault(r => r.Id == id);

                if (role == null)
                    return NotFound();

                return Ok(role);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get role {RoleId}", id);
                return StatusCode(500, new { message = "Failed to retrieve role" });
            }
        }

        [HttpPost("assign")]
        
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
        {
            try
            {
                await _roleService.AssignRoleToUserAsync(request);
                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Role assignment failed");
                return BadRequest(new { message = ex.Message });
            }
           
        }


        

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var roles = await _roleService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get roles for user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to retrieve user roles" });
            }
        }



    }
}
