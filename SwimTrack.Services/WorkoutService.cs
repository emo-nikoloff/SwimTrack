using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Common.Utilities;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Request;
using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Shared.Enums;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;
using static SwimTrack.Shared.Constants.ValidationMessages.WorkoutValidation;

namespace SwimTrack.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;

    private readonly IWorkoutResultRepository _workoutResultRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly BasicModelValidator _modelValidator;

    public WorkoutService(IWorkoutRepository workoutRepository, IWorkoutResultRepository workoutResultRepository, IUnitOfWork unitOfWork)
    {
        _workoutRepository = workoutRepository;
        _workoutResultRepository = workoutResultRepository;
        _unitOfWork = unitOfWork;
        _modelValidator = new();
    }

    public async Task<EditWorkoutResponseModel?> GetWorkoutForEditAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return null;
        }

        EditWorkoutResponseModel editWorkoutResponse = new()
        {
            Id = requestedWorkout.Id,
            Date = requestedWorkout.Date,
            PlannedDistance = requestedWorkout.PlannedDistance,
            PlannedDuration = requestedWorkout.PlannedDuration,
            Notes = requestedWorkout.Notes,
            Status = requestedWorkout.Status,
        };

        return editWorkoutResponse;
    }

    public async Task<ServiceResult> EditWorkoutAsync(int trainingPlanId, EditWorkoutRequestModel request, string currentUserId)
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

        Workout? requestedWorkout =
            await _workoutRepository.All()
                .Include(w => w.TrainingPlan)
                .FirstOrDefaultAsync(w =>
                    w.Id == request.Id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundWorkout);
        }

        if (requestedWorkout.Status == WorkoutStatus.Planned)
        {
            bool isWorkoutDateValid = DateValidator.ValidateWorkoutDate(
                request.Date,
                requestedWorkout.TrainingPlan.StartDate,
                requestedWorkout.TrainingPlan.EndDate);
            if (!isWorkoutDateValid)
            {
                string propertyName = nameof(EditWorkoutRequestModel.Date);
                string errorMessage = WorkoutDateOutsidePlanPeriod;

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

        if (requestedWorkout.Status == WorkoutStatus.Planned)
        {
            requestedWorkout.Date = request.Date;
            requestedWorkout.PlannedDistance = request.PlannedDistance;
            requestedWorkout.PlannedDuration = request.PlannedDuration;
        }
        requestedWorkout.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<WorkoutResultResponseModel?> GetWorkoutForCompletionAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return null;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned || requestedWorkout.Date.Date > DateTime.Today)
        {
            return null;
        }

        WorkoutResultResponseModel workoutResultResponse = new()
        {
            ActualDistance = requestedWorkout.PlannedDistance,
            ActualDuration = requestedWorkout.PlannedDuration,
            CompletedAt = requestedWorkout.Date,
            Notes = requestedWorkout.Notes,
        };

        return workoutResultResponse;
    }

    public async Task<ServiceResult> CompleteWorkoutAsync(int id, int trainingPlanId, CreateWorkoutResultRequestModel request, string currentUserId)
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

        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundWorkout);
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidOperation,
                CannotCompleteWorkoutDueToStatus);
        }

        if (requestedWorkout.Date.Date > DateTime.Today)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidOperation,
                CannotCompleteWorkoutInFuture);
        }

        bool isWorkoutCompletionDateValid = DateValidator.ValidateWorkoutCompletionDate(
            request.CompletedAt,
            requestedWorkout.Date);
        if (!isWorkoutCompletionDateValid)
        {
            string propertyName = nameof(CreateWorkoutResultRequestModel.CompletedAt);
            string errorMessage = InvalidWorkoutCompletionDate;

            ServiceError serviceError = new(propertyName, errorMessage);

            serviceErrors.Add(serviceError);
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        requestedWorkout.Status = WorkoutStatus.Completed;

        WorkoutResult newWorkoutResult = new()
        {
            ActualDistance = request.ActualDistance,
            ActualDuration = request.ActualDuration,
            CompletedAt = request.CompletedAt,
            Notes = request.Notes,
            WorkoutId = requestedWorkout.Id
        };

        _workoutResultRepository.Add(newWorkoutResult);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SkipWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundWorkout);
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidOperation,
                CannotSkipWorkoutDueToStatus);
        }

        if (requestedWorkout.Date.Date > DateTime.Today)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidOperation,
                CannotSkipWorkoutInFuture);
        }

        requestedWorkout.Status = WorkoutStatus.Skipped;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> PlanAgainWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundWorkout);
        }

        if (requestedWorkout.Status != WorkoutStatus.Skipped)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidOperation,
                CannotPlanAgainWorkoutDueToStatus);
        }

        requestedWorkout.Status = WorkoutStatus.Planned;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
