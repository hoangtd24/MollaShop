using Microsoft.AspNetCore.Identity;

namespace Molla.Models;
public class ApplicationRoles: IdentityRole{
    public string Description {get; set;} = string.Empty;
}