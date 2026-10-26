using SwimTrack.Shared.Enums;
using SwimTrack.DTOs.WorkoutResult;

namespace SwimTrack.DTOs.Workout;

public class WorkoutViewModel
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int PlannedDistance { get; set; }

    public int PlannedDuration { get; set; }

    public string? Notes { get; set; }

    public WorkoutStatus Status { get; set; }

    public WorkoutResultViewModel? Result { get; set; }
}
