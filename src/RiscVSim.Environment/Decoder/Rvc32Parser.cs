using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class Rvc32Parser
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Rvc32Parser()
        {

        }

        public void ParseLwSp(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            //
            // C.LWSP loads a 32-bit value from memory into register rd. It computes an effective address
            // by adding the zero-extended offset, scaled by 4, to the stack pointer, x2. It expands to lw rd,
            // offset[7:2](x2).C.LWSP is only valid when rd̸ = x0; the code points with rd = x0 are reserved.
            //
            // 15...13     12      11..7     6 ... 2         1..0
            // C.LWSP   offset[5] dest̸=0   offset[4:2|7:6]   C2
            //
            // Immediate :  [5] [4] [3] [2] [7] [6]

            Logger.Info("Parsing C.LWSP");

            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Funct3 = 2;
            instructionPayload.Rs1 = 2;

            int immediate = DecodeLoadStore(rvcPayload.Immediate);
            instructionPayload.SignedImmediate = immediate;
        }

        public void ParseSwSp(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            //
            // C.SWSP stores a 32-bit value in register rs2 to memory. It computes an effective address by
            // adding the zero - extended offset, scaled by 4, to the stack pointer, x2. It expands to sw rs2,
            // offset[7:2](x2).
            //
            // Immediate : [5] [4] [3] [2] [7] [6]

            Logger.Info("Parsing C.SWSP");

            instructionPayload.Funct3 = 2;
            instructionPayload.Rs2 = rvcPayload.Rs2; // The source
            instructionPayload.Rs1 = 2; // The register pointing to the memory address

            int immediate = DecodeLoadStore(rvcPayload.Immediate);
            instructionPayload.SignedImmediate = immediate;
        }

        public void ParseJrAndJalr (RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            //
            // F4 = 8
            // C.JR (jump register) performs an unconditional control transfer to the address in register rs1.
            // C.JR expands to jalr x0, 0(rs1).C.JR is only valid when rs1̸ = x0; the code point with rs1 = x0
            // is reserved.
            //
            // F4 = 9
            // C.JALR (jump and link register) performs the same operation as C.JR, but additionally writes the
            // address of the instruction following the jump(pc + 2) to the link register, x1. C.JALR expands to
            // jalr x1, 0(rs1).C.JALR is only valid when rs1̸ = x0; the code point with rs1 = x0 corresponds
            // to the C.EBREAK instruction.
            //
            // 

            Logger.Info("Parsing C.JR / C.JALR with F4 = {f4:X}",rvcPayload.Funct4);

            var f4 = rvcPayload.Funct4;
            if (f4 == 8)
            {
                // C.JR
                if (rvcPayload.Rs1 == 0)
                {
                    throw new RvcFormatException("Invalid C.JR coding. Rs1 cannot be 0");
                }

                instructionPayload.Rs1 = rvcPayload.Rs1;

            }

            if (f4 == 9)
            {
                // C.JALR
                if (rvcPayload.Rs1 == 0)
                {
                    throw new RvcFormatException("Invalid C.JALR coding. Rs1 cannot be 0");
                }

                instructionPayload.Rd = 1;
                instructionPayload.Rs1 = rvcPayload.Rs1;
            }

            // RvcFormatException
        }

        public void ParseSlli (RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            // C.SLLI is a CI-format instruction that performs a logical left shift of the value in register rd then
            // writes the result to rd.The shift amount is encoded in the shamt field.

            Logger.Info("Parsing C.SLLI");

            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Funct3 = 1;
            // The type decoder automatically generated the correct order
            instructionPayload.SignedImmediate = rvcPayload.Immediate;
        }

        #region Helper

        private int DecodeLoadStore (int immediateCoding)
        {
            int immediate;
            int buffer = immediateCoding;

            // Extract from right to left and shift
            // 7,6
            immediate = buffer & 0x03;
            immediate <<= 6;

            // 4,3,2
            buffer >>= 2;
            var b432 = buffer & 0x07;
            b432 <<= 2;
            immediate |= b432;

            // 5
            buffer >>= 3;
            var b5 = buffer & 0x01;
            b5 <<= 5;
            immediate |= b5;

            return immediate;
        }

        #endregion
    }
}
