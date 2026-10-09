using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SwimTrack.Data.Models;

namespace SwimTrack.Data.Configuration;

public class TrainingPlanConfiguration : IEntityTypeConfiguration<TrainingPlan>
{
    public readonly static List<TrainingPlan> TrainingPlans = new()
    {
        new TrainingPlan
        {
            Id = 1,
            Name = "Основна издръжливост",
            Goal = "Изграждане на аеробна издръжливост и подобряване на техниката",
            StartDate = new DateTime(2026, 1, 5),
            EndDate = new DateTime(2026, 2, 1),
        },
        new TrainingPlan
        {
            Id = 2,
            Name = "Подготовка за 1500 м",
            Goal = "Подготовка за плуване на 1500 метра под 30 минути",
            StartDate = new DateTime(2026, 2, 2),
            EndDate = new DateTime(2026, 3, 1),
        },
    };

    public void Configure(EntityTypeBuilder<TrainingPlan> builder)
    {
        builder.HasData(TrainingPlans);
    }
}
