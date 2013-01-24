using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MiniBench.Tests
{
    /// <summary>
    /// Verifies that the ResultColumns enum is defined correctly
    /// </summary>
    [TestFixture]
    public class ResultColumnsTest
    {
        /// <summary>
        /// Verifies that the flag parts are defined correctly.
        /// New columns should be added here.
        /// </summary>
        [Test]
        public void TestFlagPart()
        {
            TestFlags(ResultColumns.Name, ResultColumns.Duration, ResultColumns.Iterations, ResultColumns.Score);
        }

        /// <summary>
        /// Verifies that NameAndDuration is identical to Name | Duration
        /// </summary>
        [Test]
        public void TestNameAndDuration()
        {
            Assert.AreEqual(ResultColumns.NameAndDuration, ResultColumns.Name | ResultColumns.Duration);
        }

        /// <summary>
        /// Verifies that NameAndIterations is identical to Name | Iterations
        /// </summary>
        [Test]
        public void TestNameAndIterations()
        {
            Assert.AreEqual(ResultColumns.NameAndIterations, ResultColumns.Name | ResultColumns.Iterations);
        }

        /// <summary>
        /// Verifies that NameAndScore is identical to Name | Score
        /// </summary>
        [Test]
        public void TestNameAndScore()
        {
            Assert.AreEqual(ResultColumns.NameAndScore, ResultColumns.Name | ResultColumns.Score);
        }

        /// <summary>
        /// Verifies that the old field contains all known values.
        /// New columns should be added here.
        /// </summary>
        [Test]
        public void TestAll()
        {
            ResultColumns knownColumns = ResultColumns.Name | ResultColumns.Duration | ResultColumns.Iterations | ResultColumns.Score;
            Assert.AreEqual(ResultColumns.All & knownColumns, knownColumns);
        }

        #region Helper methods to test flag enums

        /// <summary>
        /// Verifies that the given enum values are all different powers of 2.
        /// </summary>
        /// <param name="values"></param>
        private void TestFlags(params Enum[] values)
        {
            HashSet<ulong> longValues = new HashSet<ulong>();
            foreach (Enum enumValue in values)
            {
                ulong numericValue = Convert.ToUInt64(enumValue);
                Assert.IsFalse(longValues.Contains(numericValue), "Same value defined twice in enum: " + numericValue);

                Assert.IsTrue(IsPowerOfTwo(numericValue));
                longValues.Add(numericValue);
            }
        }

        /// <summary>
        /// Returns true if the number is a multiple of 2, false otherwise.
        /// </summary>
        private static bool IsPowerOfTwo(ulong number)
        {
            // nice trick to find out if a number is a power of two
            // http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2/600306#600306
            return number != 0
                && (number & (number - 1)) == 0;
        }

        #endregion
    }
}
