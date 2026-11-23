using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.ModelValidation.TrainingPlan;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingPlan.InputModels;

public abstract class BaseTrainingPlanFormInputModel : IValidatableObject
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    // TODO: Decide how to implement the new TrainingGoal
    // [MaxLength(GoalMaxLength)]
    // public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool isTrainingPlanPeriodValid = DateValidator.ValidatePlanPeriod(StartDate, EndDate);
        if (!isTrainingPlanPeriodValid)
        {
            yield return new ValidationResult(
                InvalidTrainingPlanPeriod,
                new[]
                {
                    nameof(EndDate)
                });
        }
    }
}
