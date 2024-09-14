using System.ComponentModel.DataAnnotations;

namespace Molla.Areas.Admin.Models.Roles;
public class EditRoleViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;
    [Required(ErrorMessage = "Role Name is Required")]
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }
}