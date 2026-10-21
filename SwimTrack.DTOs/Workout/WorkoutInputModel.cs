using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.EntityValidation.Workout;

namespace SwimTrack.DTOs.Workout;

public class WorkoutInputModel
{
    public int? Id { get; set; }

    public DateTime Date { get; set; }

    [Range(1, int.MaxValue)]
    public int PlannedDistance { get; set; }

    [Range(1, int.MaxValue)]
    public int PlannedDuration { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
}
