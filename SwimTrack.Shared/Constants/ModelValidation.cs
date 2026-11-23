namespace SwimTrack.Shared.Constants;

public static class ModelValidation
{
    public static class TrainingPlan
    {
        public const int NameMinLength = 3;
        public const int TitleMaxLength = 100;

        // TODO: Decide how to implement the new TrainingGoal
        // public const int GoalMaxLength = 250;
    }

    public static class Workout
    {
        public const int DistanceMinValue = 1;
        public const int DistanceMaxValue = 20000;

        public const int DurationMinValue = 1;
        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;
    }

    public static class WorkoutResult
    {
        public const int DistanceMinValue = 1;
        public const int DistanceMaxValue = 20000;

        public const int DurationMinValue = 1;
        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;
    }
}
