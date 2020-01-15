using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    public class OpCode08 : OpCodeCommand
    {
        public OpCode08 (IMemory memory, Register register) : base (memory,register)
        {
            // base ()
        }

        public override int Opcode => 0x08;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            return true;
        }
    }
}
