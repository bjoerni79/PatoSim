using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface ICsrRegister
    {
        void Write(int registerIndex, byte value);

        byte Read(int registerIndex);
    }
}
