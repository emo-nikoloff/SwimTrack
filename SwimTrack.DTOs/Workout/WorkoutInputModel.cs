using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.EntityValidation.Workout;

namespace SwimTrack.DTOs.Workout;

public class WorkoutInputModel
{
    public int? Id { get; set; }

    public DateTime Date { get; set; }

    [Range(DistanceMinValue, DistanceMaxValue)]
    public int PlannedDistance { get; set; }

    [Range(DurationMinValue, DurationMaxValue)]
    public int PlannedDuration { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
}
