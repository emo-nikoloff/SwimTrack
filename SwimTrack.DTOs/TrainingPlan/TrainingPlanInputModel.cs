using System.ComponentModel.DataAnnotations;

using SwimTrack.DTOs.Workout;
using static SwimTrack.Shared.Constants.EntityValidation.TrainingPlan;

namespace SwimTrack.DTOs.TrainingPlan;

public class TrainingPlanInputModel
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [MaxLength(GoalMaxLength)]
    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IEnumerable<WorkoutInputModel> Workouts { get; set; } = new List<WorkoutInputModel>();
}
