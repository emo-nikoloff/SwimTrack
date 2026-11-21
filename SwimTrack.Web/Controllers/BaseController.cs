using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.Services.Common.Results;
using static SwimTrack.Shared.Constants.ValidationMessages.Identity;

namespace SwimTrack.Web.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public abstract class BaseController : Controller
{
    protected string UserId
        => ValidateAuthorizedUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));

    protected void AddServiceErrorsToModelState(IEnumerable<ServiceError> errors)
    {
        foreach (ServiceError error in errors)
        {
            ModelState.AddModelError(
                error.PropertyName ?? string.Empty,
                error.ErrorMessage);
        }
    }

    private string ValidateAuthorizedUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException(InvalidUserId);
        }

        return userId;
    }
}
