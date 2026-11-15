using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Request;
using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Shared.Enums;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Services.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;

    private readonly IWorkoutResultRepository _workoutResultRepository;

    private readonly IUnitOfWork _unitOfWork;

    public WorkoutService(IWorkoutRepository workoutRepository, IWorkoutResultRepository workoutResultRepository, IUnitOfWork unitOfWork)
    {
        _workoutRepository = workoutRepository;
        _workoutResultRepository = workoutResultRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<bool> EditWorkoutAsync(int trainingPlanId, EditWorkoutRequestModel request, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .Include(w => w.TrainingPlan)
                .FirstOrDefaultAsync(w =>
                    w.Id == request.Id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status == WorkoutStatus.Planned)
        {
            bool isWorkoutDateValid = DateValidator.ValidateWorkoutDate(
                request.Date,
                requestedWorkout.TrainingPlan.StartDate,
                requestedWorkout.TrainingPlan.EndDate);
            if (!isWorkoutDateValid)
            {
                throw new ValidationException(new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                        {
                            nameof(EditWorkoutRequestModel.Date)
                        }),
                    null,
                    null);
            }

            requestedWorkout.Date = request.Date;
            requestedWorkout.PlannedDistance = request.PlannedDistance;
            requestedWorkout.PlannedDuration = request.PlannedDuration;
        }
        requestedWorkout.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync();

        return true;
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

    public async Task<bool> CompleteWorkoutAsync(int id, int trainingPlanId, CreateWorkoutResultRequestModel request, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned || requestedWorkout.Date.Date > DateTime.Today)
        {
            return false;
        }

        bool isWorkoutCompletionDateValid = DateValidator.ValidateWorkoutCompletionDate(
            request.CompletedAt,
            requestedWorkout.Date);
        if (!isWorkoutCompletionDateValid)
        {
            throw new ValidationException(new ValidationResult(
                InvalidWorkoutCompletionDate,
                new[]
                    {
                        nameof(CreateWorkoutResultRequestModel.CompletedAt)
                    }),
                null,
                null);
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

        return true;
    }

    public async Task<bool> SkipWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned || requestedWorkout.Date.Date > DateTime.Today)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Skipped;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PlanAgainWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout =
            await _workoutRepository.All()
                .FirstOrDefaultAsync(w =>
                    w.Id == id
                    && w.TrainingPlanId == trainingPlanId
                    && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Skipped)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Planned;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
