using SAD.Interview.Case3.Contracts;
using SAD.Interview.Case3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Interview.Case3.Tests.Models
{
    public class FailingStreamPrinter : StreamPrinter
    {
        public int FailInterval { get; set; } = 3;
        public int FailCount { get; set; } = 1;

        public bool FailOnStart { get; init; }

        private int _runCount = 0;
        private int _currentFailCount = 0;

        public FailingStreamPrinter(Stream stream) : base(stream)
        {
        }

        protected override void WriteJobDataToStream(IPrintJob printJob, byte[] printJobData)
        {
            if ((_runCount + (FailOnStart ? 0 : 1)) % FailInterval == 0)
            {
                if (_currentFailCount == FailCount)
                {
                    Interlocked.Add(ref _currentFailCount, -FailCount);
                }
                else
                {
                    Interlocked.Increment(ref _currentFailCount);

                    throw new InvalidOperationException("The printer currently cannot accept print requests");
                }
            }

            Interlocked.Increment(ref _runCount);
            base.WriteJobDataToStream(printJob, printJobData);
        }
    }
}
