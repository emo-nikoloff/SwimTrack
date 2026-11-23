namespace SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;

public class AllTrainingGoalsPageViewModel
{
    public AllTrainingGoalsPageViewModel(IReadOnlyCollection<AllTrainingGoalsViewModel> trainingGoals)
    {
        TrainingGoals = trainingGoals;
    }

    public IReadOnlyCollection<AllTrainingGoalsViewModel> TrainingGoals { get; }

    public bool HasUserGoals
        => TrainingGoals.Count > 0;
}
