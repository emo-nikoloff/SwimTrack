using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SwimTrack.Data.Enums;
using SwimTrack.Data.Models;

namespace SwimTrack.Data.Configuration;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public readonly static List<Workout> Workouts = new()
    {
        new Workout
        {
            Id = 1,
            Date = new DateTime(2026, 1, 6),
            PlannedDistance = 2000,
            PlannedDuration = 50,
            Notes = "Фокус върху свободен стил и спокойно темпо.",
            Status = WorkoutStatus.Completed,
            TrainingPlanId = 1,
        },
        new Workout
        {
            Id = 2,
            Date = new DateTime(2026, 1, 8),
            PlannedDistance = 2200,
            PlannedDuration = 55,
            Notes = "Технически упражнения с дъска и плавници.",
            Status = WorkoutStatus.Completed,
            TrainingPlanId = 1,
        },
        new Workout
        {
            Id = 3,
            Date = new DateTime(2026, 1, 10),
            PlannedDistance = 1800,
            PlannedDuration = 45,
            Notes = "Възстановително плуване.",
            Status = WorkoutStatus.Skipped,
            TrainingPlanId = 1,
        },
        new Workout
        {
            Id = 4,
            Date = new DateTime(2026, 2, 3),
            PlannedDistance = 2500,
            PlannedDuration = 60,
            Notes = "Интервали 10 x 100 м със стабилно темпо.",
            Status = WorkoutStatus.Completed,
            TrainingPlanId = 2,
        },
        new Workout
        {
            Id = 5,
            Date = new DateTime(2026, 2, 5),
            PlannedDistance = 3000,
            PlannedDuration = 70,
            Notes = "Дълга тренировка с акцент върху издръжливостта.",
            Status = WorkoutStatus.Planned,
            TrainingPlanId = 2,
        },
    };

    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasData(Workouts);
    }
}
