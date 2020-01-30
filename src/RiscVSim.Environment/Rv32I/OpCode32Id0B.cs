using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class OpCode32Id0B : OpCodeCommand
    {
        private AtomicInstruction atomic;

        public OpCode32Id0B(IMemory memory, IRegister register) : base(memory,register)
        {
            atomic = new AtomicInstruction(memory, register);
        }

        public override int Opcode => 0x0B;



        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rs1 = payload.Rs1;
            var rs2 = payload.Rs2;
            var rd = payload.Rd;
            var f3 = payload.Rd;

            // F7 pattern
            var f7 = payload.Funct7;
            var f5 = f7 >> 2;           // bits 31 ... 27
            var aq = (f7 & 0x02) >> 1;  // acquire bit 26
            var rl = f7 & 0x01;         // release bit 25

            Logger.Info("OpCode 0B : rd = {rd}, rs1 = {rs1}, rs2 = {rs2}, funct3 = {f3}, aq = {aq}, rl = {rl}, f5 = {f5}", rd, rs1, rs2, f3, aq, rl, f5);

            atomic.ExecuteW(rd, rs1, rs2, rl, aq, f5);

            return true;
        }
    }
}
