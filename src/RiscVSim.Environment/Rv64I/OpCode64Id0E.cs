using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id0E : OpCodeCommand
    {
        private Multiplier multiplier;
        private Divider divider;

        public OpCode64Id0E(IMemory memory, IRegister register) : base(memory,register)
        {
            multiplier = new Multiplier(Architecture.Rv64I, register);
            divider = new Divider(Architecture.Rv64I, register);
        }

        public override int Opcode => 0x0E;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            /*
             * # RV64M
                mulw    rd rs1 rs2 31..25=1 14..12=0 6..2=0x0E 1..0=3
                divw    rd rs1 rs2 31..25=1 14..12=4 6..2=0x0E 1..0=3
                divuw   rd rs1 rs2 31..25=1 14..12=5 6..2=0x0E 1..0=3
                remw    rd rs1 rs2 31..25=1 14..12=6 6..2=0x0E 1..0=3
                remuw   rd rs1 rs2 31..25=1 14..12=7 6..2=0x0E 1..0=3
             * 
             */

            if (payload.Funct7 != 0x01)
            {
                throw new RiscVSimException("Invalid F7 Coding for RV64M opcode detected!");
            }

            var rd = payload.Rd;
            var rs1Coding = Register.ReadBlock(payload.Rs1);
            var rs1CodingLower = rs1Coding.Take(4);

            var rs2Coding = Register.ReadBlock(payload.Rs2);
            var rs2CodingLower = rs2Coding.Take(4);


            switch (payload.Funct3)
            {
                // mulw
                case 0:
                    multiplier.ExecuteMulw(rd, rs1CodingLower, rs2CodingLower);
                    break;

                case 4:
                case 5:
                    divider.Divw(rd, rs1CodingLower, rs2CodingLower);
                    break;

                case 6:
                case 7:
                    divider.Remw(rd, rs1CodingLower, rs2CodingLower);
                    break;

                // Error
                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, payload.Funct3));
            }

            return true;
        }
    }
}
