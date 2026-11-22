using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Validation;
using SwimTrack.Web.ViewModels.Workout.InputModels;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingPlan.InputModels;

public class CreateTrainingPlanFormInputModel : BaseTrainingPlanFormInputModel
{
    public List<WorkoutFormInputModel> Workouts { get; set; } = new();

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (ValidationResult error in base.Validate(validationContext))
        {
            yield return error;
        }

        if (Workouts == null)
        {
            yield break;
        }

        for (int i = 0; i < Workouts.Count; i++)
        {
            bool isWorkoutDateInPlanPeriod = DateValidator.ValidateWorkoutDate(Workouts[i].Date, StartDate, EndDate);
            if (!isWorkoutDateInPlanPeriod)
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
