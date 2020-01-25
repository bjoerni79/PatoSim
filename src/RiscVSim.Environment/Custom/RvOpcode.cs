using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Custom
{
    public class RvOpcode : OpCodeCommand
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public RvOpcode(IMemory memory, IRegister register) : base(memory,register)
        {
        }

        public override int Opcode => 0x00;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var rs2 = payload.Rs2;
            var f3 = payload.Funct3;
            var f7 = payload.Funct7;

            Logger.Info("Opcode RV : rd = {rd}, rs1 = {rs1}, rs2 = {rs2}, f3 = {f3}, f7 = {f7}", rd, rs1, rs2, f3, f7);

            if (f7 == 0x02)
            {
                var rs1Value = Register.ReadSignedInt(rs1);

                Console.WriteLine(rs1Value);
            }

            return true;
        }
    }
}
