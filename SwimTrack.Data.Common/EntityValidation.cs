namespace SwimTrack.Data.Common;

public static class EntityValidation
{
    public static class TrainingPlan
    {
        public const int TitleMaxLength = 100;

        // TODO: Decide how to implement the new TrainingGoal
        // public const int GoalMaxLength = 250;

        public const string DateColumnType = "date";
    }

    public static class TrainingGoal
    {
        public const int TitleMaxLength = 100;
    }

    public static class Workout
    {
        public const int DistanceMaxValue = 20000;

        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;

        public const string DateColumnType = "date";
    }

    public static class WorkoutResult
    {
        public const int DistanceMaxValue = 20000;

        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;

        public const string DateColumnType = "date";
    }

    public static class SwimmingPool
    {
        public const int NameMaxLength = 100;

        public const int LocationMaxLength = 200;
    }
}
