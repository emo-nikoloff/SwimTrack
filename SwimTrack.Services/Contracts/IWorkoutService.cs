using SwimTrack.DTOs.Workout;
using SwimTrack.DTOs.WorkoutResult;

namespace SwimTrack.Services.Contracts;

public interface IWorkoutService
{
    Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id, string currentUserId);

    Task<bool> EditWorkoutAsync(int id, WorkoutInputModel workoutInputModel, string currentUserId);

    Task<WorkoutResultInputModel?> GetWorkoutForCompletionAsync(int id, string currentUserId);

    Task<bool> CompleteWorkoutAsync(int id, WorkoutResultInputModel workoutResultInputModel, string currentUserId);

    Task<bool> SkipWorkoutAsync(int id, string currentUserId);
}
