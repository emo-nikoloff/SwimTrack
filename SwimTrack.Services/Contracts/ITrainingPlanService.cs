using SwimTrack.Services.Common.Results;
using SwimTrack.Services.DTOs.TrainingPlan.Request;
using SwimTrack.Services.DTOs.TrainingPlan.Response;

namespace SwimTrack.Services.Contracts;

public interface ITrainingPlanService
{
    Task<IEnumerable<AllTrainingPlansResponseModel>> GetAllTrainingPlansAsync(string currentUserId);

    Task<ServiceResult> CreateTrainingPlanAsync(CreateTrainingPlanRequestModel request, string currentUserId);

    Task<EditTrainingPlanResponseModel?> GetTrainingPlanForEditAsync(int id, string currentUserId);

    Task<ServiceResult> EditTrainingPlanAsync(EditTrainingPlanRequestModel request, string currentUserId);

    Task<ServiceResult> RemoveTrainingPlanAsync(int id, string currentUserId);

    Task<TrainingPlanDetailsResponseModel?> GetTrainingPlanDetailsAsync(int id, string currentUserId);
}
