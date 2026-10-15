using SwimTrack.DTOs.TrainingPlan;

namespace SwimTrack.Services.Contracts;

public interface ITrainingPlanService
{
    Task<IEnumerable<TrainingPlanViewModel>> GetAllTrainingPlansAsync();

    Task CreateTrainingPlanAsync(TrainingPlanInputModel trainingPlanInputModel);

    Task<TrainingPlanInputModel?> GetTrainingPlanForEditAsync(int id);

    Task<bool> EditTrainingPlanAsync(int id, TrainingPlanInputModel trainingPlanInputModel);

    Task<bool> RemoveTrainingPlanAsync(int id);

    Task<TrainingPlanViewModel?> GetTrainingPlanDetailsAsync(int id);
}
