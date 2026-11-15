using SwimTrack.Web.ViewModels.Workout.ViewModels;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class AllTrainingPlansViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IEnumerable<WorkoutViewModel> Workouts { get; set; } = new List<WorkoutViewModel>();
}
