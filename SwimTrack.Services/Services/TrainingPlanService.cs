using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data;
using SwimTrack.Data.Models;
using SwimTrack.DTOs.TrainingPlan;
using SwimTrack.DTOs.Workout;
using SwimTrack.Services.Contracts;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Services.Services;

public class TrainingPlanService : ITrainingPlanService
{
    private readonly SwimTrackDbContext _dbContext;

    public TrainingPlanService(SwimTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TrainingPlanViewModel>> GetAllTrainingPlansAsync(string currentUserId)
    {
        return await _dbContext.TrainingPlans
            .Where(tp => tp.UserId == currentUserId)
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
                        Status = w.Status,
                    })
                    .ToArray(),
            })
            .ToArrayAsync();
    }

    public async Task CreateTrainingPlanAsync(TrainingPlanInputModel trainingPlanInputModel, string currentUserId)
    {
        ValidateTrainingPlanDates(trainingPlanInputModel);

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
            UserId = currentUserId,
        };

        await _dbContext.TrainingPlans.AddAsync(newTrainingPlan);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TrainingPlanInputModel?> GetTrainingPlanForEditAsync(int id, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans
            .Include(tp => tp.Workouts)
            .FirstOrDefaultAsync(tp =>
                tp.Id == id
                && tp.UserId == currentUserId);
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
                    Id = w.Id,
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                })
                .ToArray(),
        };

        return trainingPlanInputModel;
    }

    public async Task<bool> EditTrainingPlanAsync(int id, TrainingPlanInputModel trainingPlanInputModel, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans
            .Include(tp => tp.Workouts)
            .FirstOrDefaultAsync(tp =>
                tp.Id == id
                && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        ValidateTrainingPlanDates(trainingPlanInputModel);

        requestedTrainingPlan.Name = trainingPlanInputModel.Name;
        requestedTrainingPlan.Goal = trainingPlanInputModel.Goal;
        requestedTrainingPlan.StartDate = trainingPlanInputModel.StartDate;
        requestedTrainingPlan.EndDate = trainingPlanInputModel.EndDate;

        IEnumerable<int> submittedWorkoutIds = trainingPlanInputModel.Workouts
            .Where(w => w.Id.HasValue)
            .Select(w => w.Id!.Value)
            .ToHashSet();

        IEnumerable<Workout> workoutsToRemove = requestedTrainingPlan.Workouts
            .Where(w => !submittedWorkoutIds.Contains(w.Id))
            .ToArray();

        _dbContext.Workouts.RemoveRange(workoutsToRemove);

        foreach (WorkoutInputModel workoutInputModel in trainingPlanInputModel.Workouts)
        {
            if (workoutInputModel.Id.HasValue)
            {
                Workout? existingWorkout = requestedTrainingPlan.Workouts
                    .FirstOrDefault(workout => workout.Id == workoutInputModel.Id.Value);
                if (existingWorkout == null)
                {
                    return false;
                }

                existingWorkout.Date = workoutInputModel.Date;
                existingWorkout.PlannedDistance = workoutInputModel.PlannedDistance;
                existingWorkout.PlannedDuration = workoutInputModel.PlannedDuration;
                existingWorkout.Notes = workoutInputModel.Notes;

                continue;
            }

            requestedTrainingPlan.Workouts.Add(new Workout
            {
                Date = workoutInputModel.Date,
                PlannedDistance = workoutInputModel.PlannedDistance,
                PlannedDuration = workoutInputModel.PlannedDuration,
                Notes = workoutInputModel.Notes,
            });
        }

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveTrainingPlanAsync(int id, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan = await _dbContext.TrainingPlans
            .FirstOrDefaultAsync(tp =>
                tp.Id == id
                && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        _dbContext.TrainingPlans.Remove(requestedTrainingPlan);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<TrainingPlanViewModel?> GetTrainingPlanDetailsAsync(int id, string currentUserId)
    {
        return await _dbContext.TrainingPlans
            .Where(tp => tp.Id == id && tp.UserId == currentUserId)
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
                        Status = w.Status,
                    })
                    .ToArray(),
            })
            .FirstOrDefaultAsync();
    }

    private static void ValidateTrainingPlanDates(TrainingPlanInputModel trainingPlanInputModel)
    {
        bool isTrainingPlanPeriodValid = DateValidator.ValidatePlanPeriod(
            trainingPlanInputModel.StartDate,
            trainingPlanInputModel.EndDate);
        if (!isTrainingPlanPeriodValid)
        {
            throw new ValidationException(new ValidationResult(
                InvalidTrainingPlanPeriod,
                new[]
                    {
                        nameof(TrainingPlanInputModel.EndDate)
                    }),
                null,
                null);
        }

        List<WorkoutInputModel> workouts = trainingPlanInputModel.Workouts.ToList();
        for (int i = 0; i < workouts.Count; i++)
        {
            bool isWorkoutDateValid = DateValidator.ValidateWorkoutDate(
                workouts[i].Date,
                trainingPlanInputModel.StartDate,
                trainingPlanInputModel.EndDate);
            if (!isWorkoutDateValid)
            {
                throw new ValidationException(new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                        {
                            $"{nameof(TrainingPlanInputModel.Workouts)}[{i}].{nameof(WorkoutInputModel.Date)}"
                        }),
                    null,
                    null);
            }
        }
    }
}
