using System.Linq;
using NUnit.Framework;

namespace MiniBench.Tests
{
    [TestFixture]
    public class TestSuiteTest
    {
        private static readonly TestSuite<int, int> EmptySuite = new TestSuite<int, int>("Tests");

        [Test]
        public void PlusReturnsNewSuite()
        {
            Assert.AreEqual(0, EmptySuite.Count());
            var newSuite = EmptySuite.Plus(x => x);
            Assert.AreEqual(0, EmptySuite.Count());
            Assert.AreEqual(1, newSuite.Count());
            var nextSuite = newSuite.Plus(x => x, "Identity");
            Assert.AreEqual(1, newSuite.Count());
            Assert.AreEqual(2, nextSuite.Count());
        }

        [Test]
        public void PlusRetainsName()
        {
            Assert.AreEqual(EmptySuite.Name, EmptySuite.Plus(x => x).Name);
        }

        [Test]
        public void PlusReturnValueAddsTest()
        {
            BenchmarkTest<int, int> test = new BenchmarkTest<int, int>(x => x, "foo");
            Assert.AreSame(test, EmptySuite.Plus(test).First());
        }
    }
}
