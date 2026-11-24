using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SwimTrack.Data.Models;

namespace SwimTrack.Data;

public class SwimTrackDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
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

        modelBuilder.Entity<TrainingGoal>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_TrainingGoal_TargetDistance_Positive",
                    "[TargetDistance] > 0");

                table.HasCheckConstraint(
                    "CK_TrainingGoal_ValidDateRange",
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

        modelBuilder.Entity<SwimmingPool>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_SwimmingPool_Length_Positive",
                    "[Length] > 0");
            });
    }
}
