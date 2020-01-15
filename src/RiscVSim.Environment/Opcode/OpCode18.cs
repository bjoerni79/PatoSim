using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    /// <summary>
    /// Implements the RV32I conditional branches BNE...
    /// </summary>
    public class OpCode18 : OpCodeCommand
    {
        public OpCode18 (IMemory memory, Register register) : base (memory,register)
        {
            // base (...)
        }

        public override int Opcode => 0x18;

        public override void Execute(Instruction instruction, InstructionPayload payload)
        {
            // Update x1 !
            // Push to ras ???
        }
    }
}
