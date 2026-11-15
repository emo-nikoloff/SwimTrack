using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Common.Utilities;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.TrainingPlan.Request;
using SwimTrack.Services.DTOs.TrainingPlan.Response;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Response;

namespace SwimTrack.Services.Services;

public class TrainingPlanService : ITrainingPlanService
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;

    private readonly IWorkoutRepository _workoutRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly BasicModelValidator _modelValidator;

    public TrainingPlanService(ITrainingPlanRepository trainingPlanRepository, IWorkoutRepository workoutRepository, IUnitOfWork unitOfWork)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _workoutRepository = workoutRepository;
        _unitOfWork = unitOfWork;
        _modelValidator = new();
    }

    public async Task<IEnumerable<AllTrainingPlansResponseModel>> GetAllTrainingPlansAsync(string currentUserId)
    {
        return await _trainingPlanRepository.AllAsNoTracking()
            .Where(tp => tp.UserId == currentUserId)
            .Select(tp => new AllTrainingPlansResponseModel
            {
                Id = tp.Id,
                Name = tp.Name,
                Goal = tp.Goal,
                StartDate = tp.StartDate,
                EndDate = tp.EndDate,
                Workouts = tp.Workouts
                    .Select(w => new WorkoutResponseModel
                    {
                        Id = w.Id,
                        Date = w.Date,
                        PlannedDistance = w.PlannedDistance,
                        PlannedDuration = w.PlannedDuration,
                        Notes = w.Notes,
                        Status = w.Status,
                        Result = w.WorkoutResult == null
                            ? null
                            : new WorkoutResultResponseModel
                            {
                                ActualDistance = w.WorkoutResult.ActualDistance,
                                ActualDuration = w.WorkoutResult.ActualDuration,
                                CompletedAt = w.WorkoutResult.CompletedAt,
                                Notes = w.WorkoutResult.Notes,
                            },
                    })
                    .ToArray(),
            })
            .ToArrayAsync();
    }

    public async Task CreateTrainingPlanAsync(CreateTrainingPlanRequestModel request, string currentUserId)
    {
        bool isInputValid = _modelValidator.IsValid(request);
        if (!isInputValid)
        {
            return;
        }

        bool hasInvalidWorkoutInput = request.Workouts
            .Any(w => !_modelValidator.IsValid(w));
        if (hasInvalidWorkoutInput)
        {
            return;
        }

        TrainingPlan newTrainingPlan = new()
        {
            Name = request.Name,
            Goal = request.Goal,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Workouts = request.Workouts
                .Select(workoutInputModel => new Workout
                {
                    Date = workoutInputModel.Date,
                    PlannedDistance = workoutInputModel.PlannedDistance,
                    PlannedDuration = workoutInputModel.PlannedDuration,
                    Notes = workoutInputModel.Notes,
                })
                .ToArray(),
            UserId = currentUserId,
        };

        await _trainingPlanRepository.AddAsync(newTrainingPlan);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<EditTrainingPlanResponseModel?> GetTrainingPlanForEditAsync(int id, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan =
            await _trainingPlanRepository.AllAsNoTracking()
                .Include(tp => tp.Workouts)
                .FirstOrDefaultAsync(tp =>
                    tp.Id == id
                    && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return null;
        }

        EditTrainingPlanResponseModel editTrainingPlanResponse = new()
        {
            Id = requestedTrainingPlan.Id,
            Name = requestedTrainingPlan.Name,
            Goal = requestedTrainingPlan.Goal,
            StartDate = requestedTrainingPlan.StartDate,
            EndDate = requestedTrainingPlan.EndDate,
            Workouts = requestedTrainingPlan.Workouts
                .Select(w => new EditWorkoutResponseModel
                {
                    Id = w.Id,
                    Date = w.Date,
                    PlannedDistance = w.PlannedDistance,
                    PlannedDuration = w.PlannedDuration,
                    Notes = w.Notes,
                    Status = w.Status,
                })
                .ToArray(),
        };

        return editTrainingPlanResponse;
    }

    public async Task<bool> EditTrainingPlanAsync(EditTrainingPlanRequestModel request, string currentUserId)
    {
        bool isInputValid = _modelValidator.IsValid(request);
        if (!isInputValid)
        {
            return false;
        }

        bool hasInvalidWorkoutInput = request.NewWorkouts
            .Any(w => !_modelValidator.IsValid(w));
        if (hasInvalidWorkoutInput)
        {
            return false;
        }

        TrainingPlan? requestedTrainingPlan =
            await _trainingPlanRepository.All()
                .Include(tp => tp.Workouts)
                .FirstOrDefaultAsync(tp =>
                    tp.Id == request.Id
                    && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        HashSet<int> workoutIdsToKeep = request.WorkoutIdsToKeep
            .ToHashSet();

        if (workoutIdsToKeep.Count != request.WorkoutIdsToKeep.Count)
        {
            return false;
        }

        HashSet<int> existingWorkoutIds = requestedTrainingPlan.Workouts
            .Select(w => w.Id)
            .ToHashSet();

        if (!workoutIdsToKeep.IsSubsetOf(existingWorkoutIds))
        {
            return false;
        }

        IEnumerable<Workout> workoutsToRemove = requestedTrainingPlan.Workouts
            .Where(w => !workoutIdsToKeep.Contains(w.Id))
            .ToArray();

        requestedTrainingPlan.Name = request.Name;
        requestedTrainingPlan.Goal = request.Goal;
        requestedTrainingPlan.StartDate = request.StartDate;
        requestedTrainingPlan.EndDate = request.EndDate;

        _workoutRepository.RemoveRange(workoutsToRemove);

        foreach (CreateWorkoutRequestModel workoutRequest in request.NewWorkouts)
        {
            requestedTrainingPlan.Workouts.Add(new Workout
            {
                Date = workoutRequest.Date,
                PlannedDistance = workoutRequest.PlannedDistance,
                PlannedDuration = workoutRequest.PlannedDuration,
                Notes = workoutRequest.Notes,
            });
        }

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveTrainingPlanAsync(int id, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan =
            await _trainingPlanRepository.All()
                .FirstOrDefaultAsync(tp =>
                    tp.Id == id
                    && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return false;
        }

        _trainingPlanRepository.Remove(requestedTrainingPlan);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<TrainingPlanDetailsResponseModel?> GetTrainingPlanDetailsAsync(int id, string currentUserId)
    {
        return await _trainingPlanRepository.AllAsNoTracking()
            .Where(tp => tp.Id == id && tp.UserId == currentUserId)
            .Select(tp => new TrainingPlanDetailsResponseModel
            {
                Id = tp.Id,
                Name = tp.Name,
                Goal = tp.Goal,
                StartDate = tp.StartDate,
                EndDate = tp.EndDate,
                Workouts = tp.Workouts
                    .Select(w => new WorkoutResponseModel
                    {
                        Id = w.Id,
                        Date = w.Date,
                        PlannedDistance = w.PlannedDistance,
                        PlannedDuration = w.PlannedDuration,
                        Notes = w.Notes,
                        Status = w.Status,
                        Result = w.WorkoutResult == null
                            ? null
                            : new WorkoutResultResponseModel
                            {
                                ActualDistance = w.WorkoutResult.ActualDistance,
                                ActualDuration = w.WorkoutResult.ActualDuration,
                                CompletedAt = w.WorkoutResult.CompletedAt,
                                Notes = w.WorkoutResult.Notes,
                            },
                    })
                    .ToArray(),
            })
            .FirstOrDefaultAsync();
    }
}
