using System.ComponentModel.DataAnnotations;

namespace SwimTrack.Services.Common.Utilities;

public class BasicModelValidator
{
    public IReadOnlyCollection<ValidationResult> ValidationResults = new List<ValidationResult>();

    public bool IsValid(object model)
    {
        ValidationContext validationContext = new(model);
        List<ValidationResult> validationResults = new();

        ValidationResults = validationResults;

        return Validator.TryValidateObject(model, validationContext, validationResults, true);
    }

    public static bool IsValid(object model, List<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new(model);

        return Validator.TryValidateObject(model, validationContext, validationResults, true);
    }
}
