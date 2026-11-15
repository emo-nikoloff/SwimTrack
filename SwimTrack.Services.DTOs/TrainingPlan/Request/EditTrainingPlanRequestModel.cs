using System.ComponentModel.DataAnnotations;

using SwimTrack.Services.DTOs.Workout.Request;
using static SwimTrack.Shared.Constants.EntityValidation.TrainingPlan;

namespace SwimTrack.Services.DTOs.TrainingPlan.Request;

public class EditTrainingPlanRequestModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [MaxLength(GoalMaxLength)]
    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<int> WorkoutIdsToKeep { get; set; } = new List<int>();

    public IReadOnlyCollection<CreateWorkoutRequestModel> NewWorkouts { get; set; } = new List<CreateWorkoutRequestModel>();
}
