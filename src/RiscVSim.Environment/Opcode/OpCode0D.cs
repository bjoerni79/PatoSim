using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    public class OpCode0D : OpCodeCommand
    {

        public OpCode0D(IMemory memory, Register register) : base(memory,register)
        {
            // see base();
        }

        public override int Opcode => 0x0D;

        public override void Execute(Instruction instruction, InstructionPayload payload)
        {
            // LUI (load upper immediate) is used to build 32-bit constants and uses the U-type format. 
            // LUI places the U - immediate value in the top 20 bits of the destination register rd, filling in the lowest 12 bits with zeros.

            uint workingBuffer;
            int rd = payload.Rd;
            var unsingedImmedaite = payload.UnsignedImmediate;

            workingBuffer = unsingedImmedaite;
            workingBuffer <<= 12;

            Register.WriteUnsignedInt(rd, workingBuffer);
        }
    }
}
