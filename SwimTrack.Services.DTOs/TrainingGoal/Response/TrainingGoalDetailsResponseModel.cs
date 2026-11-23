namespace SwimTrack.Services.DTOs.TrainingGoal.Response;

public class TrainingGoalDetailsResponseModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int TargetDistance { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
