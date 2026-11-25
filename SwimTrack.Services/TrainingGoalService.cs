using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Common.Utilities;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.TrainingGoal.Request;
using SwimTrack.Services.DTOs.TrainingGoal.Response;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;
using static SwimTrack.Shared.Constants.ValidationMessages.TrainingGoalValidation;

namespace SwimTrack.Services;

public class TrainingGoalService : ITrainingGoalService
{
    private readonly ITrainingGoalRepository _trainingGoalRepository;

    private readonly ITrainingPlanRepository _trainingPlanRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly BasicModelValidator _modelValidator;

    public TrainingGoalService(
        ITrainingGoalRepository trainingGoalRepository,
        ITrainingPlanRepository trainingPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _trainingGoalRepository = trainingGoalRepository;
        _trainingPlanRepository = trainingPlanRepository;
        _unitOfWork = unitOfWork;
        _modelValidator = new();
    }

    public async Task<ServiceResult> CreateTrainingGoalAsync(CreateTrainingGoalRequestModel request, string currentUserId)
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

        bool isGoalPeriodValid = DateValidator.ValidatePeriod(
            request.StartDate,
            request.EndDate);
        if (!isGoalPeriodValid)
        {
            string propertyName = nameof(CreateTrainingGoalRequestModel.EndDate);
            string errorMessage = InvalidTrainingGoalPeriod;

            ServiceError serviceError = new(propertyName, errorMessage);

            serviceErrors.Add(serviceError);
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        TrainingGoal newTrainingGoal = new()
        {
            Title = request.Title,
            TargetDistance = request.TargetDistance,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            UserId = currentUserId,
        };

        await _trainingGoalRepository.AddAsync(newTrainingGoal);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<EditTrainingGoalResponseModel?> GetTrainingGoalForEditAsync(int id, string currentUserId)
    {
        TrainingGoal? requestedGoal = await _trainingGoalRepository
            .AllAsNoTracking()
            .FirstOrDefaultAsync(tg =>
                tg.Id == id &&
                tg.UserId == currentUserId);
        if (requestedGoal == null)
        {
            return null;
        }

        EditTrainingGoalResponseModel editTrainingGoalResponse = new()
        {
            Id = requestedGoal.Id,
            Title = requestedGoal.Title,
            TargetDistance = requestedGoal.TargetDistance,
            StartDate = requestedGoal.StartDate,
            EndDate = requestedGoal.EndDate,
        };

        return editTrainingGoalResponse;
    }

    public async Task<ServiceResult> EditTrainingGoalAsync(EditTrainingGoalRequestModel request, string currentUserId)
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

        bool isGoalPeriodValid = DateValidator.ValidatePeriod(
            request.StartDate,
            request.EndDate);
        if (!isGoalPeriodValid)
        {
            string propertyName = nameof(EditTrainingGoalRequestModel.EndDate);
            string errorMessage = InvalidTrainingGoalPeriod;

            ServiceError serviceError = new(propertyName, errorMessage);

            serviceErrors.Add(serviceError);
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        TrainingGoal? requestedGoal = await _trainingGoalRepository
            .All()
            .Include(tg => tg.TrainingPlans)
            .FirstOrDefaultAsync(tg =>
                tg.Id == request.Id &&
                tg.UserId == currentUserId);
        if (requestedGoal == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundTrainingGoal);
        }

        foreach (TrainingPlan trainingPlan in requestedGoal.TrainingPlans)
        {
            bool isPlanWithinNewGoalPeriod = DateValidator.ValidatePlanWithinGoalPeriod(
                trainingPlan.StartDate,
                trainingPlan.EndDate,
                request.StartDate,
                request.EndDate);
            if (!isPlanWithinNewGoalPeriod)
            {
                string? propertyName = null;
                string errorMessage = string.Format(
                    ExistingPlanOutsideGoalPeriod,
                    trainingPlan.Title);

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

        requestedGoal.Title = request.Title;
        requestedGoal.TargetDistance = request.TargetDistance;
        requestedGoal.StartDate = request.StartDate;
        requestedGoal.EndDate = request.EndDate;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveTrainingGoalAsync(int id, string currentUserId)
    {
        TrainingGoal? requestedGoal = await _trainingGoalRepository
            .All()
            .FirstOrDefaultAsync(tg =>
                tg.Id == id &&
                tg.UserId == currentUserId);
        if (requestedGoal == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundTrainingGoal);
        }

        IEnumerable<TrainingPlan> referencingTrainingPlans = await _trainingPlanRepository
            .All()
            .Where(tp =>
                tp.TrainingGoalId == id &&
                tp.UserId == currentUserId)
            .ToArrayAsync();
        foreach (TrainingPlan trainingPlan in referencingTrainingPlans)
        {
            trainingPlan.TrainingGoalId = null;
            trainingPlan.TrainingGoal = null;
        }

        _trainingGoalRepository.Remove(requestedGoal);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<TrainingGoalDetailsResponseModel?> GetTrainingGoalDetailsAsync(int id, string currentUserId)
    {
        return await _trainingGoalRepository
            .AllAsNoTracking()
            .Where(tg => tg.Id == id && tg.UserId == currentUserId)
            .Select(tg => new TrainingGoalDetailsResponseModel
            {
                Id = tg.Id,
                Title = tg.Title,
                TargetDistance = tg.TargetDistance,
                StartDate = tg.StartDate,
                EndDate = tg.EndDate,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AllTrainingGoalsResponseModel>> GetAllTrainingGoalsAsync(string currentUserId)
    {
        return await _trainingGoalRepository
            .AllAsNoTracking()
            .Where(tg => tg.UserId == currentUserId)
            .Select(tg => new AllTrainingGoalsResponseModel
            {
                Id = tg.Id,
                Title = tg.Title,
                TargetDistance = tg.TargetDistance,
                StartDate = tg.StartDate,
                EndDate = tg.EndDate,
            })
            .ToArrayAsync();
    }
}
