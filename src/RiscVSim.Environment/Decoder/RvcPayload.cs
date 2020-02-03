using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class RvcPayload
    {
        public RvcPayload()
        {

        }

        public void LoadCI(int op, int immm, int rd, int f3)
        {
            Op = op;
            Rd = rd;
            Funct3 = f3;
            Immediate = immm;
            Type = InstructionType.RVC_CI;
        }

        public InstructionType Type { get; private set; }

        public int Op { get; private set; }

        public int Rs1 { get; private set; }

        public int Rs2 { get; private set; }

        public int Rd { get; private set; }

        public int Funct3 { get; private set; }

        public int Immediate { get; private set; }

    }
}
