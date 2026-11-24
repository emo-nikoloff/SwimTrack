using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.ModelValidation.WorkoutResult;

namespace SwimTrack.Services.DTOs.WorkoutResult.Request;

public class CreateWorkoutResultRequestModel
{
    [Range(DistanceMinValue, DistanceMaxValue)]
    public int ActualDistance { get; set; }

    [Range(DurationMinValue, DurationMaxValue)]
    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }

    public int? SwimmingPoolId { get; set; }
}
