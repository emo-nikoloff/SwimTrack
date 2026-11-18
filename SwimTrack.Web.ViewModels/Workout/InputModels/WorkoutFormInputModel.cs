using System.ComponentModel.DataAnnotations;

using SwimTrack.Shared.Enums;
using static SwimTrack.Shared.Constants.ModelValidation.Workout;

namespace SwimTrack.Web.ViewModels.Workout.InputModels;

public class WorkoutFormInputModel
{
    public int? Id { get; set; }

    public DateTime Date { get; set; }

    [Range(DistanceMinValue, DistanceMaxValue)]
    public int PlannedDistance { get; set; }

    [Range(DurationMinValue, DurationMaxValue)]
    public int PlannedDuration { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }

    public WorkoutStatus Status { get; set; } = WorkoutStatus.Planned;

    public int TrainingPlanId { get; set; }
}
