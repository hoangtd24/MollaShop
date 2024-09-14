using System.ComponentModel.DataAnnotations;
namespace Molla.Areas.Admin.Models.Roles
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}