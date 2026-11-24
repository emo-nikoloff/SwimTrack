using SwimTrack.Services.DTOs.TrainingGoal.Response;
using SwimTrack.Services.DTOs.Workout.Response;

namespace SwimTrack.Services.DTOs.TrainingPlan.Response;

public class TrainingPlanDetailsResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TrainingGoalResponseModel? TrainingGoal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<WorkoutResponseModel> Workouts { get; set; } = new List<WorkoutResponseModel>();
}
