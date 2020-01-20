using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public enum Architecture
    {
        Unknown,
        Rv32I, // 4 Bytes = 32 Bit
        Rv32E,
        Rv64I
    };
}
