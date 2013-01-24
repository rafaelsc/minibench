namespace MiniBench
{
    using System;

    /// <summary>
    /// Exception thrown to indicate that a test has failed to return the expected result,
    /// or has thrown an exception.
    /// </summary>
    [Serializable]
    public class TestFailureException : Exception
    {
        public TestFailureException() { }
        public TestFailureException(string message) : base(message) { }
        public TestFailureException(string message, Exception inner) : base(message, inner) { }
        protected TestFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
