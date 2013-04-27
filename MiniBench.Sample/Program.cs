namespace MiniBench.Sample
{
    using System;
    using System.Linq;
    using System.Text;
    using MiniBench;

    static class Program
    {

        static void Main(string[] args)
        {
            BenchmarkStringJoinForSmallDataSet();
            BenchmarkStringJoinForBigDataSet();

            Console.ReadLine();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void BenchmarkStringJoinForSmallDataSet()
        {
            string[] testData = { "a", "b", "c", "d", "e" };
            string expectedData = "a b c d e";

            BenchmarkStringJoin(testData, expectedData);
        }
        private static void BenchmarkStringJoinForBigDataSet()
        {
            string[] testData = Enumerable.Range(0, 5000).Select(idx => idx.ToString()).ToArray() ;
            string expectedData = String.Join(" ", testData);

            BenchmarkStringJoin(testData, expectedData);
        }
  
        private static void BenchmarkStringJoin(string[] testData, string expectedData)
        {
            var testName = String.Format("Joining strings - with {0} string", testData.Length);

            // Create a TestSuite class for a group of BenchmarTests
            var benchmarkSuite = new TestSuite<string[], string>(testName)
                                    // You don't have to specify a method group - but you'll probably want to give an explicit name if you use
                                    .Plus(input => String.Join(" ", input), "String.Join")
                                    .Plus(LoopingWithStringBuilderCommumUsage)
                                    .Plus(input => LoopingWithStringBuilderWithInitialCapacity(input, expectedData.Length + 2), "LoopingWithStringBuilderWithInitialCapacity")
                                    .Plus(LoopingWithStringBuilderWithInitialValue)
                                    .Plus(input => LoopingWithStringBuilderWithInitialValueAndCapacity(input, expectedData.Length + 2), "LoopingWithStringBuilderWithInitialValueAndCapacity")
                                    .Plus(LoopingWithStringConcatenation)
                                    .Plus(LoopingWithStringConcat)
                                    .Plus(LoopingWithStringFormat);

            // This returns a ResultSuite
            var resultsSmallData = benchmarkSuite.RunTests(testData, expectedData)
                                        // Again, scaling returns a new ResultSuite, with all the results scaled 
                                        // - in this case they'll all have the same number of iterations
                                        .ScaleByBest(ScalingMode.VaryDuration);
     
            // There are pairs for name and score, iterations or duration - but we want name, duration *and* score
            resultsSmallData.Display(ResultColumns.NameAndDuration | ResultColumns.Score,
                                        // Scale the scores to make the best one get 1.0
                                        resultsSmallData.FindBest());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static string LoopingWithStringBuilderCommumUsage(string[] input)
        {
            StringBuilder builder = new StringBuilder();
            
            builder.Append(input[0]);
            for (int i = 1; i < input.Length; i++)
            {
                builder.Append(" ");
                builder.Append(input[i]);
            }
            return builder.ToString();
        }

        static string LoopingWithStringConcat(string[] input)
        {
            var ret = input[0];
            for (var i = 1; i < input.Length; i++)
            {
                // At least avoid *one* temporary string per iteration
                ret = String.Concat(ret, " ", input[i]);
            }
            return ret;
        }

        static string LoopingWithStringBuilderWithInitialValue(string[] input)
        {
            StringBuilder builder = new StringBuilder(input[0]);
            for (int i = 1; i < input.Length; i++)
            {
                builder.Append(" ");
                builder.Append(input[i]);
            }
            return builder.ToString();
        }

        static string LoopingWithStringBuilderWithInitialValueAndCapacity(string[] input, int capacity)
        {
            StringBuilder builder = new StringBuilder(input[0], capacity);
            for (int i = 1; i < input.Length; i++)
            {
                builder.Append(" ");
                builder.Append(input[i]);
            }
            return builder.ToString();
        }

        static string LoopingWithStringConcatenation(string[] input)
        {
            string ret = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                // At least avoid *one* temporary string per iteration
                ret = ret + " " + input[i];
            }
            return ret;
        }

        static string LoopingWithStringFormat(string[] input)
        {
            var ret = input[0];
            for (var i = 1; i < input.Length; i++)
            {
                // At least avoid *one* temporary string per iteration
                ret = String.Format("{0} {1}", ret, input[i]);
            }
            return ret;
        }


        static string LoopingWithStringBuilderWithInitialCapacity(string[] input, int capacity)
        {
            var builder = new StringBuilder(capacity);

            builder.Append(input[0]);
            for (var i = 1; i < input.Length; i++)
            {
                builder.Append(" ");
                builder.Append(input[i]);
            }
            return builder.ToString();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}