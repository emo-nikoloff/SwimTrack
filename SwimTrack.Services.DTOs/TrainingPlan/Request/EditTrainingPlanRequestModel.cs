using System.ComponentModel.DataAnnotations;

using SwimTrack.Services.DTOs.Workout.Request;
using static SwimTrack.Shared.Constants.ModelValidation.TrainingPlan;

namespace SwimTrack.Services.DTOs.TrainingPlan.Request;

public class EditTrainingPlanRequestModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(TitleMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    public int? TrainingGoalId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyCollection<int> KeptWorkoutIds { get; set; } = new List<int>();

    public IReadOnlyList<CreateWorkoutRequestModel> NewWorkouts { get; set; } = new List<CreateWorkoutRequestModel>();
}
