using System.ComponentModel.DataAnnotations;

namespace SwimTrack.Web.Areas.Admin.ViewModels;

public class UserRoleInputModel
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;
}
