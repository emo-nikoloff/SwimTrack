using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static SwimTrack.Shared.Constants.EntityValidation.WorkoutResult;

namespace SwimTrack.Data.Models;

public class WorkoutResult
{
    [Key]
    public int Id { get; set; }

    public int ActualDistance { get; set; }

    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }

    [ForeignKey(nameof(Workout))]
    public int WorkoutId { get; set; }

    public virtual Workout Workout { get; set; } = null!;
}
