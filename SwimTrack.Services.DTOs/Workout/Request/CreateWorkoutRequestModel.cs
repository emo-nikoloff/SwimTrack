using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.EntityValidation.Workout;

namespace SwimTrack.Services.DTOs.Workout.Request;

public class CreateWorkoutRequestModel
{
    public DateTime Date { get; set; }

    [Range(DistanceMinValue, DistanceMaxValue)]
    public int PlannedDistance { get; set; }

    [Range(DurationMinValue, DurationMaxValue)]
    public int PlannedDuration { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
}
