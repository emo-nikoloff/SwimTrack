using System.ComponentModel.DataAnnotations;

using SwimTrack.DTOs.Workout;
using SwimTrack.Shared.Validation;
using static SwimTrack.Shared.Constants.EntityValidation.TrainingPlan;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.DTOs.TrainingPlan;

public class TrainingPlanInputModel : IValidatableObject
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [MaxLength(GoalMaxLength)]
    public string? Goal { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IEnumerable<WorkoutInputModel> Workouts { get; set; } = new List<WorkoutInputModel>();

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

        List<WorkoutInputModel> workouts = Workouts.ToList();
        for (int i = 0; i < workouts.Count; i++)
        {
            if (!DateValidator.ValidateWorkoutDate(workouts[i].Date, StartDate, EndDate))
            {
                yield return new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                    {
                        $"{nameof(Workouts)}[{i}].{nameof(WorkoutInputModel.Date)}"
                    });
            }
        }
    }
}
