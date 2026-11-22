using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Contracts;
using SwimTrack.Services.Common.Results;
using SwimTrack.Services.Common.Utilities;
using SwimTrack.Services.Contracts;
using SwimTrack.Services.DTOs.TrainingPlan.Request;
using SwimTrack.Services.DTOs.TrainingPlan.Response;
using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ApplicationDefaults;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;
using static SwimTrack.Shared.Constants.ValidationMessages.TrainingPlanValidation;

namespace SwimTrack.Services;

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

    public async Task<ServiceResult> CreateTrainingPlanAsync(CreateTrainingPlanRequestModel request, string currentUserId)
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

        bool isTrainingPlanPeriodValid = DateValidator.ValidatePlanPeriod(
            request.StartDate,
            request.EndDate);
        if (!isTrainingPlanPeriodValid)
        {
            string propertyName = nameof(CreateTrainingPlanRequestModel.EndDate);
            string errorMessage = InvalidTrainingPlanPeriod;

            ServiceError serviceError = new(propertyName, errorMessage);

            serviceErrors.Add(serviceError);
        }

        for (int i = 0; i < request.Workouts.Count; i++)
        {
            CreateWorkoutRequestModel workout = request.Workouts[i];

            bool isWorkoutRequestValid = _modelValidator.IsValid(workout);
            if (!isWorkoutRequestValid)
            {
                IReadOnlyCollection<ValidationResult> inputErrors = _modelValidator.ValidationResults;
                foreach (ValidationResult error in inputErrors)
                {
                    string? propertyName = error.MemberNames.FirstOrDefault();
                    if (propertyName != null)
                    {
                        propertyName = $"Workouts[{i}].{propertyName}";
                    }
                    else
                    {
                        propertyName = $"Workouts[{i}]";
                    }
                    string errorMessage = error.ErrorMessage ?? string.Empty;

                    ServiceError serviceError = new(propertyName, errorMessage);

                    serviceErrors.Add(serviceError);
                }
            }

            if (isTrainingPlanPeriodValid)
            {
                bool isWorkoutDateInPlanPeriod = DateValidator.ValidateWorkoutDate(
                    workout.Date,
                    request.StartDate,
                    request.EndDate);
                if (!isWorkoutDateInPlanPeriod)
                {
                    string propertyName = $"Workouts[{i}].{nameof(workout.Date)}";
                    string errorMessage = WorkoutDateOutsidePlanPeriod;

                    ServiceError serviceError = new(propertyName, errorMessage);

                    serviceErrors.Add(serviceError);
                }
            }
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
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

        return ServiceResult.Success();
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

    public async Task<ServiceResult> EditTrainingPlanAsync(EditTrainingPlanRequestModel request, string currentUserId)
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

        bool isTrainingPlanPeriodValid = DateValidator.ValidatePlanPeriod(
            request.StartDate,
            request.EndDate);
        if (!isTrainingPlanPeriodValid)
        {
            string propertyName = nameof(EditTrainingPlanRequestModel.EndDate);
            string errorMessage = InvalidTrainingPlanPeriod;

            ServiceError serviceError = new(propertyName, errorMessage);

            serviceErrors.Add(serviceError);
        }

        for (int i = 0; i < request.NewWorkouts.Count; i++)
        {
            CreateWorkoutRequestModel workout = request.NewWorkouts[i];

            bool isWorkoutRequestValid = _modelValidator.IsValid(workout);
            if (!isWorkoutRequestValid)
            {
                IReadOnlyCollection<ValidationResult> inputErrors = _modelValidator.ValidationResults;
                foreach (ValidationResult error in inputErrors)
                {
                    string? propertyName = error.MemberNames.FirstOrDefault();
                    if (propertyName != null)
                    {
                        propertyName = $"NewWorkouts[{i}].{propertyName}";
                    }
                    else
                    {
                        propertyName = $"NewWorkouts[{i}]";
                    }
                    string errorMessage = error.ErrorMessage ?? string.Empty;

                    ServiceError serviceError = new(propertyName, errorMessage);

                    serviceErrors.Add(serviceError);
                }
            }

            if (isTrainingPlanPeriodValid)
            {
                bool isWorkoutDateInPlanPeriod = DateValidator.ValidateWorkoutDate(
                    workout.Date,
                    request.StartDate,
                    request.EndDate);
                if (!isWorkoutDateInPlanPeriod)
                {
                    string propertyName = $"NewWorkouts[{i}].{nameof(workout.Date)}";
                    string errorMessage = WorkoutDateOutsidePlanPeriod;

                    ServiceError serviceError = new(propertyName, errorMessage);

                    serviceErrors.Add(serviceError);
                }
            }
        }

        if (serviceErrors.Count > 0)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                serviceErrors);
        }

        TrainingPlan? requestedTrainingPlan =
            await _trainingPlanRepository.All()
                .Include(tp => tp.Workouts)
                .FirstOrDefaultAsync(tp =>
                    tp.Id == request.Id
                    && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundTrainingPlan);
        }

        HashSet<int> existingWorkoutIds = requestedTrainingPlan.Workouts
            .Select(w => w.Id)
            .ToHashSet();

        HashSet<int> keptWorkoutIds = request.KeptWorkoutIds.ToHashSet();

        // Security check: ensure kept workout IDs actually belong to this training plan
        if (!keptWorkoutIds.IsSubsetOf(existingWorkoutIds))
        {
            return ServiceResult.Failure(
                ServiceResultStatus.InvalidRequest,
                InvalidTrainingPlanWorkout);
        }

        // Validate that kept workouts' dates in DB fall within the (possibly updated) plan period
        IEnumerable<Workout> workoutsToKeep = requestedTrainingPlan.Workouts
            .Where(w => keptWorkoutIds.Contains(w.Id));

        foreach (Workout workout in workoutsToKeep)
        {
            bool isWorkoutDateValid = DateValidator.ValidateWorkoutDate(
                workout.Date,
                request.StartDate,
                request.EndDate);
            if (!isWorkoutDateValid)
            {
                string? propertyName = null;
                string errorMessage = string.Format(
                    ExistingWorkoutOutsidePlanPeriod,
                    workout.Date.ToString(DateTimeFormat));

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

        // Update plan details
        requestedTrainingPlan.Name = request.Name;
        requestedTrainingPlan.Goal = request.Goal;
        requestedTrainingPlan.StartDate = request.StartDate;
        requestedTrainingPlan.EndDate = request.EndDate;

        // Delete workouts that were removed by the user
        IEnumerable<Workout> workoutsToRemove = requestedTrainingPlan.Workouts
            .Where(w => !keptWorkoutIds.Contains(w.Id))
            .ToArray();

        _workoutRepository.RemoveRange(workoutsToRemove);

        // Add newly created workouts
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

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveTrainingPlanAsync(int id, string currentUserId)
    {
        TrainingPlan? requestedTrainingPlan =
            await _trainingPlanRepository.All()
                .FirstOrDefaultAsync(tp =>
                    tp.Id == id
                    && tp.UserId == currentUserId);
        if (requestedTrainingPlan == null)
        {
            return ServiceResult.Failure(
                ServiceResultStatus.NotFound,
                NotFoundTrainingPlan);
        }

        _trainingPlanRepository.Remove(requestedTrainingPlan);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
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
