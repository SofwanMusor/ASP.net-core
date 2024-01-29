using Microsoft.AspNetCore.Identity;
using InterfaceRole;

public class RolesService : IRolesService
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesService(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task CreateRoleAsync(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
