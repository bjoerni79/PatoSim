using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id03 : OpCodeCommand
    {
        public OpCode64Id03 (IMemory memory, IRegister register) : base(memory, register)
        {

        }

        public override int Opcode => 0x03;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            return true;
        }
    }
}
