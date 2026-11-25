using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Seeding.Utilities;
using SwimTrack.Web.Areas.Admin.ViewModels;
using static SwimTrack.Shared.Constants.ApplicationDefaults;
using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity;
using static SwimTrack.Shared.Constants.ApplicationDefaults.Notification;
using static SwimTrack.Shared.Constants.ValidationMessages.Notification;

namespace SwimTrack.Web.Areas.Admin.Controllers;

public class UserManagementController : BaseAdminController
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly SignInManager<IdentityUser> _signInManager;

    public UserManagementController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IReadOnlyCollection<IdentityUser> users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToArrayAsync();

        List<UserViewModel> userViewModels = new(users.Count);
        foreach (IdentityUser user in users)
        {
            UserViewModel userViewModel = new()
            {
                Id = user.Id,
                Email = user.Email ?? MissingUserEmail,
                Roles = await _userManager
                    .GetRolesAsync(user),
            };

            userViewModels.Add(userViewModel);
        }

        UserManagementViewModel viewModel = new(userViewModels, IdentitySeeder.DefaultRoles);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole([FromForm] UserRoleInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        IdentityUser? dbUser = await _userManager
            .FindByIdAsync(inputModel.UserId);
        if (dbUser == null)
        {
            TempData[ErrorNotificationKey] = UserNotFound;

            return RedirectToAction(nameof(Index));
        }

        IdentityRole? dbRole = await _roleManager
            .FindByNameAsync(inputModel.RoleName);
        if (dbRole == null)
        {
            TempData[ErrorNotificationKey] = RoleNotFound;

            return RedirectToAction(nameof(Index));
        }

        bool userAlreadyInRole = await _userManager
            .IsInRoleAsync(dbUser, inputModel.RoleName);
        if (userAlreadyInRole)
        {
            TempData[WarningNotificationKey] = UserAlreadyInRole;

            return RedirectToAction(nameof(Index));
        }

        IdentityResult result = await _userManager
            .AddToRoleAsync(dbUser, inputModel.RoleName);
        if (!result.Succeeded)
        {
            TempData[ErrorNotificationKey] = UnexpectedError;

            return RedirectToAction(nameof(Index));
        }

        TempData[SuccessNotificationKey] = RoleAssignedSuccess;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole([FromForm] UserRoleInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        IdentityUser? dbUser = await _userManager
            .FindByIdAsync(inputModel.UserId);
        if (dbUser == null)
        {
            TempData[ErrorNotificationKey] = UserNotFound;

            return RedirectToAction(nameof(Index));
        }

        IdentityRole? dbRole = await _roleManager
            .FindByNameAsync(inputModel.RoleName);
        if (dbRole == null)
        {
            TempData[ErrorNotificationKey] = RoleNotFound;

            return RedirectToAction(nameof(Index));
        }

        bool userInRole = await _userManager
            .IsInRoleAsync(dbUser, inputModel.RoleName);
        if (!userInRole)
        {
            TempData[WarningNotificationKey] = UserNotInRole;

            return RedirectToAction(nameof(Index));
        }

        bool isAdminRole = string.Equals(
            dbRole.Name,
            Role.Admin,
            StringComparison.OrdinalIgnoreCase);
        if (isAdminRole)
        {
            IList<IdentityUser> admins = await _userManager
                .GetUsersInRoleAsync(Role.Admin);
            if (admins.Count <= 1)
            {
                TempData[ErrorNotificationKey] = CannotRemoveLastAdminRole;

                return RedirectToAction(nameof(Index));
            }
        }

        IdentityResult result = await _userManager
            .RemoveFromRoleAsync(dbUser, inputModel.RoleName);
        if (!result.Succeeded)
        {
            TempData[ErrorNotificationKey] = UnexpectedError;

            return RedirectToAction(nameof(Index));
        }

        bool isSelf = string.Equals(dbUser.Id, UserId, StringComparison.Ordinal);
        if (isSelf)
        {
            await _signInManager
                .RefreshSignInAsync(dbUser);
        }

        TempData[SuccessNotificationKey] = RoleRemovedSuccess;

        if (isSelf && isAdminRole)
        {
            return LocalRedirect(ApplicationHomePagePath);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromForm] string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest();
        }

        IdentityUser? dbUser = await _userManager
            .FindByIdAsync(userId);
        if (dbUser == null)
        {
            TempData[ErrorNotificationKey] = UserNotFound;

            return RedirectToAction(nameof(Index));
        }

        bool isSelf = string.Equals(dbUser.Id, UserId, StringComparison.Ordinal);
        if (isSelf)
        {
            TempData[ErrorNotificationKey] = CannotDeleteOwnAccount;

            return RedirectToAction(nameof(Index));
        }

        bool isTargetAdmin = await _userManager
            .IsInRoleAsync(dbUser, Role.Admin);
        if (isTargetAdmin)
        {
            IList<IdentityUser> admins = await _userManager
                .GetUsersInRoleAsync(Role.Admin);
            if (admins.Count <= 1)
            {
                TempData[ErrorNotificationKey] = CannotDeleteLastAdmin;

                return RedirectToAction(nameof(Index));
            }
        }

        IdentityResult result = await _userManager
            .DeleteAsync(dbUser);
        if (!result.Succeeded)
        {
            TempData[ErrorNotificationKey] = UnexpectedError;

            return RedirectToAction(nameof(Index));
        }

        TempData[SuccessNotificationKey] = UserDeletedSuccess;

        return RedirectToAction(nameof(Index));
    }
}
