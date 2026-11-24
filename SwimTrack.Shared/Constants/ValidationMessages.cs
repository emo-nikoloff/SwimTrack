namespace SwimTrack.Shared.Constants;

public static class ValidationMessages
{
    public static class Identity
    {
        public const string InvalidUserId =
            "The authenticated user's ID could not be retrieved.";

        public const string InvalidIdentityRoleOperation =
            "Role {0} cannot be created!";
    }

    public static class DateValidation
    {
        public const string InvalidTrainingPlanPeriod =
            "The end date cannot be before the start date.";

        public const string InvalidTrainingGoalPeriod =
            "The end date cannot be before the start date.";

        public const string WorkoutDateOutsidePlanPeriod =
            "The workout date must be within the training plan period.";

        public const string PlanPeriodOutsideGoalPeriod =
            "The training plan period must be within the training goal period.";

        public const string InvalidWorkoutCompletionDate =
            "The workout completion date cannot be before the workout date or in the future.";
    }

    public static class TrainingPlanValidation
    {
        public const string NotFoundTrainingPlan =
            "The requested training plan was not found.";

        public const string InvalidTrainingPlanWorkout =
            "One or more submitted workout IDs do not belong to the requested training plan.";

        public const string ExistingWorkoutOutsidePlanPeriod =
            "The existing workout scheduled for {0} falls outside the new training plan period.";
    }

    public static class TrainingGoalValidation
    {
        public const string NotFoundTrainingGoal =
            "The requested training goal was not found.";

        public const string ExistingPlanOutsideGoalPeriod =
            "The existing training plan “{0}” falls outside the new training goal period.";
    }

    public static class SwimmingPoolValidation
    {
        public const string NotFoundSwimmingPool =
            "The requested swimming pool was not found.";
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
