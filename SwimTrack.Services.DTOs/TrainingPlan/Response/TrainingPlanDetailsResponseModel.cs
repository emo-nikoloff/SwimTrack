using SwimTrack.Services.DTOs.Workout.Response;

namespace SwimTrack.Services.DTOs.TrainingPlan.Response;

public class TrainingPlanDetailsResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    // TODO: Decide how to implement the new TrainingGoal
    // public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<WorkoutResponseModel> Workouts { get; set; } = new List<WorkoutResponseModel>();
}
