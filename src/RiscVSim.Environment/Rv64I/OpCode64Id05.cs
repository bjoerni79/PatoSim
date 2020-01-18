using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id05 : OpCodeCommand
    {
        public OpCode64Id05(IMemory memory, IRegister register) : base(memory,register)
        {
            // base...)
        }

        public override int Opcode => 0x05;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            // AUIPC (add upper immediate to pc) is used to build pc-relative addresses and uses the U-type format.AUIPC forms a 32 - bit offset from the 20 - bit U - immediate,
            // filling in the lowest 12 bits with zeros, adds this offset to the address of the AUIPC instruction, then places the result in register rd.

            uint workingBuffer;
            int rd = payload.Rd;
            var unsingedImmedaite = payload.UnsignedImmediate;
            var address = Register.ReadUnsignedInt(Register.ProgramCounter);

            workingBuffer = unsingedImmedaite;
            workingBuffer <<= 12;

            workingBuffer += address;

            Register.WriteUnsignedInt(rd, workingBuffer);

            // RV64I
            // AUIPC (add upper immediate to pc) uses the same opcode as RV32I. AUIPC is used to build pcrelative
            // addresses and uses the U - type format.AUIPC appends 12 low - order zero bits to the 20 - bit
            // U - immediate, sign - extends the result to 64 bits, adds it to the address of the AUIPC instruction,
            // then places the result in register rd.

            // https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/sign-extension

            return true;
        }
    }
}
