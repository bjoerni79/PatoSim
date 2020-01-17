using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface ICpu32 : ICpu
    {
        void AssignRasStack(Stack<uint> rasStack);
    }
}
