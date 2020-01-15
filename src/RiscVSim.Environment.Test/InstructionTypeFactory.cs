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

        public static IEnumerable<byte> CreateSType(uint opcode, uint funct3, uint rs1, uint rs2, int immediate)
        {
            //
            // funct7   = 7 Bits
            // rs2      = 5 Bits
            // 
            //

            var firstPart = immediate & 0x1F;
            var secondPart = immediate;
            secondPart >>= 5;


            uint buffer;

            // Write funct7
            buffer = Convert.ToUInt32(secondPart);
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
            buffer |= Convert.ToUInt32(firstPart);
            // Write Opcode
            buffer <<= 5;
            buffer |= opcode;
            // Write 11 (32 Bit pattern)
            buffer <<= 2;
            buffer |= 3;

            return BuildInstruction(buffer);
        }

        public static IEnumerable<byte> CreateBType (uint opcode, uint rs1, uint rs2, uint funct3, int immediate)
        {
            uint buffer = 0;
            int block4 = 0;

            if (immediate < 0)
            {
                block4 = 1;
            }

            var internalImmediate = immediate >> 1;

            var block3 = internalImmediate >> 4;
            block3 &= 0x3F;

            var block2 = internalImmediate & 0x80;
            block2 >>= 7;

            var block1 = internalImmediate & 0x1E;
            block1 >>= 1;

            // block4, block3
            uint funct7Value = Convert.ToUInt32(block4);
            funct7Value <<= 6;
            funct7Value |= Convert.ToUInt32(block3);


            uint rdValue = Convert.ToUInt32(block1);
            rdValue <<= 1;
            rdValue |= Convert.ToUInt32(block2);



            //
            // funct7   = 7 Bits
            // rs2      = 5 Bits
            // 
            //


            // Write funct7
            buffer = funct7Value;
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
            buffer |= rdValue;
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
            var nop = CreateIType(Constant.OPIMM, 0, Constant.opOPIMMaddi, 0, 0);
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
