using SwimTrack.Data.Common;
using SwimTrack.Data.Models;

namespace SwimTrack.Data.Repository.Contracts;

public interface ITrainingPlanRepository : IRepository<TrainingPlan>
{
    Task<TrainingPlan?> FindByIdAsync(int id);
}
