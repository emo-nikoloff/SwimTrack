using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Common.Utilities;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.SwimmingPool.Request;
using SwimTrack.Services.DTOs.SwimmingPool.Response;
using static SwimTrack.Shared.Constants.ValidationMessages.SwimmingPoolValidation;

namespace SwimTrack.Services;

public class SwimmingPoolService : ISwimmingPoolService
{
    private readonly ISwimmingPoolRepository _swimmingPoolRepository;

    private readonly ITrainingPlanRepository _trainingPlanRepository;

    private readonly IWorkoutResultRepository _workoutResultRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly BasicModelValidator _modelValidator;

    public SwimmingPoolService(
        ISwimmingPoolRepository swimmingPoolRepository,
        ITrainingPlanRepository trainingPlanRepository,
        IWorkoutResultRepository workoutResultRepository,
        IUnitOfWork unitOfWork)
    {
        _swimmingPoolRepository = swimmingPoolRepository;
        _trainingPlanRepository = trainingPlanRepository;
        _workoutResultRepository = workoutResultRepository;
        _unitOfWork = unitOfWork;
        _modelValidator = new();
    }

    public async Task<ServiceResult> CreateSwimmingPoolAsync(CreateSwimmingPoolRequestModel request, string currentUserId)
    {
        List<ServiceError> serviceErrors = new();

        bool isRequestValid = _modelValidator.IsValid(request);
        if (!isRequestValid)
        {
            IReadOnlyCollection<ValidationResult> inputErrors = _modelValidator.ValidationResults;
            foreach (ValidationResult error in inputErrors)
            {
                string? propertyName = error.MemberNames.FirstOrDefault();
                string errorMessage = error.ErrorMessage ?? string.Empty;

                ServiceError serviceError = new(propertyName, errorMessage);

                serviceErrors.Add(serviceError);
            }
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        SwimmingPool newSwimmingPool = new()
        {
            Name = request.Name,
            Length = request.Length,
            Location = request.Location,
            UserId = currentUserId,
        };

        await _swimmingPoolRepository.AddAsync(newSwimmingPool);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<EditSwimmingPoolResponseModel?> GetSwimmingPoolForEditAsync(int id, string currentUserId)
    {
        SwimmingPool? requestedPool = await _swimmingPoolRepository
            .AllAsNoTracking()
            .FirstOrDefaultAsync(sp =>
                sp.Id == id
                && sp.UserId == currentUserId);
        if (requestedPool == null)
        {
            return null;
        }

        EditSwimmingPoolResponseModel editSwimmingPoolResponse = new()
        {
            Id = requestedPool.Id,
            Name = requestedPool.Name,
            Length = requestedPool.Length,
            Location = requestedPool.Location,
        };

        return editSwimmingPoolResponse;
    }

    public async Task<ServiceResult> EditSwimmingPoolAsync(EditSwimmingPoolRequestModel request, string currentUserId)
    {
        List<ServiceError> serviceErrors = new();

        bool isRequestValid = _modelValidator.IsValid(request);
        if (!isRequestValid)
        {
            IReadOnlyCollection<ValidationResult> inputErrors = _modelValidator.ValidationResults;
            foreach (ValidationResult error in inputErrors)
            {
                string? propertyName = error.MemberNames.FirstOrDefault();
                string errorMessage = error.ErrorMessage ?? string.Empty;

                ServiceError serviceError = new(propertyName, errorMessage);

                serviceErrors.Add(serviceError);
            }
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        SwimmingPool? requestedPool = await _swimmingPoolRepository
            .All()
            .FirstOrDefaultAsync(sp =>
                sp.Id == request.Id
                && sp.UserId == currentUserId);
        if (requestedPool == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundSwimmingPool);
        }

        requestedPool.Name = request.Name;
        requestedPool.Length = request.Length;
        requestedPool.Location = request.Location;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveSwimmingPoolAsync(int id, string currentUserId)
    {
        SwimmingPool? requestedPool = await _swimmingPoolRepository
            .All()
            .FirstOrDefaultAsync(sp =>
                sp.Id == id
                && sp.UserId == currentUserId);

        if (requestedPool == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundSwimmingPool);
        }

        IEnumerable<TrainingPlan> referencingTrainingPlans = await _trainingPlanRepository
            .All()
            .Where(tp => tp.SwimmingPoolId == requestedPool.Id)
            .ToArrayAsync();

        foreach (TrainingPlan trainingPlan in referencingTrainingPlans)
        {
            trainingPlan.SwimmingPoolId = null;
            trainingPlan.SwimmingPool = null;
        }

        IEnumerable<WorkoutResult> referencingWorkoutResults = await _workoutResultRepository
            .All()
            .Where(wr => wr.SwimmingPoolId == requestedPool.Id)
            .ToArrayAsync();

        foreach (WorkoutResult workoutResult in referencingWorkoutResults)
        {
            workoutResult.SwimmingPoolId = null;
            workoutResult.SwimmingPool = null;
        }

        _swimmingPoolRepository.Remove(requestedPool);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<SwimmingPoolDetailsResponseModel?> GetSwimmingPoolDetailsAsync(int id, string currentUserId)
    {
        return await _swimmingPoolRepository
            .AllAsNoTracking()
            .Where(sp => sp.Id == id && sp.UserId == currentUserId)
            .Select(sp => new SwimmingPoolDetailsResponseModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Length = sp.Length,
                Location = sp.Location,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AllSwimmingPoolsResponseModel>> GetAllSwimmingPoolsAsync(string currentUserId)
    {
        return await _swimmingPoolRepository
            .AllAsNoTracking()
            .Where(sp => sp.UserId == currentUserId)
            .Select(sp => new AllSwimmingPoolsResponseModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Length = sp.Length,
                Location = sp.Location,
            })
            .ToArrayAsync();
    }
}
