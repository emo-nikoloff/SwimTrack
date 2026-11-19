using SwimTrack.Services.DTOs.WorkoutResult.Response;
using SwimTrack.Shared.Enums;

namespace SwimTrack.Services.DTOs.Workout.Response;

public class WorkoutResponseModel
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int PlannedDistance { get; set; }

    public int PlannedDuration { get; set; }

    public string? Notes { get; set; }

    public WorkoutStatus Status { get; set; }

    public WorkoutResultResponseModel? Result { get; set; }
}
