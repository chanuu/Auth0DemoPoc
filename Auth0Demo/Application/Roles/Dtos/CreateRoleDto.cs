using System.ComponentModel.DataAnnotations;

namespace Auth0Demo.Application.Roles.Dtos
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }
    }
}
