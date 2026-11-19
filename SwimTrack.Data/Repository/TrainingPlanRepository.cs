using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Common;
using SwimTrack.Data.Repository.Contracts;

namespace SwimTrack.Data.Repository;

public class TrainingPlanRepository : BaseRepository<TrainingPlan>, ITrainingPlanRepository
{
    public TrainingPlanRepository(SwimTrackDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TrainingPlan?> FindByIdAsync(int id)
        => await DbSet.FindAsync(id);
}
