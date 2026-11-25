namespace SwimTrack.Web.Areas.Admin.ViewModels;

public class UserManagementViewModel
{
    public UserManagementViewModel(
        IReadOnlyCollection<UserViewModel> users,
        IReadOnlyCollection<string> applicationRoles)
    {
        Users = users;
        ApplicationRoles = applicationRoles;
    }

    public IReadOnlyCollection<UserViewModel> Users { get; }

    public IReadOnlyCollection<string> ApplicationRoles { get; }

    public bool HasUsers
        => Users.Count > 0;
}
