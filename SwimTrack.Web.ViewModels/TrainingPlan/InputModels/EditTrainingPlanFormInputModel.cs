using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Validation;
using SwimTrack.Web.ViewModels.Workout.InputModels;
using SwimTrack.Web.ViewModels.Workout.ViewModels;
using static SwimTrack.Shared.Constants.ValidationMessages.DateValidation;

namespace SwimTrack.Web.ViewModels.TrainingPlan.InputModels;

public class EditTrainingPlanFormInputModel : BaseTrainingPlanFormInputModel
{
    public int Id { get; set; }

    public List<WorkoutViewModel> ExistingWorkouts { get; set; } = new();

    public List<int> KeptWorkoutIds { get; set; } = new();

    public List<WorkoutFormInputModel> NewWorkouts { get; set; } = new();

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (ValidationResult error in base.Validate(validationContext))
        {
            yield return error;
        }

        if (NewWorkouts == null)
        {
            yield break;
        }

        for (int i = 0; i < NewWorkouts.Count; i++)
        {
            bool isWorkoutDateInPlanPeriod = DateValidator.ValidateWorkoutDate(NewWorkouts[i].Date, StartDate, EndDate);
            if (!isWorkoutDateInPlanPeriod)
            {
                yield return new ValidationResult(
                    WorkoutDateOutsidePlanPeriod,
                    new[]
                    {
                        $"{nameof(NewWorkouts)}[{i}].{nameof(WorkoutFormInputModel.Date)}"
                    });
            }
        }
    }
}
