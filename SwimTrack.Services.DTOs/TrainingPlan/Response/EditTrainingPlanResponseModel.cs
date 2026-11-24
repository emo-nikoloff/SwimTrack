using SwimTrack.Services.DTOs.Workout.Response;

namespace SwimTrack.Services.DTOs.TrainingPlan.Response;

public class EditTrainingPlanResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? TrainingGoalId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<EditWorkoutResponseModel> Workouts { get; set; } = new List<EditWorkoutResponseModel>();

    public int? SwimmingPoolId { get; set; }
}
