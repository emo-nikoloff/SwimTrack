using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Request;
using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Web.ViewModels.Workout.InputModels;
using SwimTrack.Web.ViewModels.WorkoutResult.InputModels;

namespace SwimTrack.Web.Controllers;

public class WorkoutController : BaseController
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

        EditWorkoutResponseModel? editWorkoutResponse =
            await _workoutService.GetWorkoutForEditAsync(
                id.Value,
                trainingPlanId.Value,
                UserId);
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

        try
        {
            bool isEditSuccessful = await _workoutService.EditWorkoutAsync(
                trainingPlanId.Value,
                editWorkoutRequest,
                UserId);
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

        WorkoutResultResponseModel? workoutResultResponse = await _workoutService.GetWorkoutForCompletionAsync(
            id.Value,
            trainingPlanId.Value,
            UserId);
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

        try
        {
            bool isCompletionSuccessful = await _workoutService.CompleteWorkoutAsync(
                id.Value,
                trainingPlanId.Value,
                createWorkoutResultRequest,
                UserId);
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

        bool isSkipSuccessful = await _workoutService.SkipWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            UserId);
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

        bool isPlanAgainSuccessful = await _workoutService.PlanAgainWorkoutAsync(
            id.Value,
            trainingPlanId.Value,
            UserId);
        if (!isPlanAgainSuccessful)
        {
            return NotFound();
        }

        return RedirectToAction(
            nameof(TrainingPlanController.Details),
            "TrainingPlan",
            new { id = trainingPlanId.Value });
    }
}
