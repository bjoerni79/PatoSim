using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class OpCode32Id05 : OpCodeCommand
    {


        public OpCode32Id05(IMemory memory, IRegister register) : base(memory,register)
        {

        }

        public override int Opcode => 05;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            // AUIPC (add upper immediate to pc) is used to build pc-relative addresses and uses the U-type format.AUIPC forms a 32 - bit offset from the 20 - bit U - immediate,
            // filling in the lowest 12 bits with zeros, adds this offset to the address of the AUIPC instruction, then places the result in register rd.

            uint workingBuffer;
            int rd = payload.Rd;
            var unsingedImmedaite = payload.UnsignedImmediate;
            var address = Register.ReadUnsignedInt(Register.ProgramCounter);

            // Shift the value to left
            workingBuffer = unsingedImmedaite;
            workingBuffer <<= 12;

            // Add the current address to the working buffer
            workingBuffer += address;

            // Write the value to target register
            Register.WriteUnsignedInt(rd, workingBuffer);

            return true;
        }
    }
}
