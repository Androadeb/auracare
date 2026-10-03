using Alexapps.SkinCare.Dtos.Auth.Commands.Register;
using Alexapps.SkinCare.Dtos.Auth.Results.Register;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers.Client;

public class ClientAuthController(IAuthService authService) : SkinCareController
{
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Client.Register)]
    public async Task<RegisterResult> RegisterAsync([FromForm] RegisterCommand command, CancellationToken cancellationToken = default)
    {
        return await authService.RegisterAsync(command);
    }
}
