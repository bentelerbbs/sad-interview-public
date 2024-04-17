using SAD.Interview.Case3.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Interview.Case3.Tests.Models
{
    internal class PrintJobSpeedByLength : IPrintJob
    {
        public int PrintJobDurationInMilliseconds { get => _content.Length * 5; }

        public int PrintCount { get; private set; }

        private readonly string _content;

        public PrintJobSpeedByLength(string content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string PreparePrintOutput()
        {
            PrintCount++;

            Task.Delay(PrintJobDurationInMilliseconds).Wait();

            return _content;
        }
    }
}
