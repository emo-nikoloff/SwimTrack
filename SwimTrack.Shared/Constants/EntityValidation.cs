namespace SwimTrack.Shared.Constants;

public static class EntityValidation
{
    public static class TrainingPlan
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 100;

        public const int GoalMaxLength = 250;
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
