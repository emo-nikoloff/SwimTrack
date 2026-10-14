using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SwimTrack.Data.Models.Enums;
using static SwimTrack.Shared.Constants.EntityValidation.Workout;

namespace SwimTrack.Data.Models;

public class Workout
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime Date { get; set; }

    public int PlannedDistance { get; set; }

    public int PlannedDuration { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }

    public WorkoutStatus Status { get; set; } = WorkoutStatus.Planned;

    [ForeignKey(nameof(TrainingPlan))]
    public int TrainingPlanId { get; set; }

    public virtual TrainingPlan TrainingPlan { get; set; } = null!;

    public virtual WorkoutResult? WorkoutResult { get; set; }
}
