using System;
using System.Linq;
using NUnit.Framework;

namespace MiniBench.Tests
{
    [TestFixture]
    public class BenchmarkSettingsTest
    {
        [Test]
        public void ParseAllSettings()
        {
            BenchmarkSettings settings = BenchmarkSettings.Parse(
                "/calibration-time:10", "/test-time:50");
        }

        [Test]
        public void ParseIgnoresUnrecognisedArguments()
        {
            BenchmarkSettings settings = BenchmarkSettings.Parse(
                "foo", "bar", "/calibration-time:10");
            Assert.AreEqual(settings.CalibrationTime, TimeSpan.FromSeconds(10));
            Assert.AreEqual(settings.TestTime, BenchmarkSettings.Default.TestTime);
        }

        
    }
}
