using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data;
using SwimTrack.Data.Models;
using SwimTrack.DTOs.Workout;
using SwimTrack.DTOs.WorkoutResult;
using SwimTrack.Services.Contracts;
using SwimTrack.Shared.Enums;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Services.Services;

public class WorkoutService : IWorkoutService
{
    private readonly SwimTrackDbContext _dbContext;

    public WorkoutService(SwimTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w =>
            w.Id == id
            && w.TrainingPlanId == trainingPlanId
            && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return null;
        }

        WorkoutInputModel workoutInputModel = new()
        {
            Date = requestedWorkout.Date,
            PlannedDistance = requestedWorkout.PlannedDistance,
            PlannedDuration = requestedWorkout.PlannedDuration,
            Notes = requestedWorkout.Notes,
            Status = requestedWorkout.Status,
        };

        return workoutInputModel;
    }

    public async Task<bool> EditWorkoutAsync(int id, int trainingPlanId, WorkoutInputModel workoutInputModel, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w =>
                w.Id == id
                && w.TrainingPlanId == trainingPlanId
                && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        requestedWorkout.Notes = workoutInputModel.Notes;

        if (requestedWorkout.Status == WorkoutStatus.Planned)
        {
            bool isWorkoutDateValid = DateValidator.ValidateWorkoutDate(
                workoutInputModel.Date,
                requestedWorkout.TrainingPlan.StartDate,
                requestedWorkout.TrainingPlan.EndDate);
            if (!isWorkoutDateValid)
            {
                throw new ValidationException(new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                        {
                            nameof(WorkoutInputModel.Date)
                        }),
                    null,
                    null);
            }

            requestedWorkout.Date = workoutInputModel.Date;
            requestedWorkout.PlannedDistance = workoutInputModel.PlannedDistance;
            requestedWorkout.PlannedDuration = workoutInputModel.PlannedDuration;
        }

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<WorkoutResultInputModel?> GetWorkoutForCompletionAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w =>
            w.Id == id
            && w.TrainingPlanId == trainingPlanId
            && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return null;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return null;
        }

        WorkoutResultInputModel workoutResultInputModel = new()
        {
            ActualDistance = requestedWorkout.PlannedDistance,
            ActualDuration = requestedWorkout.PlannedDuration,
            Notes = requestedWorkout.Notes,
        };

        return workoutResultInputModel;
    }

    public async Task<bool> CompleteWorkoutAsync(int id, int trainingPlanId, WorkoutResultInputModel workoutResultInputModel, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w =>
            w.Id == id
            && w.TrainingPlanId == trainingPlanId
            && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Completed;

        bool isWorkoutCompletionDateValid = DateValidator.ValidateWorkoutCompletionDate(
            workoutResultInputModel.CompletedAt,
            requestedWorkout.Date);
        if (!isWorkoutCompletionDateValid)
        {
            throw new ValidationException(new ValidationResult(
                InvalidWorkoutCompletionDate,
                new[]
                    {
                        nameof(WorkoutResultInputModel.CompletedAt)
                    }),
                null,
                null);
        }

        WorkoutResult workoutResult = new()
        {
            ActualDistance = workoutResultInputModel.ActualDistance,
            ActualDuration = workoutResultInputModel.ActualDuration,
            CompletedAt = workoutResultInputModel.CompletedAt,
            Notes = workoutResultInputModel.Notes,
            WorkoutId = requestedWorkout.Id
        };

        _dbContext.WorkoutResults.Add(workoutResult);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SkipWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w =>
            w.Id == id
            && w.TrainingPlanId == trainingPlanId
            && w.TrainingPlan.UserId == currentUserId);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Skipped;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PlanAgainWorkoutAsync(int id, int trainingPlanId, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w =>
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

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
