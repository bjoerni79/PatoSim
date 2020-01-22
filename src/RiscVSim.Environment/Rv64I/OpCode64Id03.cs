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
            var rs1 = payload.Rs1;
            var rd = payload.Rd;
            var f3 = payload.Funct3;


            Logger.Info("Opcode 03: rd={rd}, rs1={rs1}, funct3={funct3}, Unsigned Imm = {uimm:X}", rd, rs1, f3, payload.UnsignedImmediate);

            return true;
        }
    }
}
