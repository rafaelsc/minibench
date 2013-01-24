using System;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace MiniBench.Tests
{
    [TestFixture]
    public class ResultSuiteDisplayTest
    {
        private static readonly ResultSuite suite = new ResultSuite("SuiteName",
                                                            new[] {
                                                                new BenchmarkResult("Result1Name", new TimeSpan(10000), 100),
                                                                new BenchmarkResult("Result2Name", new TimeSpan(5000), 200)
                                                            });
        private static readonly BenchmarkResult normalize = new BenchmarkResult("Normalizer", new TimeSpan(7000), 100);

        [Test]
        public void ResultSuiteDisplaysCorrectlyUnnormalizedAllColumns()
        {
            string expectedText = @"============ SuiteName ============
Result1Name 100 0:00.001 100.00
Result2Name 200 0:00.000  25.00

";


            string text = DisplayResultSuiteToString(suite, ResultColumns.All, null);

            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void ResultSuiteDisplaysCorrectlyNormalizedAllColumns()
        {
            string expectedText = @"============ SuiteName ============
Result1Name 100 0:00.001 1.43
Result2Name 200 0:00.000 0.36

";

            string text = DisplayResultSuiteToString(suite, ResultColumns.All, normalize);

            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void ResultSuiteDisplaysCorrectlyUnnormalized()
        {
            string expectedText = @"============ SuiteName ============
Result1Name 0:00.001 100.00
Result2Name 0:00.000  25.00

";

            string text = DisplayResultSuiteToString(suite, ResultColumns.Name | ResultColumns.Duration | ResultColumns.Score, null);

            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void ResultSuiteDisplaysCorrectlyNormalized()
        {
            string expectedText = @"============ SuiteName ============
Result1Name 0:00.001 1.43
Result2Name 0:00.000 0.36

";

            string text = DisplayResultSuiteToString(suite, ResultColumns.Name | ResultColumns.Duration | ResultColumns.Score, normalize);

            Assert.AreEqual(expectedText, text);
        }

        private static string DisplayResultSuiteToString(ResultSuite suite, ResultColumns columns, BenchmarkResult standardForScore)
        {
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            using (StreamReader reader = new StreamReader(stream))
            {
                suite.Display(writer, columns, standardForScore);
                writer.Flush();
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }
    }
}
