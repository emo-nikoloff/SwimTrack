using SwimTrack.Web.ViewModels.Workout.ViewModels;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class TrainingPlanWeekViewModel
{
    public int WeekNumber { get; set; }

    public DateTime WeekStart { get; set; }

    public DateTime WeekEnd { get; set; }

    public IReadOnlyCollection<TrainingPlanWorkoutViewModel> Workouts { get; set; } = new List<TrainingPlanWorkoutViewModel>();
}
