using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServerImplementation.Contracts
{
    public interface IPrintServer
    {
        public void AddPrinterToServer(IPrinter newPrinter);

        public void RemovePrinterFromServer(IPrinter printer);

        public void Print(IPrintJob printJob);
        public void Print(IEnumerable<IPrintJob> printJobs);

        public Task PrintAsync(IPrintJob printJob);
        public Task PrintAsync(IEnumerable<IPrintJob> printJobs);

    }
}
