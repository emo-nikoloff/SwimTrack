using Microsoft.AspNetCore.Mvc;

using SwimTrack.DTOs.TrainingPlan;
using SwimTrack.DTOs.Workout;
using SwimTrack.Services.Contracts;

namespace SwimTrack.Web.Controllers;

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
        IEnumerable<TrainingPlanViewModel> trainingPlanViewModel = await _trainingPlanService.GetAllTrainingPlansAsync();
        //IEnumerable<TrainingPlanViewModel> trainingPlanViewModel = new List<TrainingPlanViewModel>();

        return View(trainingPlanViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainingPlanInputModel());
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
            trainingPlanWorkouts.Add(new WorkoutInputModel());
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

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

        await _trainingPlanService.CreateTrainingPlanAsync(trainingPlanInputModel);

        return RedirectToAction(nameof(All));
    }

    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        TrainingPlanInputModel? trainingPlanInputModel = await _trainingPlanService.GetTrainingPlanForEditAsync(id.Value);
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

            trainingPlanWorkouts.Add(new WorkoutInputModel());
            trainingPlanInputModel.Workouts = trainingPlanWorkouts;

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

        bool requestEdit = await _trainingPlanService.EditTrainingPlanAsync(id.Value, trainingPlanInputModel);
        if (!requestEdit)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(All));
    }

    [HttpPost]
    public async Task<IActionResult> Remove([FromRoute] int? id)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        bool requestRemove = await _trainingPlanService.RemoveTrainingPlanAsync(id.Value);
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

        TrainingPlanViewModel? trainingPlanViewModel = await _trainingPlanService.GetTrainingPlanDetailsAsync(id.Value);
        if (trainingPlanViewModel == null)
        {
            return NotFound();
        }

        return View(trainingPlanViewModel);
    }
}
