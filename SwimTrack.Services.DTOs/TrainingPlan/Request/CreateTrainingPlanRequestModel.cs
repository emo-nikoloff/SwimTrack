using System.ComponentModel.DataAnnotations;

using SwimTrack.Services.DTOs.Workout.Request;
using static SwimTrack.Shared.Constants.ModelValidation.TrainingPlan;

namespace SwimTrack.Services.DTOs.TrainingPlan.Request;

public class CreateTrainingPlanRequestModel
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    // TODO: Decide how to implement the new TrainingGoal
    // [MaxLength(GoalMaxLength)]
    // public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyList<CreateWorkoutRequestModel> Workouts { get; set; } = new List<CreateWorkoutRequestModel>();
}
