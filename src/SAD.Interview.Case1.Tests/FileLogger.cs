using System.Collections.Concurrent;
using System.Text;

namespace SAD.Interview.Case1
{
    public class FileLogger
    {
        public string FilePath { get; set; }

        public FileLogger(string path)
        {
            FilePath = path;
        }

        public string LogAsString()
        {
            return "";
        }

        public void Trace(string line)
        {

        }
    }
}
