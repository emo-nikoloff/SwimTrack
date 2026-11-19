namespace SwimTrack.Web.ViewModels.TrainingPlan.ViewModels;

public class AllTrainingPlansPageViewModel
{
    public AllTrainingPlansPageViewModel(IReadOnlyCollection<AllTrainingPlansViewModel> trainingPlans)
    {
        TrainingPlans = trainingPlans;
        Dashboard = new(TrainingPlans);
    }

    public IReadOnlyCollection<AllTrainingPlansViewModel> TrainingPlans { get; }

    public TrainingPlanDashboardViewModel Dashboard { get; }

    public bool HasUserPlans
        => TrainingPlans.Count > 0;
}
