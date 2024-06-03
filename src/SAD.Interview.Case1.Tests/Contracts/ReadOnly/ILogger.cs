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
    public interface ILogger
    {
        /// <summary>
        /// Returns all the text written by this class instance
        /// |
        /// Expectations: As fast and optimized for CPU and disk as possible, NewLine at the end of string
        /// </summary>
        /// <returns></returns>
        public string AllTextWrittenByThisInstance();

        /// <summary>
        /// Writes a line with prefix '{DateTime} [TRACE]' to the target file
        /// |
        /// Expectations: Thread-safe, no data loss, as fast as possible
        /// </summary>
        /// <param name="line"></param>
        public void Trace(string line);
    }
}
