using Microsoft.AspNetCore.Mvc;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.SwimmingPool.Request;
using SwimTrack.Services.DTOs.SwimmingPool.Response;
using SwimTrack.Web.ViewModels.SwimmingPool.InputModels;
using SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;

namespace SwimTrack.Web.Controllers;

public class SwimmingPoolController : BaseController
{
    private readonly ISwimmingPoolService _swimmingPoolService;

    public SwimmingPoolController(ISwimmingPoolService swimmingPoolService)
    {
        _swimmingPoolService = swimmingPoolService;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        IEnumerable<AllSwimmingPoolsResponseModel> allSwimmingPoolsResponse = await _swimmingPoolService
            .GetAllSwimmingPoolsAsync(UserId);

        IReadOnlyCollection<AllSwimmingPoolsViewModel> allSwimmingPoolsViewModel = allSwimmingPoolsResponse
            .OrderBy(sp => sp.Name)
            .ThenBy(sp => sp.Id)
            .Select(sp => new AllSwimmingPoolsViewModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Length = sp.Length,
                Location = sp.Location,
            })
            .ToArray();

        AllSwimmingPoolsPageViewModel pageViewModel = new(allSwimmingPoolsViewModel);

        return View(pageViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSwimmingPoolFormInputModel
        {
            Length = 25,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateSwimmingPoolFormInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        CreateSwimmingPoolRequestModel createSwimmingPoolRequest = new()
        {
            Name = inputModel.Name,
            Length = inputModel.Length,
            Location = inputModel.Location,
        };

        ServiceResult result = await _swimmingPoolService
            .CreateSwimmingPoolAsync(createSwimmingPoolRequest, UserId);
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

        EditSwimmingPoolResponseModel? editSwimmingPoolResponse = await _swimmingPoolService
            .GetSwimmingPoolForEditAsync(id.Value, UserId);
        if (editSwimmingPoolResponse == null)
        {
            return NotFound();
        }

        EditSwimmingPoolFormInputModel inputModel = new()
        {
            Id = editSwimmingPoolResponse.Id,
            Name = editSwimmingPoolResponse.Name,
            Length = editSwimmingPoolResponse.Length,
            Location = editSwimmingPoolResponse.Location,
        };

        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromRoute] int? id,
        [FromForm] EditSwimmingPoolFormInputModel inputModel)
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

        EditSwimmingPoolRequestModel editSwimmingPoolRequest = new()
        {
            Id = id.Value,
            Name = inputModel.Name,
            Length = inputModel.Length,
            Location = inputModel.Location,
        };

        ServiceResult result = await _swimmingPoolService
            .EditSwimmingPoolAsync(editSwimmingPoolRequest, UserId);
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

        ServiceResult result = await _swimmingPoolService
            .RemoveSwimmingPoolAsync(id.Value, UserId);
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

        SwimmingPoolDetailsResponseModel? swimmingPoolDetailsResponse = await _swimmingPoolService
            .GetSwimmingPoolDetailsAsync(id.Value, UserId);
        if (swimmingPoolDetailsResponse == null)
        {
            return NotFound();
        }

        SwimmingPoolDetailsViewModel swimmingPoolDetailsViewModel = new()
        {
            Id = swimmingPoolDetailsResponse.Id,
            Name = swimmingPoolDetailsResponse.Name,
            Length = swimmingPoolDetailsResponse.Length,
            Location = swimmingPoolDetailsResponse.Location,
        };

        return View(swimmingPoolDetailsViewModel);
    }
}
