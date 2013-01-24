namespace MiniBench
{
    using System;

    [Flags]
    public enum ResultColumns
    {
        Name = 1,
        Iterations = 2,
        Duration = 4,
        Score = 8,
        NameAndIterations = Name | Iterations,
        NameAndDuration = Name | Duration,
        NameAndScore = Name | Score,
        All = Name | Iterations | Duration | Score
    }
}
