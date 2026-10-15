using Microsoft.AspNetCore.Mvc;

using SwimTrack.DTOs.TrainingPlan;
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

        return View(trainingPlanViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainingPlanInputModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(TrainingPlanInputModel trainingPlanInputModel)
    {
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
    public async Task<IActionResult> Edit([FromRoute] int? id, TrainingPlanInputModel trainingPlanInputModel)
    {
        if (!id.HasValue || id.Value <= 0)
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
