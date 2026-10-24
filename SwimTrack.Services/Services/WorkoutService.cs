using Microsoft.EntityFrameworkCore;

using SwimTrack.Data;
using SwimTrack.Data.Models;
using SwimTrack.DTOs.Workout;
using SwimTrack.DTOs.WorkoutResult;
using SwimTrack.Services.Contracts;
using SwimTrack.Shared.Enums;

namespace SwimTrack.Services.Services;

public class WorkoutService : IWorkoutService
{
    private readonly SwimTrackDbContext _dbContext;

    public WorkoutService(SwimTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (requestedWorkout == null)
        {
            return null;
        }

        if (requestedWorkout.TrainingPlan.UserId != currentUserId)
        {
            return null;
        }

        WorkoutInputModel workoutInputModel = new()
        {
            Date = requestedWorkout.Date,
            PlannedDistance = requestedWorkout.PlannedDistance,
            PlannedDuration = requestedWorkout.PlannedDuration,
            Notes = requestedWorkout.Notes,
        };

        return workoutInputModel;
    }

    public async Task<bool> EditWorkoutAsync(int id, WorkoutInputModel workoutInputModel, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.TrainingPlan.UserId != currentUserId)
        {
            return false;
        }

        requestedWorkout.Date = workoutInputModel.Date;
        requestedWorkout.PlannedDistance = workoutInputModel.PlannedDistance;
        requestedWorkout.PlannedDuration = workoutInputModel.PlannedDuration;
        requestedWorkout.Notes = workoutInputModel.Notes;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<WorkoutResultInputModel?> GetWorkoutForCompletionAsync(int id, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (requestedWorkout == null)
        {
            return null;
        }

        if (requestedWorkout.TrainingPlan.UserId != currentUserId)
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
            CompletedAt = DateTime.Now,
            Notes = requestedWorkout.Notes,
        };

        return workoutResultInputModel;
    }

    public async Task<bool> CompleteWorkoutAsync(int id, WorkoutResultInputModel workoutResultInputModel, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.TrainingPlan.UserId != currentUserId)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Completed;

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

    public async Task<bool> SkipWorkoutAsync(int id, string currentUserId)
    {
        Workout? requestedWorkout = await _dbContext.Workouts
            .Include(w => w.TrainingPlan)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.TrainingPlan.UserId != currentUserId)
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
}
