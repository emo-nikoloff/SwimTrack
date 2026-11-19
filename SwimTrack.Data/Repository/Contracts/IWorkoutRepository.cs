using SwimTrack.Data.Common;
using SwimTrack.Data.Models;

namespace SwimTrack.Data.Repository.Contracts;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout?> FindByIdAsync(int id);
}
