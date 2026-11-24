namespace SwimTrack.Shared.Validation;

public static class DateValidator
{
    public static bool ValidatePeriod(DateTime startDate, DateTime endDate)
        => startDate.Date <= endDate.Date;

    public static bool ValidateWorkoutDate(DateTime workoutDate, DateTime startDate, DateTime endDate)
        => workoutDate.Date >= startDate.Date
            && workoutDate.Date <= endDate.Date;

    public static bool ValidateWorkoutCompletionDate(DateTime completedAt, DateTime workoutDate)
        => completedAt.Date >= workoutDate.Date
            && completedAt.Date <= DateTime.Today;

    public static bool ValidatePlanWithinGoalPeriod(DateTime planStartDate, DateTime planEndDate, DateTime goalStartDate, DateTime goalEndDate)
        => planStartDate.Date >= goalStartDate.Date
            && planEndDate.Date <= goalEndDate.Date;
}
