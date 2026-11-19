namespace SwimTrack.Services.Common.Results;

public class ServiceResult
{
    private ServiceResult(ServiceResultStatus status, IReadOnlyCollection<ServiceError> errors)
    {
        Status = status;
        Errors = errors;
    }

    public ServiceResultStatus Status { get; }

    public IReadOnlyCollection<ServiceError> Errors { get; }

    public bool Succeeded
        => Status == ServiceResultStatus.Success;

    public static ServiceResult Success()
    {
        return new ServiceResult(
            ServiceResultStatus.Success,
            Array.Empty<ServiceError>());
    }

    public static ServiceResult Failure(ServiceResultStatus status, IEnumerable<ServiceError> errors)
    {
        if (status == ServiceResultStatus.Success)
        {
            throw new ArgumentException(
                "A successful result cannot contain failure status.",
                nameof(status));
        }

        return new ServiceResult(
            status,
            errors.ToArray());
    }

    public static ServiceResult Failure(ServiceResultStatus status, string errorMessage, string? propertyName = null)
    {
        return Failure(
            status,
            new[] { new ServiceError(propertyName, errorMessage) });
    }
}
