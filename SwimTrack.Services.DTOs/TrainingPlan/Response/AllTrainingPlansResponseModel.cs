using SwimTrack.Services.DTOs.Workout.Response;

namespace SwimTrack.Services.DTOs.TrainingPlan.Response;

public class AllTrainingPlansResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<WorkoutResponseModel> Workouts { get; set; } = new List<WorkoutResponseModel>();
}
