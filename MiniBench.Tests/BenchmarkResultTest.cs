using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MiniBench.Tests
{
    [TestFixture]
    public class BenchmarkResultTest
    {
        private static readonly BenchmarkResult FastResult = new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(30), 10000);
        // SlowResult takes twice as long to do half as many iterations
        private static readonly BenchmarkResult SlowResult = new BenchmarkResult("Slow Test", TimeSpan.FromSeconds(60), 5000);

        [Test]
        public void EqualityChecksAllProperties()
        {
            Assert.AreEqual(FastResult, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(30), 10000));
            Assert.AreNotEqual(FastResult, new BenchmarkResult("Different", TimeSpan.FromSeconds(30), 10000));
            Assert.AreNotEqual(FastResult, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(60), 10000));
            Assert.AreNotEqual(FastResult, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(30), 5000));
        }

        [Test]
        public void GetHashCodeChecksAllProperties()
        {
            int fastHash = FastResult.GetHashCode();
            Assert.AreEqual(fastHash, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(30), 10000).GetHashCode());
            Assert.AreNotEqual(fastHash, new BenchmarkResult("Different", TimeSpan.FromSeconds(30), 10000).GetHashCode());
            Assert.AreNotEqual(fastHash, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(60), 10000).GetHashCode());
            Assert.AreNotEqual(fastHash, new BenchmarkResult("Fast Test", TimeSpan.FromSeconds(30), 5000).GetHashCode());
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ZeroDurationIsProhibited()
        {
            new BenchmarkResult("Test", TimeSpan.Zero, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeDurationIsProhibited()
        {
            new BenchmarkResult("Test", TimeSpan.FromSeconds(-1), 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ZeroIterationsIsProhibited()
        {
            new BenchmarkResult("Test", TimeSpan.FromSeconds(1), 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullNameIsProhibited()
        {
            new BenchmarkResult(null, TimeSpan.FromSeconds(1), 1);
        }

        [Test]
        public void ScaleVaryingIterations()
        {
            BenchmarkResult scaledSlow = SlowResult.ScaleToStandard(FastResult, ScalingMode.VaryIterations);
            Assert.AreEqual(new BenchmarkResult("Slow Test", TimeSpan.FromSeconds(30), 2500), scaledSlow);
        }

        [Test]
        public void ScaleVaryingDuration()
        {
            BenchmarkResult scaledSlow = SlowResult.ScaleToStandard(FastResult, ScalingMode.VaryDuration);
            Assert.AreEqual(new BenchmarkResult("Slow Test", TimeSpan.FromSeconds(120), 10000), scaledSlow);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBestNullElementRejected()
        {
            BenchmarkResult.FindBest(new BenchmarkResult[] {null});
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindBestNullSequenceRejected()
        {
            BenchmarkResult.FindBest((IEnumerable<BenchmarkResult>)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBestEmptySequenceRejected()
        {
            BenchmarkResult.FindBest();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindBestFlatteningNullSequenceRejected()
        {
            BenchmarkResult.FindBest((IEnumerable<IEnumerable<BenchmarkResult>>) null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBestFlatteningNullElementRejected()
        {
            BenchmarkResult.FindBest(new IEnumerable<BenchmarkResult>[] { null });
        }

        [Test]
        public void FindBestFlattening()
        {
            BenchmarkResult best = BenchmarkResult.FindBest<BenchmarkResult[]>(new[]
            {
                new[] { SlowResult },
                new[] { FastResult },
                new[] { SlowResult }
            });
            Assert.AreEqual(FastResult, best);
        }

        [Test]
        public void FindBestNonFlattening()
        {
            BenchmarkResult best = BenchmarkResult.FindBest(new[] { SlowResult, FastResult, SlowResult });
            Assert.AreEqual(FastResult, best);
        }
    }
}
