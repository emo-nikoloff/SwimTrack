using SwimTrack.Shared.Enums;
using SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;
using SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;
using SwimTrack.Web.ViewModels.TrainingPlan.ViewModels.Enums;
using SwimTrack.Web.ViewModels.Workout.ViewModels;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class AllTrainingPlansViewModel
{
    /* Model specific properties */
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TrainingGoalViewModel? TrainingGoal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<WorkoutViewModel> Workouts { get; set; } = new List<WorkoutViewModel>();

    public SwimmingPoolViewModel? SwimmingPool { get; set; }

    /* View specific properties */
    public int CompletedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Completed);

    public int SkippedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Skipped);

    public int PlannedWorkouts
        => Workouts.Count(w => w.Status == WorkoutStatus.Planned);

    public TrainingPlanStatus Status
        => AllWorkoutsResolved
            ? TrainingPlanStatus.Finished
            : IsPlanExpired
                ? TrainingPlanStatus.Expired
                : IsPlanUpcoming
                    ? TrainingPlanStatus.Upcoming
                    : TrainingPlanStatus.Active;

    public string StatusText
        => AllWorkoutsResolved
            ? TrainingPlanStatus.Finished.ToString()
            : IsPlanExpired
                ? TrainingPlanStatus.Expired.ToString()
                : IsPlanUpcoming
                    ? TrainingPlanStatus.Upcoming.ToString()
                    : TrainingPlanStatus.Active.ToString();

    private bool AllWorkoutsResolved
        => Workouts.Count > 0 && PlannedWorkouts == 0;

    private bool IsPlanExpired
        => EndDate.Date < DateTime.Today;

    private bool IsPlanUpcoming
        => StartDate.Date > DateTime.Today;
}
