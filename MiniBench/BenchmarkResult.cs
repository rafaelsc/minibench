namespace MiniBench
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The result of running a benchmark test. This type is immutable and thread-safe.
    /// </summary>
    public sealed class BenchmarkResult : IEquatable<BenchmarkResult>
    {
        private readonly TimeSpan duration;
        /// <summary>
        /// The duration of the test. This will never be zero.
        /// </summary>
        public TimeSpan Duration { get { return duration; } }

        private readonly ulong iterations;
        /// <summary>
        /// The number of iterations executed. This will never be zero or negative.
        /// </summary>
        public ulong Iterations { get { return iterations; } }

        private readonly string name;
        /// <summary>
        /// The name of the test. This will never be null.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Constructs a new instance with the given properties.
        /// </summary>
        /// <param name="name">Name of the test. Must not be null.</param>
        /// <param name="duration">Duration of the test. Must be a positive period.</param>
        /// <param name="iterations">Number of iterations. Must not be zero.</param>
        public BenchmarkResult(string name, TimeSpan duration, ulong iterations)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("duration");
            }
            if (iterations == 0)
            {
                throw new ArgumentOutOfRangeException("iterations");
            }
            this.name = name;
            this.duration = duration;
            this.iterations = iterations;
        }

        public bool Equals(BenchmarkResult other)
        {
            return other != null && other.name == name 
                && other.iterations == iterations && other.duration == duration;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BenchmarkResult);
        }

        public override int GetHashCode()
        {
            int ret = 37;
            ret = ret * 23 + name.GetHashCode();
            ret = ret * 23 + duration.GetHashCode();
            ret = ret * 23 + iterations.GetHashCode();
            return ret;
        }

        /// <summary>
        /// Returns the "score" - the number of ticks per iteration, effectively.
        /// Scores are really only meaningful in comparison to each other.
        /// Smaller is better.
        /// </summary>
        public double Score
        {
            get { return (double) duration.Ticks / iterations; }
        }

        /// <summary>
        /// Returns the score of this result compared with the given "standard" which
        /// is deemed to have a scaled score of 1.0.
        /// </summary>
        /// <param name="standard">Standard result to compare with. May be null,
        /// in which case the raw score is returned.</param>
        public double GetScaledScore(BenchmarkResult standard)
        {
            if (standard == null)
            {
                return Score;
            }
            if (this == standard)
            {
                return 1.0; // Avoid any rounding errors
            }
            return Score / standard.Score;
        }

        /// <summary>
        /// Returns a new result based on this one, scaled to make it easy to compare with
        /// the given standard.
        /// </summary>
        /// <param name="standard">The standard to scale to</param>
        /// <param name="mode">How to scale the result</param>
        public BenchmarkResult ScaleToStandard(BenchmarkResult standard, ScalingMode mode)
        {
            if (this == standard)
            {
                return this;
            }
            switch (mode)
            {
                case ScalingMode.VaryDuration:
                    double iterationsFactor = (double)standard.iterations / iterations;
                    TimeSpan scaledDuration = TimeSpan.FromTicks((long)(duration.Ticks * iterationsFactor));
                    return new BenchmarkResult(name, scaledDuration, standard.iterations);
                case ScalingMode.VaryIterations:
                    double durationFactor = (double)standard.duration.Ticks / duration.Ticks;
                    ulong scaledIterations = (ulong)(iterations * durationFactor);
                    return new BenchmarkResult(name, standard.duration, scaledIterations);
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        /// <summary>
        /// Finds the "best" result out of the given sequence, based on score.
        /// </summary>
        public static BenchmarkResult FindBest(IEnumerable<BenchmarkResult> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            BenchmarkResult best = null;
            // Need my other LINQ project to find "Min by this projection..."
            foreach (BenchmarkResult result in results)
            {
                if (result == null)
                {
                    throw new ArgumentException("null result in sequence");
                }
                if (best == null || result.Score < best.Score)
                {
                    best = result;
                }
            }
            if (best == null)
            {
                throw new ArgumentException("Empty sequence of results provided; no \"best\" exists");
            }
            return best;
        }

        public static BenchmarkResult FindBest(params IEnumerable<BenchmarkResult>[] resultSuites)
        {
            return FindBest((IEnumerable<IEnumerable<BenchmarkResult>>)resultSuites);
        }

        /// <summary>
        /// Convenience method to find the best result from a sequence of sequences of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resultSuites"></param>
        /// <returns></returns>
        public static BenchmarkResult FindBest<T>(IEnumerable<T> resultSuites) 
            where T : IEnumerable<BenchmarkResult>
        {
            if (resultSuites == null)
            {
                throw new ArgumentNullException("resultSuites");
            }
            return FindBest(resultSuites.SelectMany(suite => 
            {
                if (suite == null)
                {
                    throw new ArgumentException("Null result suite");
                }
                return suite;
            }));
        }

        public override string ToString()
        {
            return string.Format("{0}: Duration={1}; Iterations={2}", name, duration, iterations);
        }
    }
}
