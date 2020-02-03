using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public enum InstructionType
    {
        Unknown,
        // INST32 
        R_Type,
        I_Type,
        U_Type,
        S_Type,
        J_Type,
        B_Type,
        // Compressed types
        RVC,
    }
}
