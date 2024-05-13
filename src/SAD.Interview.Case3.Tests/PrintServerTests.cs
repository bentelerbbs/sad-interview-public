using PrintServerImplementation.Contracts;
using PrintServerImplementation.Models;
using PrintServerImplementation.Tests.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace PrintServerImplementation.Tests
{
    [TestClass]
    public class PrintServerTests
    {
        private IPrintServer _assessedPrintServer = null;

        public PrintServerTests()
        {
        }

        [TestMethod]
        public void LoadBalanceTestStandard()
        {
            int printerCount = 3;
            int jobCount = 30;

            var fullTestTime = RunMultiplePrintsOnServer(_assessedPrintServer, printerCount, jobCount);
            // Ok
            Assert.IsTrue(fullTestTime < TimeSpan.FromSeconds(11), "Load balance minimum speed not achieved");
        }

        [TestMethod]
        public void LoadBalanceTestHarder()
        {
            int printerCount = 3;
            int jobCount = 90;

            var fullTestTime = RunMultiplePrintsOnServer(_assessedPrintServer, printerCount, jobCount);
            // Ok
            Assert.IsTrue(fullTestTime < TimeSpan.FromSeconds(11), "Load balance minimum speed not achieved");
        }

        [TestMethod]
        public void SynchronizedPrintingTest()
        {
            using var memStream = new MemoryStream();
            var testText = "This should come out from the printers as it came in";
            var printJobs = Regex.Split(testText, @"(?=\s)").Select(x => new PrintJobSpeedByLength(x));


            StreamPrinter[] printers = [
                new StreamPrinter(memStream),
                new StreamPrinter(memStream)
                ];

            foreach (var printer in printers)
            {
                printer.Start();
                _assessedPrintServer.AddPrinterToServer(printer);
            }

            var tasks = new List<Task>();

            _assessedPrintServer.Print(printJobs);

            var textResult = Encoding.UTF8.GetString(memStream.ToArray());

            LogDebug($"Input text: {testText}");
            LogDebug($"Output text: {textResult}");

            Assert.AreEqual(testText, textResult, "Text was not printed correctly");
        }

        [TestMethod]
        public void FailingPrinterTest()
        {
            using var memStream = new MemoryStream();
            var failingPrinter = new FailingStreamPrinter(memStream)
            {
                FailCount = 1,
                FailInterval = 2,
                FailOnStart = false
            };
            failingPrinter.Start();

            _assessedPrintServer.AddPrinterToServer(failingPrinter);

            _assessedPrintServer.Print(new PrintJobSpeedByLength("Should succeed"));

            try
            {
                _assessedPrintServer.Print(new PrintJobSpeedByLength("Should fail once"));
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("Printer exception was unhandled");
            }
        }

        [TestMethod]
        public void NoPrintersInServer()
        {
            try
            {
                _assessedPrintServer.Print(new PrintJobSpeedByLength("1"));

                Assert.Fail("Printing with no printer added should not silently continue, the job was not printed");
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is IndexOutOfRangeException)
            {
                Assert.Fail("Exceptions which indicate an internal issue were thrown");
            }
            catch (Exception ex)
            {
                LogDebug($"{ex.GetType().Name} exception was thrown, with message {ex.Message}");
            }
        }

        [TestMethod]
        public void PrintJobNothingTest()
        {
            using var memStream = new MemoryStream();
            var printer = new StreamPrinter(memStream);
            printer.Start();

            _assessedPrintServer.AddPrinterToServer(printer);

            try
            {
                _assessedPrintServer.Print(null as IPrintJob);
            }
            catch (NullReferenceException ex)
            {
                Assert.Fail($"null {nameof(IPrintJob)} value is unhandled ");
            }
            catch (ArgumentNullException anex)
            {
                LogDebug("null was handled with argument null exception, acceptable");
            }

            Assert.AreEqual(memStream.Length, 0, "Something was printed when IPrintJob was null");
            LogDebug("Null was accepted but nothing was printed");
        }

        private TimeSpan RunMultiplePrintsOnServer(IPrintServer printServer, int printerCount, int jobCount, IEnumerable<IPrintJob> jobs = null)
        {
            ConcurrentDictionary<StreamPrinter, Stream> printers = new ConcurrentDictionary<StreamPrinter, Stream>();
            ConcurrentDictionary<int, Tuple<IPrintJob, Task>> printJobTasks = new ConcurrentDictionary<int, Tuple<IPrintJob, Task>>();

            LogDebug("Adding printers");
            //Parallel.For(0, printerCount, (i) =>
            //{
            //    var stream = new MemoryStream();
            //    var printer = new StreamPrinter(stream);
            //    printers.TryAdd(printer, stream);
            //    _printServer.AddPrinterToServer(printer);
            //});

            for (int i = 0; i < printerCount; i++)
            {
                var stream = new MemoryStream();
                var printer = new StreamPrinter(stream);
                printers.TryAdd(printer, stream);
                printServer.AddPrinterToServer(printer);
            }

            // Start printers
            LogDebug("Test start");
            var start = DateTime.UtcNow;
            Parallel.ForEach(printers.Keys, (pr) => pr.Start());

            Parallel.For(0, jobCount, (i) =>
            {
                var job = jobs != null ? jobs.ElementAt(i) : new PrintJobSpeedByLength(RandomString(random.Next(10, 100)));

                printServer.Print(job);
            });

            var totalTime = DateTime.UtcNow - start;
            LogDebug($"Actual time: {totalTime}");

            return totalTime;
        }


        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void LogDebug(string text)
        {
            Debug.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}]{text}");
        }
    }
}