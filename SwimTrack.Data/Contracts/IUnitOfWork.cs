namespace SwimTrack.Data.Contracts;

public interface IUnitOfWork
{
    int SaveChanges();

    Task<int> SaveChangesAsync();
}
