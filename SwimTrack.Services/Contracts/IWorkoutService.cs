using SwimTrack.Services.DTOs.Workout.Request;
using SwimTrack.Services.DTOs.Workout.Response;
using SwimTrack.Services.DTOs.WorkoutResult.Request;
using SwimTrack.Services.DTOs.WorkoutResult.Response;

namespace SwimTrack.Services.Contracts;

public interface IWorkoutService
{
    Task<EditWorkoutResponseModel?> GetWorkoutForEditAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> EditWorkoutAsync(int trainingPlanId, EditWorkoutRequestModel request, string currentUserId);

    Task<WorkoutResultResponseModel?> GetWorkoutForCompletionAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> CompleteWorkoutAsync(int id, int trainingPlanId, CreateWorkoutResultRequestModel request, string currentUserId);

    Task<bool> SkipWorkoutAsync(int id, int trainingPlanId, string currentUserId);

    Task<bool> PlanAgainWorkoutAsync(int id, int trainingPlanId, string currentUserId);
}
