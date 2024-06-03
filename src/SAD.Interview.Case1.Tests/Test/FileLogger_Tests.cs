using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedLibraryTests;
using System.Collections.Concurrent;
using System.Text;

namespace FileWriteMultithreading
{
    [TestClass]
    public class FileLogger_Tests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            string dir = $"{AppDomain.CurrentDomain.BaseDirectory}/_Tests/FileLogger_Parallel_Writing_Test";
            Test.PrepareTestDirectory(dir);
        }

        /// <summary>
        /// Tests writing 10K lines with FileLogger
        /// </summary>
        [TestMethod]
        public void FileLogger_Serial_Writing_Test() // 1.6s
        {
            string dir = $"{AppDomain.CurrentDomain.BaseDirectory}/_Tests/FileLogger_Serial_Writing_Test";
            Test.PrepareTestDirectory(dir);
            string filePath = Path.Combine(dir, "shared_log.log");
            //
            ILogger fileLogger = new FileLogger(filePath);
            //
            StringBuilder expectedTotalText = new StringBuilder();
            int linesToWrite = 10 * 1000;
            List<string> linesExpected = new List<string>(linesToWrite);
            //
            for (int i = 0; i < linesToWrite; i++)
            {
                string line = $"LINE {i}";
                fileLogger.Trace(line);
                expectedTotalText.AppendLine(line);
                linesExpected.Add(line);
            }
            string[] linesWritten = File.ReadAllLines(filePath);
            List<string> linesFromTotalString = fileLogger.AllTextWrittenByThisInstance().Split(Environment.NewLine).ToList();
            linesFromTotalString.RemoveAt(linesFromTotalString.Count - 1); // NewLine is at the end so we get 1 last empty string
            linesFromTotalString = linesFromTotalString.Select(i => i.Split(" [TRACE] ").Last()).ToList();
            linesWritten = linesWritten.Select(i => i.Split(" [TRACE] ").Last()).ToArray();
            string[] linesMissing = linesExpected.Except(linesWritten).ToArray();
            string[] linesMissing_TotalString = linesExpected.Except(linesFromTotalString).ToArray();
            //
            Test.Compare(linesToWrite, linesWritten.Length, $"FileLogger_Serial_Writing_Test, lines should be {linesToWrite}");
            Test.Compare(linesToWrite, linesFromTotalString.Count, $"FileLogger_Serial_Writing_Test, lines from TotalString should be {linesToWrite}");
            Test.Compare(Array.Empty<string>(), linesMissing, $"FileLogger_Serial_Writing_Test, lines missing should be 0");
            Test.Compare(Array.Empty<string>(), linesMissing_TotalString, $"FileLogger_Serial_Writing_Test, lines missing from TotalString should be 0");
        }

        /// <summary>
        /// Tests writing n lines with FileLogger
        /// </summary>
        /// <param name="linesToWrite"></param>
        /// <param name="maxDegreeOfParallelism"></param>
        /// <param name="name"></param>
        [DataTestMethod]
        [DataRow(10 * 1000, 1, "10K_Lines_1_Thread")]       // 5s
        [DataRow(10 * 1000, 2, "10K_Lines_2_Threads")]      // 3.1s
        [DataRow(100 * 1000, 12, "100K_Lines_12_Threads")]  // 3.2s
        [DataRow(1000 * 1000, 24, "1M_Lines_24_Threads")]   // 4.4s
        public void FileLogger_Parallel_Writing_Test(int linesToWrite, int maxDegreeOfParallelism, string name)
        {
            string dir = $"{AppDomain.CurrentDomain.BaseDirectory}/_Tests/FileLogger_Parallel_Writing_Test";
            // Test.PrepareTestDirectory(dir); // DONE IN CLASS INIT SO THAT WE CAN SEE THE RESULT FILES AFTER ALL RUNS
            string filePath = Path.Combine(dir, $"shared_log_{name}.log");
            //
            ILogger fileLogger = new FileLogger(filePath);
            //
            ConcurrentQueue<string> linesExpected = new ConcurrentQueue<string>();
            Parallel.For(0, linesToWrite, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                string line = $"LINE {i}";
                fileLogger.Trace(line);
                linesExpected.Enqueue(line);
            });
            bool wait = true;
            if (wait)
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(300);
                }
            }
            string[] linesWritten = File.ReadAllLines(filePath);
            linesWritten = linesWritten.Select(i => i.Split(" [TRACE] ").Last()).ToArray();
            string[] linesMissing = linesExpected.Except(linesWritten).ToArray();
            Console.WriteLine($"INFO, lines missing: {linesMissing.Length}, linesExpected: {linesExpected.Count}, linesWritten: {linesWritten.Length}");
            Test.Compare(Array.Empty<string>(), linesMissing, $"FileLogger_Parallel_Writing_Test, lines missing should be 0");
            Test.Compare(linesToWrite, linesWritten.Length, $"FileLogger_Parallel_Writing_Test, lines should be {linesToWrite}");
        }
    }
}