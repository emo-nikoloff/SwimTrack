using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SwimTrack.Data.Models;

namespace SwimTrack.Data.Configuration;

public class WorkoutResultConfiguration : IEntityTypeConfiguration<WorkoutResult>
{
    public readonly static List<WorkoutResult> WorkoutResults = new()
    {
        new WorkoutResult
        {
            Id = 1,
            ActualDistance = 2000,
            ActualDuration = 48,
            CompletedAt = new DateTime(2026, 1, 6, 18, 30, 0),
            Notes = "Тренировката беше изпълнена с равномерно темпо.",
            WorkoutId = 1,
        },
        new WorkoutResult
        {
            Id = 2,
            ActualDistance = 2200,
            ActualDuration = 57,
            CompletedAt = new DateTime(2026, 1, 8, 18, 45, 0),
            Notes = "Последните 400 метра бяха с по-висока интензивност.",
            WorkoutId = 2,
        },
        new WorkoutResult
        {
            Id = 3,
            ActualDistance = 2500,
            ActualDuration = 59,
            CompletedAt = new DateTime(2026, 2, 3, 19, 0, 0),
            Notes = "Интервалите бяха изпълнени по план.",
            WorkoutId = 4,
        },
    };

    public void Configure(EntityTypeBuilder<WorkoutResult> builder)
    {
        builder.HasData(WorkoutResults);
    }
}
