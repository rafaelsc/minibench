namespace MiniBench
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public sealed class BenchmarkTest<TInput, TOutput>
    {
        /// <summary>
        /// How long we need to run for to get a reasonable idea of how long
        /// the test takes to execute, to guess the number of iterations required
        /// for TargetTestTime.
        /// </summary>
        static readonly TimeSpan MinSampleTime = TimeSpan.FromSeconds(2);

        /// <summary>
        /// How long we want to run the tests for when really timing it and running
        /// as fast as possible.
        /// </summary>
        static readonly TimeSpan TargetTestTime = TimeSpan.FromSeconds(30);


        private readonly Func<TInput, TOutput> test;
        private readonly string name;

        public BenchmarkTest(Func<TInput, TOutput> test, string description)
        {
            this.test = test;
            this.name = description;
        }

        /// <summary>
        /// Runs the test. Throws a TestFailureException if the output isn't as expected.
        /// This is only checked once, before the real benchmarking gets underway.
        /// </summary>
        /// <returns></returns>
        public BenchmarkResult Run(TInput input, TOutput expectedOutput)
        {
            TOutput actualOutput = test(input);
            if (!EqualityComparer<TOutput>.Default.Equals(expectedOutput, actualOutput))
            {
                throw new TestFailureException(name);
            }
            ulong iterations = 1;
            TimeSpan elapsed = RunAndTime(input, iterations);
            while (elapsed < MinSampleTime)
            {
                iterations *= 2;
                elapsed = RunAndTime(input, iterations);
            }
            // Upscale the sample to the target time. Do this in floating point arithmetic
            // to avoid overflow issues.
            iterations = (ulong)((TargetTestTime.Ticks / (double)elapsed.Ticks) * iterations);
            elapsed = RunAndTime(input, iterations);
            return new BenchmarkResult(name, elapsed, iterations);
        }

        private TimeSpan RunAndTime(TInput input, ulong iterations)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (ulong i = 0; i < iterations; i++)
            {
                test(input);
            }
            sw.Stop();
            return sw.Elapsed;
        }
    }
}