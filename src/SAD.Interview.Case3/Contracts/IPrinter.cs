using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServerImplementation.Contracts
{
    public interface IPrinter
    {
        /// <summary>
        /// Prints the provided PrintJob, if the printer is not able to print the job, it should throw an exception.
        /// </summary>
        /// <param name="printJob">Print Job to be printed</param>
        /// <exception cref="InvalidOperationException">If the printer is not able to print the job</exception>
        public void Print(IPrintJob printJob);

        /// <summary>
        /// Prints the provided PrintJob, if the printer is not able to print the job, it should throw an exception.
        /// </summary>
        /// <param name="printJob">Print Job to be printed</param>
        /// <exception cref="InvalidOperationException">If the printer is not able to print the job</exception>
        public Task PrintAsync(IPrintJob printJob);

        /// <summary>
        /// Returns the jobs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPrintJob> GetPrintJobsInQueue();
    }
}
