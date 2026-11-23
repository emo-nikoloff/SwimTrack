using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

using static SwimTrack.Data.Common.EntityValidation.SwimmingPool;

namespace SwimTrack.Data.Models;

public class SwimmingPool
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    public int Length { get; set; }

    [MaxLength(LocationMaxLength)]
    public string? Location { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;

    public virtual IdentityUser User { get; set; } = null!;

    public virtual ICollection<TrainingPlan> TrainingPlans { get; set; } = new List<TrainingPlan>();

    public virtual ICollection<WorkoutResult> WorkoutResults { get; set; } = new List<WorkoutResult>();
}
