namespace SwimTrack.Services.Common.Results;

public class ServiceError
{
    public ServiceError(string? propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string? PropertyName { get; }

    public string ErrorMessage { get; }
}
