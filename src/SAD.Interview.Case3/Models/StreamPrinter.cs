using SAD.Interview.Case3.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Interview.Case3.Models
{
    public class StreamPrinter : IPrinter, IStartStopCapable, IDisposable
    {
        private readonly object _threadStartStopLock = new object();
        private readonly EventWaitHandle _printEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly EventWaitHandle _printProcessEndWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private Thread? _printingThread;
        private CancellationTokenSource? _cancellationTokenSource;

        ConcurrentQueue<IPrintJob> _printJobs = new ConcurrentQueue<IPrintJob>();
        private bool disposedValue;

        private event EventHandler<IPrintJob>? _jobPrinted;
        private event EventHandler<Tuple<IPrintJob, Exception>>? _jobFailed;

        protected Stream OutputStream { get => _outputStream; }
        private Stream _outputStream;

        public bool IsStarted { get => _printingThread != null && _cancellationTokenSource?.IsCancellationRequested != true; }

        public StreamPrinter(Stream stream)
        {
            _outputStream = stream;
        }

        public IEnumerable<IPrintJob> GetPrintJobsInQueue() => _printJobs.ToList();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "The code is actually reachable when the job failed delegate gets called")]
        public void Print(IPrintJob printJob)
        {
            var jobPrintedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            Exception? printException = null;

            EventHandler<IPrintJob> jobPrintedDelegate = (s, e) =>
            {
                if (e == printJob)
                    jobPrintedWaitHandle.Set();
            };

            EventHandler<Tuple<IPrintJob, Exception>> jobFailedDelegate = (s, e) =>
            {
                if (e.Item1 == printJob)
                {
                    printException = e.Item2;
                    jobPrintedWaitHandle.Set();
                }
            };

            _jobPrinted += jobPrintedDelegate;
            _jobFailed += jobFailedDelegate;
            AddJobToQueue(printJob);

            // wait for printjob to be printed
            jobPrintedWaitHandle.WaitOne();

            _jobPrinted -= jobPrintedDelegate;
            _jobFailed -= jobFailedDelegate;


            if (printException != null)
                throw printException;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "The code is actually reachable when the job failed delegate gets called")]
        public async Task PrintAsync(IPrintJob printJob)
        {
            var jobPrintedSemaphore = new SemaphoreSlim(0, 1);
            Exception? printException = null;

            EventHandler<IPrintJob> jobPrintedDelegate = (s, e) =>
            {
                if (e == printJob)
                    jobPrintedSemaphore.Release();
            };

            EventHandler<Tuple<IPrintJob, Exception>> jobFailedDelegate = (s, e) =>
            {
                if (e.Item1 == printJob)
                {
                    printException = e.Item2;
                    jobPrintedSemaphore.Release();
                }
            };

            _jobPrinted += jobPrintedDelegate;
            _jobFailed += jobFailedDelegate;
            AddJobToQueue(printJob);

            // wait for printjob to be printed
            await jobPrintedSemaphore.WaitAsync();
            
            _jobPrinted -= jobPrintedDelegate;
            _jobFailed -= jobFailedDelegate;

            if (printException != null)
                throw printException;
        }

        public void Start()
        {
            // add synchronization, lock might be enough
            lock (_threadStartStopLock)
            {
                if (_printingThread == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _printingThread = new Thread(PrintingProcess);
                    _printingThread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (_threadStartStopLock)
            {
                // add synchronization, lock might be enough
                if (_printingThread != null)
                {
                    _printProcessEndWaitHandle.Reset();
                    _cancellationTokenSource?.Cancel();

                    _printEventWaitHandle.Set();
                    _printProcessEndWaitHandle.WaitOne();

                    _printingThread = null;
                    _cancellationTokenSource = null;
                }
            }
        }

        private void AddJobToQueue(IPrintJob printJob)
        {
            _printJobs.Enqueue(printJob);

            // Release worker thread to print jobs
            _printEventWaitHandle.Set();
        }

        /// <summary>
        /// This could take some time, as the print job could be large, or perform some modifications/transformations.
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        protected virtual byte[] PreparePrintJobForStream(IPrintJob printJob) => Encoding.UTF8.GetBytes(printJob.PreparePrintOutput());

        protected virtual void WriteJobDataToStream(IPrintJob printJob, byte[] printJobData)
        {
            _outputStream.Write(printJobData);
        }

        private void PrintingProcess()
        {
            CancellationToken cancellationToken = _cancellationTokenSource?.Token ?? throw new InvalidOperationException($"{nameof(_cancellationTokenSource)} must be set before starting the thread");

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (_printJobs.TryDequeue(out var printJob))
                {
                    try
                    {
                        WriteJobDataToStream(printJob, PreparePrintJobForStream(printJob));
                        _jobPrinted?.Invoke(this, printJob);
                    }
                    catch (Exception ex)
                    {
                        _jobFailed?.Invoke(this, Tuple.Create(printJob, ex));
                    }
                }
                else
                {
                    _printEventWaitHandle.WaitOne();
                }
            }

            _printProcessEndWaitHandle.Set();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
