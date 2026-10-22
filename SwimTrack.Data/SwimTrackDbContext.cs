using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Models;

namespace SwimTrack.Data;

public class SwimTrackDbContext : IdentityDbContext<IdentityUser>
{
    public SwimTrackDbContext(DbContextOptions<SwimTrackDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TrainingPlan> TrainingPlans { get; set; }

    public virtual DbSet<Workout> Workouts { get; set; }

    public virtual DbSet<WorkoutResult> WorkoutResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TrainingPlan>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_TrainingPlan_ValidDateRange",
                    "[EndDate] >= [StartDate]");
            });

        modelBuilder.Entity<Workout>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Workout_PlannedDistance_Positive",
                    "[PlannedDistance] > 0");

                table.HasCheckConstraint(
                    "CK_Workout_PlannedDuration_Positive",
                    "[PlannedDuration] > 0");
            });

        modelBuilder.Entity<WorkoutResult>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_WorkoutResult_ActualDistance_Positive",
                    "[ActualDistance] > 0");

                table.HasCheckConstraint(
                    "CK_WorkoutResult_ActualDuration_Positive",
                    "[ActualDuration] > 0");
            });
    }
}
