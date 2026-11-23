using SwimTrack.Services.Common.Results;
using SwimTrack.Services.DTOs.TrainingGoal.Request;
using SwimTrack.Services.DTOs.TrainingGoal.Response;

namespace SwimTrack.Services.Contracts;

public interface ITrainingGoalService
{
    Task<ServiceResult> CreateTrainingGoalAsync(CreateTrainingGoalRequestModel request, string currentUserId);

    Task<EditTrainingGoalResponseModel?> GetTrainingGoalForEditAsync(int id, string currentUserId);

    Task<ServiceResult> EditTrainingGoalAsync(EditTrainingGoalRequestModel request, string currentUserId);

    Task<ServiceResult> RemoveTrainingGoalAsync(int id, string currentUserId);

    Task<TrainingGoalDetailsResponseModel?> GetTrainingGoalDetailsAsync(int id, string currentUserId);

    Task<IEnumerable<AllTrainingGoalsResponseModel>> GetAllTrainingGoalsAsync(string currentUserId);
}
