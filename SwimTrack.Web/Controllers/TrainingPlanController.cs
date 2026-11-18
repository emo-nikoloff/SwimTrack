using Microsoft.AspNetCore.Mvc;
using SwimTrack.Services.Common.Results;

using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.TrainingPlan.Request;
using SwimTrack.Services.DTOs.TrainingPlan.Response;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Web.ViewModels.TrainingPlan.InputModels;
using SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;
using SwimTrack.Web.ViewModels.Workout.InputModels;
using SwimTrack.Web.ViewModels.Workout.ViewModels;
using SwimTrack.Web.ViewModels.WorkoutResult.ViewModels;

namespace SwimTrack.Web.Controllers;

public class TrainingPlanController : BaseController
{
    private readonly ITrainingPlanService _trainingPlanService;

    public TrainingPlanController(ITrainingPlanService trainingPlanService)
    {
        _trainingPlanService = trainingPlanService;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        IEnumerable<AllTrainingPlansResponseModel> allTrainingPlansResponse =
            await _trainingPlanService.GetAllTrainingPlansAsync(UserId);

        IReadOnlyCollection<AllTrainingPlansViewModel> allTrainingPlansViewModel = allTrainingPlansResponse
            .OrderBy(tp => tp.StartDate)
            .ThenBy(tp => tp.EndDate)
            .ThenBy(tp => tp.Id)
            .Select(tp => new AllTrainingPlansViewModel
            {
                Id = tp.Id,
                Name = tp.Name,
                Goal = tp.Goal,
                StartDate = tp.StartDate,
                EndDate = tp.EndDate,
                Workouts = tp.Workouts
                    .OrderBy(w => w.Date)
                    .ThenBy(w => w.Id)
                    .Select(w => new WorkoutViewModel
                    {
                        Id = w.Id,
                        Date = w.Date,
                        PlannedDistance = w.PlannedDistance,
                        PlannedDuration = w.PlannedDuration,
                        Notes = w.Notes,
                        Status = w.Status,
                        Result = w.Result == null
                            ? null
                            : new WorkoutResultViewModel
                            {
                                ActualDistance = w.Result.ActualDistance,
                                ActualDuration = w.Result.ActualDuration,
                                CompletedAt = w.Result.CompletedAt,
                                Notes = w.Result.Notes,
                            },
                    })
                    .ToArray(),
            })
            .ToArray();

        AllTrainingPlansPageViewModel pageViewModel = new(allTrainingPlansViewModel);

        return View(pageViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainingPlanFormInputModel()
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromQuery] string? workoutAction,
        [FromQuery] int? workoutIndex,
        [FromForm] TrainingPlanFormInputModel inputModel)
    {
        if (workoutAction == "add")
        {
            if (workoutIndex.HasValue)
            {
                return BadRequest();
            }

            inputModel.Workouts.Add(new WorkoutFormInputModel()
            {
                Date = inputModel.StartDate,
            });

            ModelState.Clear();

            return View(inputModel);
        }

        if (workoutAction == "remove")
        {
            if (!workoutIndex.HasValue
                || workoutIndex.Value < 0
                || workoutIndex.Value >= inputModel.Workouts.Count)
            {
                return BadRequest();
            }

            inputModel.Workouts.RemoveAt(workoutIndex.Value);

            ModelState.Clear();

            return View(inputModel);
        }

        if (workoutAction != null || workoutIndex.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        CreateTrainingPlanRequestModel createTrainingPlanRequest = new()
        {
            Name = inputModel.Name,
            Goal = inputModel.Goal,
            StartDate = inputModel.StartDate,
            EndDate = inputModel.EndDate,
            Workouts = inputModel.Workouts
                .Select(w => new CreateWorkoutRequestModel
                {
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                })
                .ToArray(),
        };

        ServiceResult result =
            await _trainingPlanService.CreateTrainingPlanAsync(createTrainingPlanRequest, UserId);
        if (result.Status == ServiceResultStatus.InvalidRequest)
        {
            AddServiceErrorsToModelState(result.Errors);

            return View(inputModel);
        }
        else if (result.Status != ServiceResultStatus.Success)
        {
            throw new InvalidOperationException($"Unexpected service result: {result.Status}");
        }

        return RedirectToAction(nameof(All));
    }

    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        EditTrainingPlanResponseModel? editTrainingPlanResponse =
            await _trainingPlanService.GetTrainingPlanForEditAsync(id.Value, UserId);
        if (editTrainingPlanResponse == null)
        {
            return NotFound();
        }

        TrainingPlanFormInputModel inputModel = new()
        {
            Id = editTrainingPlanResponse.Id,
            Name = editTrainingPlanResponse.Name,
            Goal = editTrainingPlanResponse.Goal,
            StartDate = editTrainingPlanResponse.StartDate,
            EndDate = editTrainingPlanResponse.EndDate,
            Workouts = editTrainingPlanResponse.Workouts
                .Select(w => new WorkoutFormInputModel
                {
                    Id = w.Id,
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                    Status = w.Status,
                })
                .ToList(),
        };

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromQuery] string? workoutAction,
        [FromQuery] int? workoutIndex,
        [FromForm] TrainingPlanFormInputModel inputModel)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        inputModel.Id = id.Value;

        if (workoutAction == "add")
        {
            if (workoutIndex.HasValue)
            {
                return BadRequest();
            }

            inputModel.Workouts.Add(new WorkoutFormInputModel()
            {
                Date = inputModel.StartDate,
            });

            ModelState.Clear();

            return View(inputModel);
        }

        if (workoutAction == "remove")
        {
            if (!workoutIndex.HasValue
                || workoutIndex.Value < 0
                || workoutIndex.Value >= inputModel.Workouts.Count)
            {
                return BadRequest();
            }

            inputModel.Workouts.RemoveAt(workoutIndex.Value);

            ModelState.Clear();

            return View(inputModel);
        }

        if (workoutAction != null || workoutIndex.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        EditTrainingPlanRequestModel editTrainingPlanRequest = new()
        {
            Id = id.Value,
            Name = inputModel.Name,
            Goal = inputModel.Goal,
            StartDate = inputModel.StartDate,
            EndDate = inputModel.EndDate,
            Workouts = inputModel.Workouts
                .Select(w => new EditWorkoutRequestModel
                {
                    Id = w.Id,
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                })
                .ToArray(),
        };

        ServiceResult result =
            await _trainingPlanService.EditTrainingPlanAsync(editTrainingPlanRequest, UserId);
        if (result.Status == ServiceResultStatus.InvalidRequest)
        {
            AddServiceErrorsToModelState(result.Errors);

            return View(inputModel);
        }
        else if (result.Status == ServiceResultStatus.NotFound)
        {
            return NotFound();
        }
        else if (result.Status != ServiceResultStatus.Success)
        {
            throw new InvalidOperationException($"Unexpected service result: {result.Status}");
        }

        return RedirectToAction(nameof(Details), new { id = id.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Remove([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        ServiceResult result =
            await _trainingPlanService.RemoveTrainingPlanAsync(id.Value, UserId);
        if (result.Status == ServiceResultStatus.NotFound)
        {
            return NotFound();
        }
        else if (result.Status != ServiceResultStatus.Success)
        {
            throw new InvalidOperationException($"Unexpected service result: {result.Status}");
        }

        return RedirectToAction(nameof(All));
    }

    [HttpGet]
    public async Task<IActionResult> Details([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        TrainingPlanDetailsResponseModel? trainingPlanDetailsResponse =
            await _trainingPlanService.GetTrainingPlanDetailsAsync(id.Value, UserId);
        if (trainingPlanDetailsResponse == null)
        {
            return NotFound();
        }

        TrainingPlanDetailsViewModel trainingPlanDetailsViewModel = new()
        {
            Id = trainingPlanDetailsResponse.Id,
            Name = trainingPlanDetailsResponse.Name,
            Goal = trainingPlanDetailsResponse.Goal,
            StartDate = trainingPlanDetailsResponse.StartDate,
            EndDate = trainingPlanDetailsResponse.EndDate,
            Workouts = trainingPlanDetailsResponse.Workouts
                .OrderBy(w => w.Date)
                .ThenBy(w => w.Id)
                .Select(w => new TrainingPlanWorkoutViewModel
                {
                    Id = w.Id,
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                    Status = w.Status,
                    Result = w.Result == null
                        ? null
                        : new WorkoutResultViewModel
                        {
                            ActualDistance = w.Result.ActualDistance,
                            ActualDuration = w.Result.ActualDuration,
                            CompletedAt = w.Result.CompletedAt,
                            Notes = w.Result.Notes,
                        },
                })
                .ToArray(),
        };

        return View(trainingPlanDetailsViewModel);
    }
}
