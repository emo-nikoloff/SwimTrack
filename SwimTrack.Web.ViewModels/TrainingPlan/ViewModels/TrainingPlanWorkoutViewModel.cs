using SwimTrack.Shared.Enums;
using SwimTrack.Web.ViewModels.Workout.ViewModels;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class TrainingPlanWorkoutViewModel : WorkoutViewModel
{
    private const double ReferenceDistance = 2000.0;

    private const int MinimumPercentage = 15;

    private const int MaximumPercentage = 100;

    public bool IsToday
        => Date.Date == DateTime.Today;

    public int PlannedDistancePercentage
        => CalculateDistancePercentage(PlannedDistance);

    public int ActualDistancePercentage
        => CalculateDistancePercentage(Result?.ActualDistance ?? PlannedDistance);

    public string DisplayNotes
    {
        get => Status switch
        {
            WorkoutStatus.Completed => !string.IsNullOrWhiteSpace(Result?.Notes)
                ? Result.Notes
                : !string.IsNullOrWhiteSpace(Notes)
                    ? Notes
                    : "No notes.",

            WorkoutStatus.Planned => !string.IsNullOrWhiteSpace(Notes)
                ? Notes
                : "Steady pace.",

            WorkoutStatus.Skipped => !string.IsNullOrWhiteSpace(Notes)
                ? Notes
                : "No notes.",

            _ => !string.IsNullOrWhiteSpace(Notes)
                ? Notes
                : "No notes."
        };

    }

    private static int CalculateDistancePercentage(int distance)
    {
        return Math.Clamp((int)Math.Round(distance / ReferenceDistance * 100), MinimumPercentage, MaximumPercentage);
    }
}
