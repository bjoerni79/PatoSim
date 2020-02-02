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

        public int Op { get; private set; }

        public int Rs1 { get; private set; }

        public int Rs2 { get; private set; }

        public int Rd { get; private set; }

        public int funct2 { get; private set; }

        public int funct3 { get; private set; }

        public int funct4 { get; private set; }

        public int funct6 { get; private set; }

        public int Immediate { get; private set; }

        public int Offset { get; private set; }

        public int JumpTarget { get; set; }
    }
}
