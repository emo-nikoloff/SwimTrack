using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ModelValidation.TrainingGoal;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingGoal.InputModels;

public abstract class BaseTrainingGoalFormInputModel : IValidatableObject
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = null!;

    [Range(TargetDistanceMinValue, TargetDistanceMaxValue)]
    public int TargetDistance { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool isTrainingGoalPeriodValid = DateValidator.ValidatePeriod(StartDate, EndDate);
        if (!isTrainingGoalPeriodValid)
        {
            yield return new ValidationResult(
                InvalidTrainingGoalPeriod,
                new[]
                {
                    nameof(EndDate)
                });
        }
    }
}
