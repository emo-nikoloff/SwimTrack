namespace SwimTrack.Services.DTOs.WorkoutResult.Response;

public class WorkoutResultResponseModel
{
    public int ActualDistance { get; set; }

    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    public string? Notes { get; set; }
}
