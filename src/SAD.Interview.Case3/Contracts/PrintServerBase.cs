using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServerImplementation.Contracts
{
    public abstract class PrintServerBase : IPrintServer
    {
        public abstract void AddPrinterToServer(IPrinter newPrinter);
        public abstract void Print(IPrintJob printJob);
        public abstract Task PrintAsync(IPrintJob printJob);
        public abstract void RemovePrinterFromServer(IPrinter printer);

        public virtual void Print(IEnumerable<IPrintJob> printJobs) => Parallel.ForEach(printJobs, Print);
        public virtual async Task PrintAsync(IEnumerable<IPrintJob> printJobs) => await Parallel.ForEachAsync(printJobs, async (pj, ct) => await PrintAsync(pj));
    }
}
