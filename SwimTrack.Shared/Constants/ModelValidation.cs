namespace SwimTrack.Shared.Constants;

public static class ModelValidation
{
    public static class TrainingPlan
    {
        public const int NameMinLength = 3;
        public const int TitleMaxLength = 100;
    }

    public static class TrainingGoal
    {
        public const int TitleMinLength = 3;
        public const int TitleMaxLength = 100;

        public const int TargetDistanceMinValue = 25;
        public const int TargetDistanceMaxValue = 100_000;
    }

    public static class SwimmingPool
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 100;

        public const int LengthMinValue = 10;
        public const int LengthMaxValue = 100;

        public const int LocationMaxLength = 200;
    }

    public static class Workout
    {
        public const int DistanceMinValue = 25;
        public const int DistanceMaxValue = 20_000;

        public const int DurationMinValue = 1;
        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;
    }

    public static class WorkoutResult
    {
        public const int DistanceMinValue = 25;
        public const int DistanceMaxValue = 20_000;

        public const int DurationMinValue = 1;
        public const int DurationMaxValue = 600;

        public const int NotesMaxLength = 500;
    }
}
