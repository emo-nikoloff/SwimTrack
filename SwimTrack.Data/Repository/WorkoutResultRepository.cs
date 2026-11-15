using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Common;
using SwimTrack.Data.Repository.Contracts;

namespace SwimTrack.Data.Repository;

public class WorkoutResultRepository : BaseRepository<WorkoutResult>, IWorkoutResultRepository
{
    public WorkoutResultRepository(SwimTrackDbContext dbContext) : base(dbContext)
    {
    }
}
