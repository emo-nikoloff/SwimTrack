using Microsoft.EntityFrameworkCore;

using SwimTrack.Data;
using SwimTrack.Data.Models;
using SwimTrack.DTOs.TrainingPlan;
using SwimTrack.DTOs.Workout;
using SwimTrack.Services.Contracts;

namespace SwimTrack.Services.Services;

public class TrainingPlanService : ITrainingPlanService
{
    private readonly SwimTrackDbContext _dbContext;

    public TrainingPlanService(SwimTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TrainingPlanViewModel>> GetAllTrainingPlansAsync()
    {
        return await _dbContext.TrainingPlans
            .Select(tp => new TrainingPlanViewModel
            {
                Id = tp.Id,
                Name = tp.Name,
                Goal = tp.Goal,
                StartDate = tp.StartDate,
                EndDate = tp.EndDate,
                Workouts = tp.Workouts
                    .Select(w => new WorkoutViewModel
                    {
                        Id = w.Id,
                        Date = w.Date,
                        PlannedDistance = w.PlannedDistance,
                        PlannedDuration = w.PlannedDuration,
                        Notes = w.Notes,
                    })
                    .ToArray(),
            })
            .ToArrayAsync();
    }

    public async Task CreateTrainingPlanAsync(TrainingPlanInputModel trainingPlanInputModel)
    {
        TrainingPlan newTrainingPlan = new()
        {
            Name = trainingPlanInputModel.Name,
            Goal = trainingPlanInputModel.Goal,
            StartDate = trainingPlanInputModel.StartDate,
            EndDate = trainingPlanInputModel.EndDate,
            Workouts = trainingPlanInputModel.Workouts
                .Select(workoutInputModel => new Workout
                {
                    Date = workoutInputModel.Date,
                    PlannedDistance = workoutInputModel.PlannedDistance,
                    PlannedDuration = workoutInputModel.PlannedDuration,
                    Notes = workoutInputModel.Notes,
                })
                .ToList(),
        };

        await _dbContext.TrainingPlans.AddAsync(newTrainingPlan);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TrainingPlanInputModel?> GetTrainingPlanForEditAsync(int id)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans
            .Include(tp => tp.Workouts)
            .FirstOrDefaultAsync(tp => tp.Id == id);
        if (requestedTrainingPlan == null)
        {
            return null;
        }

        TrainingPlanInputModel trainingPlanInputModel = new()
        {
            Name = requestedTrainingPlan.Name,
            Goal = requestedTrainingPlan.Goal,
            StartDate = requestedTrainingPlan.StartDate,
            EndDate = requestedTrainingPlan.EndDate,
            Workouts = requestedTrainingPlan.Workouts
                .Select(w => new WorkoutInputModel
                {
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                })
                .ToArray(),
        };

        return trainingPlanInputModel;
    }

    public async Task<bool> EditTrainingPlanAsync(int id, TrainingPlanInputModel trainingPlanInputModel)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans.FindAsync(id);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        requestedTrainingPlan.Name = trainingPlanInputModel.Name;
        requestedTrainingPlan.Goal = trainingPlanInputModel.Goal;
        requestedTrainingPlan.StartDate = trainingPlanInputModel.StartDate;
        requestedTrainingPlan.EndDate = trainingPlanInputModel.EndDate;
        requestedTrainingPlan.Workouts = trainingPlanInputModel.Workouts
            .Select(workoutInputModel => new Workout
            {
                Date = workoutInputModel.Date,
                PlannedDistance = workoutInputModel.PlannedDistance,
                PlannedDuration = workoutInputModel.PlannedDuration,
                Notes = workoutInputModel.Notes,
            })
            .ToArray();

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveTrainingPlanAsync(int id)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans.FindAsync(id);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        _dbContext.TrainingPlans.Remove(requestedTrainingPlan);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<TrainingPlanViewModel?> GetTrainingPlanDetailsAsync(int id)
    {
        return await _dbContext.TrainingPlans
            .Where(tp => tp.Id == id)
            .Select(tp => new TrainingPlanViewModel
            {
                Id = tp.Id,
                Name = tp.Name,
                Goal = tp.Goal,
                StartDate = tp.StartDate,
                EndDate = tp.EndDate,
                Workouts = tp.Workouts
                    .Select(w => new WorkoutViewModel
                    {
                        Id = w.Id,
                        Date = w.Date,
                        PlannedDistance = w.PlannedDistance,
                        PlannedDuration = w.PlannedDuration,
                        Notes = w.Notes,
                    })
                    .ToArray(),
            })
            .FirstOrDefaultAsync();
    }
}
