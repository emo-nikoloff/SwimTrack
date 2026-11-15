using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.WorkoutResult.Request;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Web.ViewModels.Workout.InputModels;
using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Web.ViewModels.WorkoutResult.InputModels;

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
        EditWorkoutResponseModel? editWorkoutResponse =
            await _workoutService.GetWorkoutForEditAsync(
                id.Value,
                trainingPlanId.Value,
                userId);
        if (editWorkoutResponse == null)
        {
            return NotFound();
        }

        WorkoutFormInputModel inputModel = new()
        {
            Id = editWorkoutResponse.Id,
            Date = editWorkoutResponse.Date,
            PlannedDistance = editWorkoutResponse.PlannedDistance,
            PlannedDuration = editWorkoutResponse.PlannedDuration,
            Notes = editWorkoutResponse.Notes,
            Status = editWorkoutResponse.Status,
            TrainingPlanId = trainingPlanId.Value,
        };

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId,
        [FromForm] WorkoutFormInputModel inputModel)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        inputModel.Id = id.Value;
        inputModel.TrainingPlanId = trainingPlanId.Value;

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        EditWorkoutRequestModel editWorkoutRequest = new()
        {
            Id = id.Value,
            Date = inputModel.Date,
            PlannedDistance = inputModel.PlannedDistance,
            PlannedDuration = inputModel.PlannedDuration,
            Notes = inputModel.Notes,
        };

        string userId = ValidateAuthorizedUserId(GetUserId());
        try
        {
            bool isEditSuccessful = await _workoutService.EditWorkoutAsync(
                trainingPlanId.Value,
                editWorkoutRequest,
                userId);
            if (!isEditSuccessful)
            {
                return NotFound();
            }
        }
        catch (ValidationException ex)
        {
            string memberKey = ex.ValidationResult?.MemberNames.FirstOrDefault()
                ?? nameof(WorkoutFormInputModel.Date);
            ModelState.AddModelError(memberKey, ex.Message);

            return View(inputModel);
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
        WorkoutResultResponseModel? workoutResultResponse = await _workoutService.GetWorkoutForCompletionAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (workoutResultResponse == null)
        {
            return NotFound();
        }

        WorkoutResultFormInputModel inputModel = new()
        {
            ActualDistance = workoutResultResponse.ActualDistance,
            ActualDuration = workoutResultResponse.ActualDuration,
            CompletedAt = workoutResultResponse.CompletedAt,
            Notes = workoutResultResponse.Notes,
            WorkoutId = id.Value,
            TrainingPlanId = trainingPlanId.Value,
        };

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(
        [FromRoute] int? id,
        [FromQuery] int? trainingPlanId,
        [FromForm] WorkoutResultFormInputModel inputModel)
    {
        if (!id.HasValue || id.Value <= 0
            || !trainingPlanId.HasValue || trainingPlanId.Value <= 0)
        {
            return BadRequest();
        }

        inputModel.WorkoutId = id.Value;
        inputModel.TrainingPlanId = trainingPlanId.Value;

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        CreateWorkoutResultRequestModel createWorkoutResultRequest = new()
        {
            ActualDistance = inputModel.ActualDistance,
            ActualDuration = inputModel.ActualDuration,
            CompletedAt = inputModel.CompletedAt,
            Notes = inputModel.Notes,
        };

        string userId = ValidateAuthorizedUserId(GetUserId());
        try
        {
            bool isCompletionSuccessful = await _workoutService.CompleteWorkoutAsync(
                id.Value,
                trainingPlanId.Value,
                createWorkoutResultRequest,
                userId);
            if (!isCompletionSuccessful)
            {
                return NotFound();
            }
        }
        catch (ValidationException ex)
        {
            string memberKey = ex.ValidationResult?.MemberNames.FirstOrDefault()
                ?? nameof(WorkoutResultFormInputModel.CompletedAt);
            ModelState.AddModelError(memberKey, ex.Message);

            return View(inputModel);
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
        bool isSkipSuccessful = await _workoutService.SkipWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (!isSkipSuccessful)
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
        bool isPlanAgainSuccessful = await _workoutService.PlanAgainWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            userId);
        if (!isPlanAgainSuccessful)
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
