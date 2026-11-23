using SwimTrack.Shared.Enums;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class TrainingPlanDetailsViewModel
{
    /* Model specific properties */
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    // TODO: Decide how to implement the new TrainingGoal
    // public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<TrainingPlanWorkoutViewModel> Workouts { get; set; } = new List<TrainingPlanWorkoutViewModel>();

    /* View specific properties */
    public int TotalWorkouts
        => Workouts.Count;

    public int CompletedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Completed);

    public int SkippedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Skipped);

    public int PlannedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Planned);

    public int CompletedWorkoutsPercentage
        => TotalWorkouts > 0
            ? (int)Math.Round((double)CompletedWorkouts / TotalWorkouts * 100)
            : 0;

    public int TotalSwumDistance
        => Workouts
            .Where(w => w.Status == WorkoutStatus.Completed)
            .Sum(w => w.Result!.ActualDistance);

    public IReadOnlyCollection<TrainingPlanWeekViewModel> Weeks
        => Workouts
            .GroupBy(w => StartOfWeek(w.Date))
            .OrderBy(g => g.Key)
            .Select(g => new TrainingPlanWeekViewModel
            {
                WeekStart = g.Key,
                WeekEnd = g.Key.AddDays(6),
                WeekNumber = ((g.Key - StartOfWeek(StartDate)).Days / 7) + 1,
                Workouts = g.ToArray(),
            })
            .ToArray();

    private static DateTime StartOfWeek(DateTime date)
    {
        int daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;

        return date.Date.AddDays(-daysSinceMonday);
    }
}
