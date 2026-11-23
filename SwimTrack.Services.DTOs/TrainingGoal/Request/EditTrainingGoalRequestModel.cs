using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.ModelValidation.TrainingGoal;

namespace SwimTrack.Services.DTOs.TrainingGoal.Request;

public class EditTrainingGoalRequestModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = null!;

    [Range(TargetDistanceMinValue, TargetDistanceMaxValue)]
    public int TargetDistance { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
