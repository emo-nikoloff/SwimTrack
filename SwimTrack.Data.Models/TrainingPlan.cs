using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

using static SwimTrack.Shared.Constants.EntityValidation.TrainingPlan;

namespace SwimTrack.Data.Models;

public class TrainingPlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [MaxLength(GoalMaxLength)]
    public string? Goal { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime EndDate { get; set; }

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;

    public virtual IdentityUser User { get; set; } = null!;
}
