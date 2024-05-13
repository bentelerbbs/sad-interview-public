using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServerImplementation.Contracts
{
    public interface IStartStopCapable
    {
        bool IsStarted { get; }

        void Start();

        void Stop();
    }
}
