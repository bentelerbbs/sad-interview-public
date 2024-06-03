using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWriteMultithreading
{
    /// <summary>
    /// This class is ready, please do NOT change
    /// </summary>
    public abstract class LoggerBase
    {
        public string FilePath { get; set; }

        public LoggerBase(string path)
        {
            FilePath = path;
        }
    }
}
