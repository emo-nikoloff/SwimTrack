namespace SwimTrack.Web.ViewModels.TrainingGoal.ViewModels;

public class TrainingGoalDetailsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int TargetDistance { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
