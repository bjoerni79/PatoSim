﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public sealed class Instruction
    {
        internal Instruction(InstructionType type, int opCode, int instLength)
        {
            Type = type;
            OpCode = opCode;
            //RegisterDestination = rd;
            InstructionLength = instLength;
        }

        public InstructionType Type { get; }

        public int OpCode { get; }

        //public int RegisterDestination { get; }


        public int InstructionLength { get; }


    }
}
