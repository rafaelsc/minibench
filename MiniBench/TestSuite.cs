namespace MiniBench
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class TestSuite
    {
        public static TestSuite<TInput, TOutput> Create<TInput, TOutput>(string name, TInput input, TOutput expectedOutput)
        {
            return new TestSuite<TInput, TOutput>(name, input, expectedOutput);
        }
    }

    /// <summary>
    /// A suite of tests to run.
    /// </summary>
    /// <typeparam name="TInput">Type of input to function to test</typeparam>
    /// <typeparam name="TOutput">Type of expected output</typeparam>
    public sealed class TestSuite<TInput, TOutput> : IEnumerable<BenchmarkTest<TInput, TOutput>>
    {
        private readonly TInput input;
        private readonly TOutput expectedOutput;
        private readonly string name;

        private readonly IList<BenchmarkTest<TInput, TOutput>> tests;

        public TestSuite(string name, TInput input, TOutput expectedOutput)
        {
            this.name = name;
            this.input = input;
            this.expectedOutput = expectedOutput;
            tests = new List<BenchmarkTest<TInput, TOutput>>();
        }

        private TestSuite(TestSuite<TInput, TOutput> parent, BenchmarkTest<TInput, TOutput> test)
        {
            name = parent.name;
            input = parent.input;
            expectedOutput = parent.expectedOutput;
            tests = new List<BenchmarkTest<TInput, TOutput>>(parent.tests.Concat(new[] { test }));
        }

        public TestSuite<TInput, TOutput> Add(Func<TInput, TOutput> test)
        {
            return Add(test, test.Method.Name);
        }

        public TestSuite<TInput, TOutput> Add(Func<TInput, TOutput> test, string description)
        {
            return new TestSuite<TInput, TOutput>(this, new BenchmarkTest<TInput, TOutput>(input, expectedOutput, test, description));
        }

        public ResultSuite RunTests()
        {
            if (tests.Count == 0)
            {
                throw new InvalidOperationException("Cannot run a test suite until it contains at least one test.");
            }
            return new ResultSuite(name, tests.Select(test => test.Run()));
        }

        public IEnumerator<BenchmarkTest<TInput, TOutput>> GetEnumerator()
        {
            return tests.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
