namespace MiniBench
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A suite of tests to run. This type is immutable; the suite is built up by
    /// repeated calls to <see cref="Plus(BenchmarkTest{TInput,TOutput})" /> (in its
    /// various overloads).
    /// </summary>
    /// <example>
    /// The idiomatic way to build up a suite is with a chain of calls to Plus:
    /// <code>
    /// var suite = new TestSuite<string, int>("String tests")
    ///         .Plus(x => x.Length, "Length")
    ///         .Plus(AddVowels)
    ///         .Plus(new BenchmarkTest<string,int>(x => x.Length, "Length again"));
    /// </code>
    /// In fact, you may not need the test suite as a variable at all, if you just want
    /// to run it at the end - a call to <see cref="RunTests" /> with return
    /// a <see cref="ResultSuite" /> which may be all you want.
    /// </example>
    /// <typeparam name="TInput">Type of input to function to test</typeparam>
    /// <typeparam name="TOutput">Type of expected output</typeparam>
    public sealed class TestSuite<TInput, TOutput> : IEnumerable<BenchmarkTest<TInput, TOutput>>
    {
        private readonly string name;
        public string Name { get { return name; } }

        private readonly IList<BenchmarkTest<TInput, TOutput>> tests;

        public TestSuite(string name)
        {
            this.name = name;
            tests = new List<BenchmarkTest<TInput, TOutput>>();
        }

        private TestSuite(TestSuite<TInput, TOutput> parent, BenchmarkTest<TInput, TOutput> test)
        {
            name = parent.name;
            tests = new List<BenchmarkTest<TInput, TOutput>>(parent.tests.Concat(new[] { test }));
        }

        public TestSuite<TInput, TOutput> Plus(Func<TInput, TOutput> test)
        {
            return Plus(test, test.Method.Name);
        }

        public TestSuite<TInput, TOutput> Plus(Func<TInput, TOutput> test, string description)
        {
            return Plus(new BenchmarkTest<TInput, TOutput>(test, description));
        }

        public TestSuite<TInput, TOutput> Plus(BenchmarkTest<TInput, TOutput> test)
        {
            return new TestSuite<TInput, TOutput>(this, test);
        }

        public ResultSuite RunTests(TInput input, TOutput expectedOutput)
        {
            if (tests.Count == 0)
            {
                throw new InvalidOperationException("Cannot run a test suite until it contains at least one test.");
            }
            return new ResultSuite(name, tests.Select(test => test.Run(input, expectedOutput)));
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
