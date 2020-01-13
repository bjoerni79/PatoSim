using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public sealed class Instruction
    {
        private InstructionType type;
        private int opCode;
        private int rd;
        private int instLength;
        private IEnumerable<byte> coding;

        internal Instruction(InstructionType type, int opCode, int rd,int instLength)
        {
            this.type = type;
            this.opCode = opCode;
            this.rd = rd;
            this.instLength = instLength;
        }

        public InstructionType Type 
        { 
            get
            {
                return type;
            }
        }

        public int OpCode 
        { 
            get
            {
                return opCode;
            }
        }

        public int RegisterDestination 
        { 
            get
            {
                return rd;
            }
        }

        public bool IsHint
        {
            get
            {
                return rd == 0;
            }
        }

        public int InstructionLength
        {
            get
            {
                return instLength;
            }
        }
    }
}
