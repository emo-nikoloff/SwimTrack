using SwimTrack.DTOs.TrainingPlan;

namespace SwimTrack.Services.Contracts;

public interface ITrainingPlanService
{
    Task<IEnumerable<TrainingPlanViewModel>> GetAllTrainingPlansAsync(string currentUserId);

    Task CreateTrainingPlanAsync(TrainingPlanInputModel trainingPlanInputModel, string currentUserId);

    Task<TrainingPlanInputModel?> GetTrainingPlanForEditAsync(int id, string currentUserId);

    Task<bool> EditTrainingPlanAsync(int id, TrainingPlanInputModel trainingPlanInputModel, string currentUserId);

    Task<bool> RemoveTrainingPlanAsync(int id, string currentUserId);

    Task<TrainingPlanViewModel?> GetTrainingPlanDetailsAsync(int id, string currentUserId);
}
