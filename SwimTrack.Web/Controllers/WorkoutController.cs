using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.DTOs.Workout;
using SwimTrack.DTOs.WorkoutResult;
using SwimTrack.Services.Contracts;
using SwimTrack.Shared.Enums;

namespace SwimTrack.Web.Controllers;

[Authorize]
public class WorkoutController : Controller
{
    private readonly IWorkoutService _workoutService;

    public WorkoutController(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        WorkoutInputModel? workoutInputModel = await _workoutService.GetWorkoutForEditAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (workoutInputModel == null)
        {
            return NotFound();
        }

        ViewData["TrainingPlanId"] = trainingPlanId.Value;

        return View(workoutInputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId,
        [FromForm] WorkoutInputModel workoutInputModel)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["TrainingPlanId"] = trainingPlanId.Value;

            return View(workoutInputModel);
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        try
        {
            bool requestEdit = await _workoutService.EditWorkoutAsync(
                id.Value,
                trainingPlanId.Value,
                workoutInputModel,
                userId);
            if (!requestEdit)
            {
                return NotFound();
            }
        }
        catch (ValidationException ex)
        {
            string memberKey = ex.ValidationResult?.MemberNames.FirstOrDefault()
                ?? nameof(WorkoutInputModel.Date);
            ModelState.AddModelError(memberKey, ex.Message);

            ViewData["TrainingPlanId"] = trainingPlanId.Value;

            return View(workoutInputModel);
        }

        return RedirectToAction(
            nameof(TrainingPlanController.Details),
            "TrainingPlan",
            new { id = trainingPlanId.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Complete(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        WorkoutResultInputModel? workoutResultInputModel = await _workoutService.GetWorkoutForCompletionAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (workoutResultInputModel == null)
        {
            return NotFound();
        }

        workoutResultInputModel.CompletedAt = DateTime.Today;

        ViewData["TrainingPlanId"] = trainingPlanId.Value;
        ViewData["WorkoutId"] = id.Value;

        return View(workoutResultInputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId,
        [FromForm] WorkoutResultInputModel workoutResultInputModel)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["TrainingPlanId"] = trainingPlanId.Value;
            ViewData["WorkoutId"] = id.Value;

            return View(workoutResultInputModel);
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        try
        {
            bool requestComplete = await _workoutService.CompleteWorkoutAsync(
                id.Value,
                trainingPlanId.Value,
                workoutResultInputModel,
                userId);
            if (!requestComplete)
            {
                return NotFound();
            }
        }
        catch (ValidationException ex)
        {
            string memberKey = ex.ValidationResult?.MemberNames.FirstOrDefault()
                ?? nameof(WorkoutResultInputModel.CompletedAt);
            ModelState.AddModelError(memberKey, ex.Message);

            ViewData["TrainingPlanId"] = trainingPlanId.Value;
            ViewData["WorkoutId"] = id.Value;

            return View(workoutResultInputModel);
        }

        return RedirectToAction(
            nameof(TrainingPlanController.Details),
            "TrainingPlan",
            new { id = trainingPlanId.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Skip(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        bool requestSkip = await _workoutService.SkipWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (!requestSkip)
        {
            return NotFound();
        }

        return RedirectToAction(
            nameof(TrainingPlanController.Details),
            "TrainingPlan",
            new { id = trainingPlanId.Value });
    }

    [HttpPost]
    public async Task<IActionResult> PlanAgain(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        bool requestPlanAgain = await _workoutService.PlanAgainWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (!requestPlanAgain)
        {
            return NotFound();
        }

        return RedirectToAction(
            nameof(TrainingPlanController.Details),
            "TrainingPlan",
            new { id = trainingPlanId.Value });
    }

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private string ValidateAuthorizedUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("General identity error occurred - id is null in authorized area!");
        }

        return userId;
    }
}
