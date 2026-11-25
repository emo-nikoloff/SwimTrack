namespace SwimTrack.Web.Areas.Admin.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public ICollection<string> Roles { get; set; } = new List<string>();
}
