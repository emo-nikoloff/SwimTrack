using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Common;
using SwimTrack.Data.Repository.Contracts;

namespace SwimTrack.Data.Repository;

public class TrainingGoalRepository : BaseRepository<TrainingGoal>, ITrainingGoalRepository
{
    public TrainingGoalRepository(SwimTrackDbContext dbContext)
        : base(dbContext)
    {
    }
}
