using SwimTrack.DTOs.Workout;

namespace SwimTrack.Services.Contracts;

public interface IWorkoutService
{
    Task<WorkoutInputModel?> GetWorkoutForEditAsync(int id);

    Task<bool> EditWorkoutAsync(int id, WorkoutInputModel workoutInputModel);

    Task<bool> CompleteWorkoutAsync(int id);

    Task<bool> SkipWorkoutAsync(int id);
}
