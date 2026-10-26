using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.DTOs.TrainingPlan;
using SwimTrack.DTOs.Workout;
using SwimTrack.Services.Contracts;

namespace SwimTrack.Web.Controllers;

[Authorize]
public class TrainingPlanController : Controller
{
    private readonly ITrainingPlanService _trainingPlanService;

    public TrainingPlanController(ITrainingPlanService trainingPlanService)
    {
        _trainingPlanService = trainingPlanService;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        string userId = ValidateAuthorizedUserId(GetUserId());
        IEnumerable<TrainingPlanViewModel> trainingPlanViewModel = await _trainingPlanService.GetAllTrainingPlansAsync(userId);

        return View(trainingPlanViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainingPlanInputModel()
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromQuery] string? workoutAction,
        [FromQuery] int? workoutIndex,
        [FromForm] TrainingPlanInputModel trainingPlanInputModel)
    {
        List<WorkoutInputModel> trainingPlanWorkouts = trainingPlanInputModel.Workouts.ToList();

        if (workoutAction == "add")
        {
            trainingPlanWorkouts.Add(new WorkoutInputModel()
            {
                Date = trainingPlanInputModel.StartDate,
            });
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

            ModelState.Clear();

            return View(trainingPlanInputModel);
        }

        if (workoutAction == "remove")
        {
            if (!workoutIndex.HasValue
                || workoutIndex.Value < 0
                || workoutIndex.Value >= trainingPlanWorkouts.Count)
            {
                return BadRequest();
            }

            trainingPlanWorkouts.RemoveAt(workoutIndex.Value);
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

            ModelState.Clear();

            return View(trainingPlanInputModel);
        }

        if (workoutAction != null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(trainingPlanInputModel);
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        await _trainingPlanService.CreateTrainingPlanAsync(trainingPlanInputModel, userId);

        return RedirectToAction(nameof(All));
    }

    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        TrainingPlanInputModel? trainingPlanInputModel = await _trainingPlanService.GetTrainingPlanForEditAsync(id.Value, userId);
        if (trainingPlanInputModel == null)
        {
            return NotFound();
        }

        return View(trainingPlanInputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromQuery] string? workoutAction,
        [FromQuery] int? workoutIndex,
        [FromForm] TrainingPlanInputModel trainingPlanInputModel)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        List<WorkoutInputModel> trainingPlanWorkouts = trainingPlanInputModel.Workouts.ToList();

        if (workoutAction == "add")
        {
            if (workoutIndex.HasValue)
            {
                return BadRequest();
            }

            trainingPlanWorkouts.Add(new WorkoutInputModel()
            {
                Date = trainingPlanInputModel.StartDate,
            });
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

            ModelState.Clear();

            return View(trainingPlanInputModel);
        }

        if (workoutAction == "remove")
        {
            if (!workoutIndex.HasValue
                || workoutIndex.Value < 0
                || workoutIndex.Value >= trainingPlanWorkouts.Count)
            {
                return BadRequest();
            }

            trainingPlanWorkouts.RemoveAt(workoutIndex.Value);
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

            ModelState.Clear();

            return View(trainingPlanInputModel);
        }

        if (workoutAction != null || workoutIndex.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(trainingPlanInputModel);
        }

        string userId = ValidateAuthorizedUserId(GetUserId());
        bool requestEdit = await _trainingPlanService.EditTrainingPlanAsync(id.Value, trainingPlanInputModel, userId);
        if (!requestEdit)
        {
            return NotFound();
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

        string userId = ValidateAuthorizedUserId(GetUserId());
        bool requestRemove = await _trainingPlanService.RemoveTrainingPlanAsync(id.Value, userId);
        if (!requestRemove)
        {
            return NotFound();
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

        string userId = ValidateAuthorizedUserId(GetUserId());
        TrainingPlanViewModel? trainingPlanViewModel = await _trainingPlanService.GetTrainingPlanDetailsAsync(id.Value, userId);
        if (trainingPlanViewModel == null)
        {
            return NotFound();
        }

        return View(trainingPlanViewModel);
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
