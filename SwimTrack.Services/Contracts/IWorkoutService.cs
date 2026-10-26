using SwimTrack.DTOs.Workout;
using SwimTrack.DTOs.WorkoutResult;

namespace SwimTrack.Services.Contracts;

public interface IWorkoutService
{
    Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> EditWorkoutAsync(int id, int trainingPlanId, WorkoutInputModel workoutInputModel, string currentUserId);

    Task<WorkoutResultInputModel?> GetWorkoutForCompletionAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> CompleteWorkoutAsync(int id, int trainingPlanId, WorkoutResultInputModel workoutResultInputModel, string currentUserId);

    Task<bool> SkipWorkoutAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> PlanAgainWorkoutAsync(int id, int trainingPlanId, string currentUserId);
}
