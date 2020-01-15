using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test
{
    public static class InstructionTypeFactory
    {
        public static IEnumerable<byte> CreateRType(uint opcode, uint rd, uint funct3, uint rs1, uint rs2, uint funct7)
        {
            //
            // funct7   = 7 Bits
            // rs2      = 5 Bits
            // 
            //

            uint buffer = 0;

            // Write funct7
            buffer = funct7;
            // Write RS2
            buffer <<= 5;
            buffer |= rs2;
            // Write RS1
            buffer <<= 5;
            buffer |= rs1;
            // Write funct3
            buffer <<= 3;
            buffer |= funct3;
            // Write RD
            buffer <<= 5;
            buffer |= rd;
            // Write Opcode
            buffer <<= 5;
            buffer |= opcode;
            // Write 11 (32 Bit pattern)
            buffer <<= 2;
            buffer |= 3;

            return BuildInstruction(buffer);
        }



        public static IEnumerable<byte> CreateIType (uint opcode, uint rd, uint funct3, uint rs1, uint immediate)
        {
            // opcode = 4
            // rd = 1;
            // f3 = 0;
            // rs1 = 2
            // i = 8

            uint buffer = 0;

            buffer = immediate;
            // Write RS1
            buffer <<= 5;
            buffer |= rs1;
            // Write f3
            buffer <<= 3;
            buffer |= funct3;
            // Write RD
            buffer <<= 5;
            buffer |= rd;
            // Write Opcode
            buffer <<= 5;
            buffer |= opcode;
            // Write 11 (32 Bit pattern)
            buffer <<= 2;
            buffer |= 3;
            
            return BuildInstruction(buffer);
        }

        public static IEnumerable<byte> CreateUType(uint opcode, uint rd, uint immediate)
        {
            // imm20 rd opcode

            uint buffer;
            buffer = immediate;
            buffer <<= 5;
            buffer |= rd;
            buffer <<= 5;
            buffer |= opcode; 
            buffer <<= 2;
            buffer |= 3;

            return BuildInstruction(buffer);
        }

        public static IEnumerable<byte> CreateNop()
        {
            var nop = CreateIType(Constant.opOPIMM, 0, Constant.opOPIMMaddi, 0, 0);
            return nop;
        }

        private static IEnumerable<byte> BuildInstruction(uint buffer)
        {
            // Build the instruction
            var b1 = buffer & 0xFF;
            var b2 = buffer & 0xFF00;
            var b3 = buffer & 0xFF0000;
            var b4 = buffer & 0xFF000000;

            b2 >>= 8;
            b3 >>= 16;
            b4 >>= 24;

            var instruction = new byte[4];
            instruction[0] = Convert.ToByte(b1);
            instruction[1] = Convert.ToByte(b2);
            instruction[2] = Convert.ToByte(b3);
            instruction[3] = Convert.ToByte(b4);
            return instruction;
        }

    }
}
