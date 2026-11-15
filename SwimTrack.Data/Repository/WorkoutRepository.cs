using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Common;
using SwimTrack.Data.Repository.Contracts;

namespace SwimTrack.Data.Repository;

public class WorkoutRepository : BaseRepository<Workout>, IWorkoutRepository
{
    public WorkoutRepository(SwimTrackDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Workout?> FindByIdAsync(int id)
        => await DbSet.FindAsync(id);
}
