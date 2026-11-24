using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Validation;
using SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;
using SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;
using static SwimTrack.Shared.Constants.ModelValidation.TrainingPlan;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingPlan.InputModels;

public abstract class BaseTrainingPlanFormInputModel : IValidatableObject
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    public int? TrainingGoalId { get; set; }

    public IEnumerable<AllTrainingGoalsViewModel> AvailableGoals { get; set; } = new List<AllTrainingGoalsViewModel>();

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? SwimmingPoolId { get; set; }

    public IEnumerable<AllSwimmingPoolsViewModel> AvailablePools { get; set; } = new List<AllSwimmingPoolsViewModel>();

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool isTrainingPlanPeriodValid = DateValidator.ValidatePeriod(StartDate, EndDate);
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
