namespace SwimTrack.Shared.Constants;

public static class ValidationMessages
{
    public static class DateValidation
    {
        public const string InvalidTrainingPlanPeriod = "The start date cannot be after the end date.";
        public const string WorkoutDateOutsidePlanPeriod = "The workout date must be within the training plan period.";
        public const string WorkoutCompletionDateBeforeWorkoutDate = "The workout completion date cannot be before the workout date.";
    }
}
