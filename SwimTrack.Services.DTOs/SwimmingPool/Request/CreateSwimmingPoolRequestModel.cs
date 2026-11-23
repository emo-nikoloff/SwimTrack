using System.ComponentModel.DataAnnotations;

using static SwimTrack.Shared.Constants.ModelValidation.SwimmingPool;

namespace SwimTrack.Services.DTOs.SwimmingPool.Request;

public class CreateSwimmingPoolRequestModel
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [Range(LengthMinValue, LengthMaxValue)]
    public int Length { get; set; }

    [MaxLength(LocationMaxLength)]
    public string? Location { get; set; }
}
