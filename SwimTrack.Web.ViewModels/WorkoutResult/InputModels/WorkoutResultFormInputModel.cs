using System.ComponentModel.DataAnnotations;

using SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;
using static SwimTrack.Shared.Constants.ModelValidation.WorkoutResult;

namespace SwimTrack.Web.ViewModels.WorkoutResult.InputModels;

public class WorkoutResultFormInputModel
{
    [Range(DistanceMinValue, DistanceMaxValue)]
    public int ActualDistance { get; set; }

    [Range(DurationMinValue, DurationMaxValue)]
    public int ActualDuration { get; set; }

    public DateTime CompletedAt { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }

    public int? SwimmingPoolId { get; set; }

    public IEnumerable<AllSwimmingPoolsViewModel> AvailablePools { get; set; } = new List<AllSwimmingPoolsViewModel>();

    public int WorkoutId { get; set; }

    public int TrainingPlanId { get; set; }
}
