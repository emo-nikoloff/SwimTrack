using SwimTrack.Data.Models;
using SwimTrack.Data.Repository.Common;
using SwimTrack.Data.Repository.Contracts;

namespace SwimTrack.Data.Repository;

public class SwimmingPoolRepository : BaseRepository<SwimmingPool>, ISwimmingPoolRepository
{
    public SwimmingPoolRepository(SwimTrackDbContext dbContext)
        : base(dbContext)
    {
    }
}
