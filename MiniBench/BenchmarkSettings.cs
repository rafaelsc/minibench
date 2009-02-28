using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBench
{
    /// <summary>
    /// The settings for running benchmarks. Currently consists of a calibration time and a test time.
    /// </summary>
    public class BenchmarkSettings
    {
        private static readonly TimeSpan DefaultCalibrationTime = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan DefaultTestTime = TimeSpan.FromSeconds(30);
        private const string CalibrationTimeFlag = "/calibration-time:";
        private const string TestTimeFlag = "/test-time:";

        /// <summary>
        /// The default settings.
        /// </summary>
        public static readonly BenchmarkSettings Default = new BenchmarkSettings(DefaultCalibrationTime, DefaultTestTime);

        private readonly TimeSpan calibrationTime;
        private readonly TimeSpan testTime;

        /// <summary>
        /// How long we need a test to run for before we're willing to guess how many iterations are
        /// required to achieve the desired test time.
        /// </summary>
        public TimeSpan CalibrationTime
        {
            get { return calibrationTime; }
        }

        /// <summary>
        /// How long we aim to run each test for.
        /// </summary>
        public TimeSpan TestTime
        {
            get { return testTime; }
        }

        /// <summary>
        /// Creates a new BenchmarkSettings object with the given settings.
        /// </summary>
        /// <param name="calibrationTime"></param>
        /// <param name="testTime"></param>
        public BenchmarkSettings(TimeSpan calibrationTime, TimeSpan testTime)
        {
            this.calibrationTime = calibrationTime;
            this.testTime = testTime;
        }

        /// <summary>
        /// Parses the given strings into a settings object.
        /// </summary>
        /// <remarks>Unrecognised
        /// flags are ignored, but recognised flags with invalid values cause
        /// an ArgumentException. Any unspecified arguments are filled in from
        /// the default settings. Recognised flags:
        /// <list type="bullet">
        /// <item>/calibration-time:XXX (seconds)</item>
        /// <item>/test-time:XXX (seconds)</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">args is null</exception>
        /// <exception cref="FormatException">Any of the elements of args is recognised as a flag,
        /// but has an invalid value.</exception>
        public static BenchmarkSettings Parse(params string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            TimeSpan calibrationTime = Default.CalibrationTime;
            TimeSpan testTime = Default.TestTime;
            foreach (string arg in args)
            {
                if (arg == null)
                {
                    continue;
                }
                if (arg.StartsWith(CalibrationTimeFlag))
                {
                    calibrationTime = TimeSpan.FromSeconds(double.Parse(arg.Substring(CalibrationTimeFlag.Length)));
                }
                else if (arg.StartsWith(TestTimeFlag))
                {
                    testTime = TimeSpan.FromSeconds(double.Parse(arg.Substring(TestTimeFlag.Length)));
                }
            }
            return new BenchmarkSettings(calibrationTime, testTime);
        }

        /// <summary>
        /// Helper method to parse the command line arguments of the current process.
        /// </summary>
        public static BenchmarkSettings ParseCommandLine()
        {
            return Parse(Environment.GetCommandLineArgs());
        }
    }
}
