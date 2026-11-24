using Microsoft.AspNetCore.Mvc;

using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.SwimmingPool.Response;
using SwimTrack.Services.DTOs.TrainingGoal.Response;
using SwimTrack.Services.DTOs.TrainingPlan.Request;
using SwimTrack.Services.DTOs.TrainingPlan.Response;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;
using SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;
using SwimTrack.Web.ViewModels.TrainingPlan.InputModels;
using SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;
using SwimTrack.Web.ViewModels.Workout.ViewModels;
using SwimTrack.Web.ViewModels.WorkoutResult.ViewModels;

namespace SwimTrack.Web.Controllers;

public class TrainingPlanController : BaseController
{
    private readonly ITrainingPlanService _trainingPlanService;

    private readonly ITrainingGoalService _trainingGoalService;

    private readonly ISwimmingPoolService _swimmingPoolService;

    public TrainingPlanController(
        ITrainingPlanService trainingPlanService,
        ITrainingGoalService trainingGoalService,
        ISwimmingPoolService swimmingPoolService)
    {
        _trainingPlanService = trainingPlanService;
        _trainingGoalService = trainingGoalService;
        _swimmingPoolService = swimmingPoolService;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        IEnumerable<AllTrainingPlansResponseModel> allTrainingPlansResponse = await _trainingPlanService
            .GetAllTrainingPlansAsync(UserId);

        IReadOnlyCollection<AllTrainingPlansViewModel> allTrainingPlansViewModel = allTrainingPlansResponse
            .OrderBy(tp => tp.StartDate)
            .ThenBy(tp => tp.EndDate)
            .ThenBy(tp => tp.Id)
            .Select(tp => new AllTrainingPlansViewModel
            {
                Id = tp.Id,
                Name = tp.Name,
                TrainingGoal = tp.TrainingGoal == null
                    ? null
                    : new TrainingGoalViewModel
                    {
                        Id = tp.TrainingGoal.Id,
                        Title = tp.TrainingGoal.Title,
                        TargetDistance = tp.TrainingGoal.TargetDistance,
                        StartDate = tp.TrainingGoal.StartDate,
                        EndDate = tp.TrainingGoal.EndDate,
                    },
                SwimmingPool = tp.SwimmingPool == null
                    ? null
                    : new SwimmingPoolViewModel
                    {
                        Id = tp.SwimmingPool.Id,
                        Name = tp.SwimmingPool.Name,
                        Length = tp.SwimmingPool.Length,
                        Location = tp.SwimmingPool.Location,
                    },
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
                                SwimmingPool = w.Result.SwimmingPool == null
                                    ? null
                                    : new SwimmingPoolViewModel
                                    {
                                        Id = w.Result.SwimmingPool.Id,
                                        Name = w.Result.SwimmingPool.Name,
                                        Length = w.Result.SwimmingPool.Length,
                                        Location = w.Result.SwimmingPool.Location,
                                    },
                            },
                    })
                    .ToArray(),
            })
            .ToArray();

        AllTrainingPlansPageViewModel pageViewModel = new(allTrainingPlansViewModel);

        return View(pageViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        CreateTrainingPlanFormInputModel inputModel = new()
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
        };

        await PopulateAvailableGoalsAsync(inputModel);
        await PopulateAvailablePoolsAsync(inputModel);

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateTrainingPlanFormInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAvailableGoalsAsync(inputModel);
            await PopulateAvailablePoolsAsync(inputModel);

            return View(inputModel);
        }

        CreateTrainingPlanRequestModel createTrainingPlanRequest = new()
        {
            Name = inputModel.Name,
            TrainingGoalId = inputModel.TrainingGoalId,
            SwimmingPoolId = inputModel.SwimmingPoolId,
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

        ServiceResult result = await _trainingPlanService
            .CreateTrainingPlanAsync(createTrainingPlanRequest, UserId);
        if (result.Status == ServiceResultStatus.InvalidRequest)
        {
            AddServiceErrorsToModelState(result.Errors);

            await PopulateAvailableGoalsAsync(inputModel);
            await PopulateAvailablePoolsAsync(inputModel);

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

        EditTrainingPlanResponseModel? editTrainingPlanResponse = await _trainingPlanService
            .GetTrainingPlanForEditAsync(id.Value, UserId);
        if (editTrainingPlanResponse == null)
        {
            return NotFound();
        }

        EditTrainingPlanFormInputModel inputModel = new()
        {
            Id = editTrainingPlanResponse.Id,
            Name = editTrainingPlanResponse.Name,
            TrainingGoalId = editTrainingPlanResponse.TrainingGoalId,
            SwimmingPoolId = editTrainingPlanResponse.SwimmingPoolId,
            StartDate = editTrainingPlanResponse.StartDate,
            EndDate = editTrainingPlanResponse.EndDate,
            ExistingWorkouts = editTrainingPlanResponse.Workouts
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
                })
                .ToList(),
            KeptWorkoutIds = editTrainingPlanResponse.Workouts
                .Select(w => w.Id)
                .ToList(),
        };

        await PopulateAvailableGoalsAsync(inputModel);
        await PopulateAvailablePoolsAsync(inputModel);

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromForm] EditTrainingPlanFormInputModel inputModel)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        inputModel.Id = id.Value;

        if (!ModelState.IsValid)
        {
            await PopulateAvailableGoalsAsync(inputModel);
            await PopulateAvailablePoolsAsync(inputModel);
            await RestoreExistingWorkoutsAsync(inputModel);

            return View(inputModel);
        }

        EditTrainingPlanRequestModel editTrainingPlanRequest = new()
        {
            Id = id.Value,
            Name = inputModel.Name,
            TrainingGoalId = inputModel.TrainingGoalId,
            SwimmingPoolId = inputModel.SwimmingPoolId,
            StartDate = inputModel.StartDate,
            EndDate = inputModel.EndDate,
            KeptWorkoutIds = inputModel.KeptWorkoutIds,
            NewWorkouts = inputModel.NewWorkouts
                .Select(w => new CreateWorkoutRequestModel
                {
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                })
                .ToArray(),
        };

        ServiceResult result = await _trainingPlanService
            .EditTrainingPlanAsync(editTrainingPlanRequest, UserId);
        if (result.Status == ServiceResultStatus.InvalidRequest)
        {
            AddServiceErrorsToModelState(result.Errors);

            await PopulateAvailableGoalsAsync(inputModel);
            await PopulateAvailablePoolsAsync(inputModel);
            await RestoreExistingWorkoutsAsync(inputModel);

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

        ServiceResult result = await _trainingPlanService
            .RemoveTrainingPlanAsync(id.Value, UserId);
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

        TrainingPlanDetailsResponseModel? trainingPlanDetailsResponse = await _trainingPlanService
            .GetTrainingPlanDetailsAsync(id.Value, UserId);
        if (trainingPlanDetailsResponse == null)
        {
            return NotFound();
        }

        TrainingPlanDetailsViewModel trainingPlanDetailsViewModel = new()
        {
            Id = trainingPlanDetailsResponse.Id,
            Name = trainingPlanDetailsResponse.Name,
            TrainingGoal = trainingPlanDetailsResponse.TrainingGoal == null
                ? null
                : new TrainingGoalViewModel
                {
                    Id = trainingPlanDetailsResponse.TrainingGoal.Id,
                    Title = trainingPlanDetailsResponse.TrainingGoal.Title,
                    TargetDistance = trainingPlanDetailsResponse.TrainingGoal.TargetDistance,
                    StartDate = trainingPlanDetailsResponse.TrainingGoal.StartDate,
                    EndDate = trainingPlanDetailsResponse.TrainingGoal.EndDate,
                },
            SwimmingPool = trainingPlanDetailsResponse.SwimmingPool == null
                ? null
                : new SwimmingPoolViewModel
                {
                    Id = trainingPlanDetailsResponse.SwimmingPool.Id,
                    Name = trainingPlanDetailsResponse.SwimmingPool.Name,
                    Length = trainingPlanDetailsResponse.SwimmingPool.Length,
                    Location = trainingPlanDetailsResponse.SwimmingPool.Location,
                },
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
                            SwimmingPool = w.Result.SwimmingPool == null
                                ? null
                                : new SwimmingPoolViewModel
                                {
                                    Id = w.Result.SwimmingPool.Id,
                                    Name = w.Result.SwimmingPool.Name,
                                    Length = w.Result.SwimmingPool.Length,
                                    Location = w.Result.SwimmingPool.Location,
                                },
                        },
                })
                .ToArray(),
        };

        return View(trainingPlanDetailsViewModel);
    }

    private async Task PopulateAvailableGoalsAsync(BaseTrainingPlanFormInputModel inputModel)
    {
        IEnumerable<AllTrainingGoalsResponseModel> goals = await _trainingGoalService
            .GetAllTrainingGoalsAsync(UserId);

        inputModel.AvailableGoals = goals
            .OrderBy(g => g.StartDate)
            .ThenBy(g => g.EndDate)
            .ThenBy(g => g.Id)
            .Select(g => new AllTrainingGoalsViewModel
            {
                Id = g.Id,
                Title = g.Title,
                TargetDistance = g.TargetDistance,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
            })
            .ToArray();
    }

    private async Task PopulateAvailablePoolsAsync(BaseTrainingPlanFormInputModel inputModel)
    {
        IEnumerable<AllSwimmingPoolsResponseModel> pools = await _swimmingPoolService
            .GetAllSwimmingPoolsAsync(UserId);

        inputModel.AvailablePools = pools
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Select(p => new AllSwimmingPoolsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Length = p.Length,
                Location = p.Location,
            })
            .ToArray();
    }

    private async Task RestoreExistingWorkoutsAsync(EditTrainingPlanFormInputModel inputModel)
    {
        if (inputModel.Id <= 0)
        {
            return;
        }

        EditTrainingPlanResponseModel? trainingPlan = await _trainingPlanService
            .GetTrainingPlanForEditAsync(inputModel.Id, UserId);
        if (trainingPlan != null)
        {
            HashSet<int> keptWorkouts = inputModel.KeptWorkoutIds.ToHashSet();

            inputModel.ExistingWorkouts = trainingPlan.Workouts
                .Where(w => keptWorkouts.Contains(w.Id))
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
                })
                .ToList();
        }
    }
}
