using Microsoft.EntityFrameworkCore;

using SwimTrack.Data;
using SwimTrack.Data.Models;
using SwimTrack.DTOs.Workout;
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

    public async Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(w => w.Id == id);
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
        };

        return workoutInputModel;
    }

    public async Task<bool> EditWorkoutAsync(int id, WorkoutInputModel workoutInputModel)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FindAsync(id);
        if (requestedWorkout == null)
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

    public async Task<bool> CompleteWorkoutAsync(int id)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FindAsync(id);
        if (requestedWorkout == null)
        {
            return false;
        }

        if (requestedWorkout.Status != WorkoutStatus.Planned)
        {
            return false;
        }

        requestedWorkout.Status = WorkoutStatus.Completed;
        // TODO: Create a new record for WorkoutResults table

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SkipWorkoutAsync(int id)
    {
        Workout? requestedWorkout = await _dbContext.Workouts.FindAsync(id);
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
}
