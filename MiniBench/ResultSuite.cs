using System.Globalization;

namespace MiniBench
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    public sealed class ResultSuite : IEnumerable<BenchmarkResult>
    {
        private static readonly Dictionary<ResultColumns, Func<BenchmarkResult, BenchmarkResult, string>> Formatters =
            new Dictionary<ResultColumns, Func<BenchmarkResult, BenchmarkResult, string>>
        {
            { ResultColumns.Name, (result, ignored) => result.Name },
            { ResultColumns.Iterations, (result, ignored) => result.Iterations.ToString(CultureInfo.InvariantCulture) },
            { ResultColumns.Duration, (result, ignored) => string.Format("{0}:{1:00}.{2:000}", 
                (int) result.Duration.TotalMinutes, result.Duration.Seconds, result.Duration.Milliseconds) },
            { ResultColumns.Score, (result, standard) => result.GetScaledScore(standard).ToString("F2") }
        };
        private static readonly ResultColumns[] IndividualColumns = { ResultColumns.Name, ResultColumns.Iterations, ResultColumns.Duration, ResultColumns.Score };

        private readonly List<BenchmarkResult> results;
        private readonly string name;
        public string Name { get { return name; } }

        public ResultSuite(string name, IEnumerable<BenchmarkResult> results)
        {
            this.name = name;
            this.results = new List<BenchmarkResult>(results);
            if (this.results.Count == 0)
            {
                throw new ArgumentException("Empty sequence of results provided.");
            }
        }

        public ResultSuite Scale(BenchmarkResult standard, ScalingMode mode)
        {
            return new ResultSuite(name, results.Select(x => x.ScaleToStandard(standard, mode)));
        }

        /// <summary>
        /// Scales the contents of this result suite in the given mode, using the best result
        /// (in terms of score) as the "standard".
        /// </summary>
        public ResultSuite ScaleByBest(ScalingMode mode)
        {
            return Scale(FindBest(), mode);
        }

        public BenchmarkResult this[int index]
        {
            get { return results[index]; }
        }

        public BenchmarkResult FindBest()
        {
            return BenchmarkResult.FindBest(this);
        }

        public IEnumerator<BenchmarkResult> GetEnumerator()
        {
            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Convenience method to display the results when no score scaling is required and output should be Console.Out.
        /// </summary>
        /// <param name="columns"></param>
        public void Display(ResultColumns columns)
        {
            Display(columns, null);
        }

        /// <summary>
        /// Displays the results, optionally scaling scores to the given standard, printing to Console.Out.
        /// </summary>
        /// <param name="columns">Columns to display</param>
        /// <param name="standardForScore">Result to count as a score of 1.0. May be null, in which 
        /// case the raw scores (ticks per iteration) are displayed.</param>
        public void Display(ResultColumns columns, BenchmarkResult standardForScore)
        {
            Display(Console.Out, columns, standardForScore);
        }

        /// <summary>
        /// Displays the results, optionally scaling scores to the given standard, printing to the given TextWriter.
        /// </summary>
        /// <param name="output">TextWriter to output to</param>
        /// <param name="columns">Columns to display</param>
        /// <param name="standardForScore">Result to count as a score of 1.0. May be null, in which 
        /// case the raw scores (ticks per iteration) are displayed.</param>
        public void Display(TextWriter output, ResultColumns columns, BenchmarkResult standardForScore)
        {
            output.WriteLine("============ {0} ============", name);

            List<List<string>> formattedColumns = new List<List<string>>();

            // This isn't as easy to do in a query expression as one might expect.
            foreach (ResultColumns candidateColumn in IndividualColumns)
            {
                if ((columns & candidateColumn) != 0)
                {
                    Func<BenchmarkResult, BenchmarkResult, string> formatter = Formatters[candidateColumn];
                    formattedColumns.Add(this.Select(result => formatter(result, standardForScore)).ToList());
                }
            }
            List<int> maxLengths = formattedColumns.Select(col => col.Max(entry => entry.Length)).ToList();

            for (int row = 0; row < results.Count; row++)
            {
                for (int col = 0; col < formattedColumns.Count; col++)
                {
                    string unpadded = formattedColumns[col][row];
                    if (col == 0)
                    {
                        output.Write(unpadded.PadRight(maxLengths[col]));
                    }
                    else
                    {
                        output.Write(" ");
                        output.Write(unpadded.PadLeft(maxLengths[col]));
                    }
                }
                output.WriteLine();
            }
            output.WriteLine();
        }
    }
}
