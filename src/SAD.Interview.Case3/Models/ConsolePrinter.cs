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
    public class ConsolePrinter : StreamPrinter
    {
        private string _outputPrefix = string.Empty;
        private string _outputSuffix = string.Empty;

        public ConsolePrinter() : base(Console.OpenStandardOutput())
        {
        }

        public ConsolePrinter(string prefix, string suffix) : this()
        {
            _outputPrefix = prefix;
            _outputSuffix = suffix;
        }

        protected override void WriteJobDataToStream(IPrintJob printJob, byte[] printJobData)
        {
            OutputStream.Write(Encoding.UTF8.GetBytes(_outputPrefix));
            base.WriteJobDataToStream(printJob, printJobData);
            OutputStream.Write(Encoding.UTF8.GetBytes(_outputSuffix));
            OutputStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
        }
    }
}
