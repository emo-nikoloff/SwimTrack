using System.ComponentModel.DataAnnotations;


using static SwimTrack.Shared.Constants.EntityValidation.WorkoutResult;

namespace SwimTrack.DTOs.WorkoutResult;

public class WorkoutResultInputModel
{
    [Range(1, int.MaxValue)]
    public int ActualDistance { get; set; }

    [Range(1, int.MaxValue)]
    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
}
