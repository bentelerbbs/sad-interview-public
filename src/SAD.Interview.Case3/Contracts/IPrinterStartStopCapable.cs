﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Interview.Case3.Contracts
{
    public interface IStartStopCapable
    {
        bool IsStarted { get; }

        void Start();

        void Stop();
    }
}