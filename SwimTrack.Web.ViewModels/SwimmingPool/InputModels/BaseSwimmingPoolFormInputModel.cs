using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.ModelValidation.SwimmingPool;

namespace SwimTrack.Web.ViewModels.SwimmingPool.InputModels;

public abstract class BaseSwimmingPoolFormInputModel
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [Range(LengthMinValue, LengthMaxValue)]
    public int Length { get; set; }

    [MaxLength(LocationMaxLength)]
    public string? Location { get; set; }
}
