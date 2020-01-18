using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id1C : OpCodeCommand
    {
        public OpCode64Id1C (IMemory memory, IRegister register) : base(memory,register)
        {

        }

        public override int Opcode => 0x1C;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            return true;
        }
    }
}
