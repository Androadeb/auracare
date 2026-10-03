using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Alexapps.SkinCare.Seeders
{

    public class UserRolesDataSeeder : ITransientDependency
    {
        private readonly IdentityRoleManager roleManager;

        public UserRolesDataSeeder(IdentityRoleManager roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var roles = new List<IdentityRole>
    {
        new IdentityRole(Guid.NewGuid(), RoleEnum.ADMIN),
        new IdentityRole(Guid.NewGuid(), RoleEnum.CLIENT),
        new IdentityRole(Guid.NewGuid(), RoleEnum.DOCTOR),
        new IdentityRole(Guid.NewGuid(), RoleEnum.LAB)
    };

            foreach (var role in roles)
            {
                var existingRole = await roleManager.FindByNameAsync(role.Name);

                if (existingRole == null)
                {
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(e => $"{e.Code}: {e.Description}")
                        );

                        throw new Exception(
                            $"Failed to create role '{role.Name}': {errors}"
                        );
                    }
                }
            }
        }
    }

}


