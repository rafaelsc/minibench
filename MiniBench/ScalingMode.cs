namespace MiniBench
{
    /// <summary>
    /// Options of how to scale results.
    /// </summary>
    public enum ScalingMode
    {
        /// <summary>
        /// The results all use the same number of iterations,
        /// but they vary by duration.
        /// </summary>
        VaryDuration,
        /// <summary>
        /// The results all have the same duration, but they
        /// vary by the number of iterations completed within that time.
        /// </summary>
        VaryIterations
    }
}
