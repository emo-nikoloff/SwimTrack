using SwimTrack.DTOs.Workout;

namespace SwimTrack.DTOs.TrainingPlan;

public class TrainingPlanViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IEnumerable<WorkoutViewModel> Workouts { get; set; } = new List<WorkoutViewModel>();
}
