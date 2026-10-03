using Alexapps.SkinCare.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Alexapps.SkinCare.Permissions;

public class SkinCarePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SkinCarePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(SkinCarePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SkinCareResource>(name);
    }
}



