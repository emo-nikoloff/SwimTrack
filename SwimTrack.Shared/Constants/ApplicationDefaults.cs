namespace SwimTrack.Shared.Constants;

public static class ApplicationDefaults
{
    public const string DateTimeFormat = "dd.MM.yyyy";

    public static class Identity
    {
        public const int PasswordMaximumLength = 100;

        public static class Role
        {
            public const string User = "User";
            public const string Manager = "Manager";
            public const string Admin = "Admin";
        }
    }
}
