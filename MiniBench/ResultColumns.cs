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
        NameAndIterations = 1 | 2,
        NameAndDuration = 1 | 4,
        NameAndScore = 1 | 8,
        All = 1 | 2 | 4 | 8
    }
}
