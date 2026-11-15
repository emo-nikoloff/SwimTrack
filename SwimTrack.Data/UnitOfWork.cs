using SwimTrack.Data.Contracts;

namespace SwimTrack.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly SwimTrackDbContext _dbContext;

    public UnitOfWork(SwimTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int SaveChanges()
        => _dbContext.SaveChanges();

    public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();
}
