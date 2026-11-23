using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

using static SwimTrack.Data.Common.EntityValidation.TrainingPlan;

namespace SwimTrack.Data.Models;

public class TrainingPlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [ForeignKey(nameof(TrainingGoal))]
    public int? TrainingGoalId { get; set; }

    public virtual TrainingGoal? TrainingGoal { get; set; }

    [Column(TypeName = DateColumnType)]
    public DateTime StartDate { get; set; }

    [Column(TypeName = DateColumnType)]
    public DateTime EndDate { get; set; }

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    [ForeignKey(nameof(SwimmingPool))]
    public int? SwimmingPoolId { get; set; }

    public virtual SwimmingPool? SwimmingPool { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;

    public virtual IdentityUser User { get; set; } = null!;
}
