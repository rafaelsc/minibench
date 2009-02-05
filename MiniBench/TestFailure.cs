namespace MiniBench
{
    using System;

    /// <summary>
    /// Exception thrown to indicate that a test has failed to return the expected result,
    /// or has thrown an exception.
    /// </summary>
    public sealed class TestFailureException : Exception
    {
        public TestFailureException(string message) : base(message)
        {            
        }
    }
}
