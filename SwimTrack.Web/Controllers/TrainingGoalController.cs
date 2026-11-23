using Microsoft.AspNetCore.Mvc;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.TrainingGoal.Request;
using SwimTrack.Services.DTOs.TrainingGoal.Response;
using SwimTrack.Web.ViewModels.TrainingGoal.InputModels;
using SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;

namespace SwimTrack.Web.Controllers;

public class TrainingGoalController : BaseController
{
    private readonly ITrainingGoalService _trainingGoalService;

    public TrainingGoalController(ITrainingGoalService trainingGoalService)
    {
        _trainingGoalService = trainingGoalService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateTrainingGoalFormInputModel
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateTrainingGoalFormInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        CreateTrainingGoalRequestModel createTrainingGoalRequest = new()
        {
            Title = inputModel.Title,
            TargetDistance = inputModel.TargetDistance,
            StartDate = inputModel.StartDate,
            EndDate = inputModel.EndDate,
        };

        ServiceResult result = await _trainingGoalService
            .CreateTrainingGoalAsync(createTrainingGoalRequest, UserId);
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

        EditTrainingGoalResponseModel? editTrainingGoalResponse = await _trainingGoalService
            .GetTrainingGoalForEditAsync(id.Value, UserId);
        if (editTrainingGoalResponse == null)
        {
            return NotFound();
        }

        EditTrainingGoalFormInputModel inputModel = new()
        {
            Id = editTrainingGoalResponse.Id,
            Title = editTrainingGoalResponse.Title,
            TargetDistance = editTrainingGoalResponse.TargetDistance,
            StartDate = editTrainingGoalResponse.StartDate,
            EndDate = editTrainingGoalResponse.EndDate,
        };

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromForm] EditTrainingGoalFormInputModel inputModel)
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return BadRequest();
        }

        inputModel.Id = id.Value;

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        EditTrainingGoalRequestModel editTrainingGoalRequest = new()
        {
            Id = id.Value,
            Title = inputModel.Title,
            TargetDistance = inputModel.TargetDistance,
            StartDate = inputModel.StartDate,
            EndDate = inputModel.EndDate,
        };

        ServiceResult result = await _trainingGoalService
            .EditTrainingGoalAsync(editTrainingGoalRequest, UserId);
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

        ServiceResult result = await _trainingGoalService
            .RemoveTrainingGoalAsync(id.Value, UserId);
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

        TrainingGoalDetailsResponseModel? trainingGoalDetailsResponse = await _trainingGoalService
            .GetTrainingGoalDetailsAsync(id.Value, UserId);
        if (trainingGoalDetailsResponse == null)
        {
            return NotFound();
        }

        TrainingGoalDetailsViewModel trainingGoalDetailsViewModel = new()
        {
            Id = trainingGoalDetailsResponse.Id,
            Title = trainingGoalDetailsResponse.Title,
            TargetDistance = trainingGoalDetailsResponse.TargetDistance,
            StartDate = trainingGoalDetailsResponse.StartDate,
            EndDate = trainingGoalDetailsResponse.EndDate,
        };

        return View(trainingGoalDetailsViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        IEnumerable<AllTrainingGoalsResponseModel> allTrainingGoalsResponse = await _trainingGoalService
            .GetAllTrainingGoalsAsync(UserId);

        IReadOnlyCollection<AllTrainingGoalsViewModel> allTrainingGoalsViewModel = allTrainingGoalsResponse
            .OrderBy(tg => tg.StartDate)
            .ThenBy(tg => tg.EndDate)
            .ThenBy(tg => tg.Id)
            .Select(tg => new AllTrainingGoalsViewModel
            {
                Id = tg.Id,
                Title = tg.Title,
                TargetDistance = tg.TargetDistance,
                StartDate = tg.StartDate,
                EndDate = tg.EndDate,
            })
            .ToArray();

        AllTrainingGoalsPageViewModel pageViewModel = new(allTrainingGoalsViewModel);

        return View(pageViewModel);
    }
}
