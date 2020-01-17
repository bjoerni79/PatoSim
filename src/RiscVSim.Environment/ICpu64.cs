using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface ICpu64 : ICpu
    {
        void AssignRasStack(Stack<ulong> rasStack);
    }
}
