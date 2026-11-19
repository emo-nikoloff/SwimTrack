using SwimTrack.Shared.Enums;
using SwimTrack.Web.ViewModels.Workout.ViewModels;
using SwimTrack.Web.ViewModels.TrainingPlan.ViewModels.Enums;

namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class TrainingPlanDashboardViewModel
{
    private readonly IReadOnlyCollection<AllTrainingPlansViewModel> _trainingPlans;

    private readonly DateTime _today = DateTime.Today;

    private readonly (AllTrainingPlansViewModel TrainingPlan, WorkoutViewModel Workout)? _displaySession;

    public TrainingPlanDashboardViewModel(IReadOnlyCollection<AllTrainingPlansViewModel> trainingPlans)
    {
        _trainingPlans = trainingPlans;
        _displaySession = GetDisplaySession();
    }

    /* Hero card */
    public WorkoutViewModel? DisplayWorkout
        => _displaySession?.Workout;

    public AllTrainingPlansViewModel? DisplayWorkoutPlan
        => _displaySession?.TrainingPlan;

    public WorkoutState? State
        => DisplayWorkout == null
            ? null
            : DisplayWorkout.Date.Date == _today
                ? WorkoutState.Today
                : DisplayWorkout.Date.Date < _today
                    ? WorkoutState.Overdue
                    : DisplayWorkout.Date.Date == _today.AddDays(1)
                        ? WorkoutState.Tomorrow
                        : WorkoutState.Upcoming;

    public AllTrainingPlansViewModel? TrainingPlanWithoutWorkouts
        => _trainingPlans
            .FirstOrDefault(tp => tp.Workouts.Count == 0);

    /* Completion card */
    public int TotalCompletedWorkouts
        => _trainingPlans
            .Sum(tp =>
                tp.Workouts.Count(w =>
                    w.Status == WorkoutStatus.Completed));

    public int TotalSkippedWorkouts
        => _trainingPlans
            .Sum(tp =>
                tp.Workouts.Count(w =>
                    w.Status == WorkoutStatus.Skipped));

    public int DueWorkouts
        => _trainingPlans
            .Sum(tp =>
                tp.Workouts.Count(w =>
                    w.Date.Date <= _today));

    public int CompletionPercentage
        => DueWorkouts > 0
        ? (int)Math.Round((double)TotalCompletedWorkouts / DueWorkouts * 100)
        : 0;

    /* Distance card */
    public int TotalDistanceSwum
        => _trainingPlans
            .SelectMany(tp => tp.Workouts)
            .Where(w => w.Status == WorkoutStatus.Completed)
            .Sum(w => w.Result!.ActualDistance);

    private (AllTrainingPlansViewModel TrainingPlan, WorkoutViewModel Workout)? GetDisplaySession()
    {
        var plannedWorkouts = _trainingPlans
            .SelectMany(tp =>
                tp.Workouts.Select(w =>
                    (Plan: tp, Workout: w)))
            .Where(x => x.Workout.Status == WorkoutStatus.Planned);

        var todaySession = plannedWorkouts
            .FirstOrDefault(x => x.Workout.Date.Date == _today);

        if (todaySession.Workout != null)
        {
            return todaySession;
        }

        var overdueSession = plannedWorkouts
            .FirstOrDefault(x => x.Workout.Date.Date < _today);

        if (overdueSession.Workout != null)
        {
            return overdueSession;
        }

        var upcomingSession = plannedWorkouts
            .FirstOrDefault(x => x.Workout.Date.Date > _today);

        if (upcomingSession.Workout != null)
        {
            return upcomingSession;
        }

        return null;
    }
}
