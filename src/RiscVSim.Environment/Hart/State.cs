using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    public enum State
    {
        Init,
        Waiting,
        Running,
        Stopped,
        FatalError
    }
}
