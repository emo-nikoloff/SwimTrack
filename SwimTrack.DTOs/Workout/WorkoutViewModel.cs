using SwimTrack.Shared.Enums;

namespace SwimTrack.DTOs.Workout;

public class WorkoutViewModel
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int PlannedDistance { get; set; }

    public int PlannedDuration { get; set; }

    public string? Notes { get; set; }

    public WorkoutStatus Status { get; set; }
}
