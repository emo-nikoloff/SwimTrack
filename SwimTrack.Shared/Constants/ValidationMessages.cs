namespace SwimTrack.Shared.Constants;

public static class ValidationMessages
{
    public static class Identity
    {
        public const string InvalidUserId =
            "The authenticated user's ID could not be retrieved.";
    }

    public static class DateValidation
    {
        public const string InvalidTrainingPlanPeriod =
            "The end date cannot be before the start date.";

        public const string WorkoutDateOutsidePlanPeriod =
            "The workout date must be within the training plan period.";

        public const string InvalidWorkoutCompletionDate =
            "The workout completion date cannot be before the workout date or in the future.";
    }

    public static class TrainingPlanValidation
    {
        public const string NotFoundTrainingPlan =
            "The requested training plan was not found.";

        public const string DuplicateWorkoutIds =
            "Duplicate workout IDs were submitted.";

        public const string InvalidTrainingPlanWorkout =
            "One or more submitted workout IDs do not belong to the requested training plan.";

        public const string ExistingWorkoutOutsidePlanPeriod =
            "The existing workout scheduled for {0} falls outside the new training plan period.";
    }

    public static class WorkoutValidation
    {
        public const string NotFoundWorkout =
            "The requested workout was not found.";

        public const string CannotCompleteWorkoutDueToStatus =
            "The workout cannot be completed because it is not in Planned status.";

        public const string CannotCompleteWorkoutInFuture =
            "The workout cannot be completed because its scheduled date is in the future.";

        public const string CannotSkipWorkoutDueToStatus =
            "The workout cannot be skipped because it is not in Planned status.";

        public const string CannotSkipWorkoutInFuture =
            "The workout cannot be skipped because its scheduled date is in the future.";

        public const string CannotPlanAgainWorkoutDueToStatus =
            "The workout cannot be planned again because it is not in Skipped status.";
    }
}
