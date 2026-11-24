using SwimTrack.Services.DTOs.SwimmingPool.Response;

namespace SwimTrack.Services.DTOs.WorkoutResult.Response;

public class WorkoutResultResponseModel
{
    public int ActualDistance { get; set; }

    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    public string? Notes { get; set; }

    public int? SwimmingPoolId { get; set; }

    public SwimmingPoolResponseModel? SwimmingPool { get; set; }
}
