namespace SwimTrack.DTOs.WorkoutResult;

public class WorkoutResultViewModel
{
    public int ActualDistance { get; set; }

    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    public string? Notes { get; set; }
}
