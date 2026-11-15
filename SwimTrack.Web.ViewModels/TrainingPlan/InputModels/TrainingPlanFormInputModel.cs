using System.ComponentModel.DataAnnotations;

using SwimTrack.Web.ViewModels.Workout.InputModels;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.EntityValidation.TrainingPlan;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingPlan.InputModels;

public class TrainingPlanFormInputModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [MaxLength(GoalMaxLength)]
    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<WorkoutFormInputModel> Workouts { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!DateValidator.ValidatePlanPeriod(StartDate, EndDate))
        {
            yield return new ValidationResult(
                InvalidTrainingPlanPeriod,
                new[]
                {
                    nameof(EndDate)
                });
            yield break;
        }

        for (int i = 0; i < Workouts.Count; i++)
        {
            if (!DateValidator.ValidateWorkoutDate(Workouts[i].Date, StartDate, EndDate))
            {
                yield return new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                    {
                        $"{nameof(Workouts)}[{i}].{nameof(WorkoutFormInputModel.Date)}"
                    });
            }
        }
    }
}
