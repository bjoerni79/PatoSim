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

        public void ParseLw(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            //
            // C.LW loads a 32-bit value from memory into register rd ′. It computes an effective address by
            // adding the zero - extended offset, scaled by 4, to the base address in register rs1 ′. It expands to lw
            // rd ′, offset[6:2](rs1 ′).
            //

            Logger.Info("Parsing C.LW");

            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Funct3 = 2;

            int immediate = DecodeLoadStoreW(rvcPayload.Immediate);
            instructionPayload.SignedImmediate = immediate;

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

            int immediate = DecodeLoadStoreSp(rvcPayload.Immediate);
            instructionPayload.SignedImmediate = immediate;
        }

        public void ParseSw (RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            //
            // C.SW stores a 32-bit value in register rs2 ′ to memory. It computes an effective address by adding
            // the zero-extended offset, scaled by 4, to the base address in register rs1 ′. It expands to sw rs2 ′,
            // offset[6:2](rs1 ′).
            //

            Logger.Info("Parsing C.SP");

            instructionPayload.Funct3 = 2;
            instructionPayload.Rs2 = rvcPayload.Rs2; // The source
            instructionPayload.Rs1 = rvcPayload.Rs1; // The register pointing to the memory address

            int immediate = DecodeLoadStoreW(rvcPayload.Immediate);
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

            int immediate = DecodeLoadStoreSp(rvcPayload.Immediate);
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

        public  void ParseAddi4Spn (RvcPayload payload, InstructionPayload instructionPayload)
        {
            // C.ADDI4SPN is a CIW-format instruction that adds a zero-extended non-zero immediate, scaled
            // by 4, to the stack pointer, x2, and writes the result to rd ′. This instruction is used to generate
            // pointers to stack-allocated variables, and expands to addi rd ′, x2, nzuimm[9:2].C.ADDI4SPN
            // is only valid when nzuimm̸ = 0; the code points with nzuimm = 0 are reserved.

            Logger.Info("Parsing C.ADDI4SPN");

            // nzuimm  5 4 9 8 7 6 2 3

            instructionPayload.Rs1 = 2;
            instructionPayload.Rd = payload.Rd;
            instructionPayload.Funct3 = 0;

            // 3
            int buffer = payload.Immediate;
            int immediate = buffer & 0x01;
            int current;
            immediate <<= 3;

            // 2
            buffer >>= 1;
            current = buffer & 0x01;
            current <<= 2;
            immediate |= current;

            // 9 8 7 6 
            buffer >>= 1;
            current = buffer & 0x0F;
            current <<= 6;
            immediate |= current;

            // 5 4
            buffer >>= 4;
            current = buffer & 0x03;
            current <<= 4;
            immediate |= current;


            instructionPayload.SignedImmediate = immediate;
        }

        public void ParseJ (RvcPayload payload, InstructionPayload instructionPayload)
        {
            instructionPayload.Rd = 0;
            instructionPayload.SignedImmediate = DecodeJalOffset(payload.Immediate);
        }

        public void ParseJal (RvcPayload payload, InstructionPayload instructionPayload)
        {
            instructionPayload.Rd = 1;
            instructionPayload.SignedImmediate = DecodeJalOffset(payload.Immediate);
        }

        #region Helper

        private int DecodeJalOffset(int immediateCoding)
        {
            // 11 4 9 8 10 6 7 3 2 1 5
            // There is no 0 index this time!

            uint current;
            uint buffer = Convert.ToUInt32(immediateCoding);
            
            // 5
            uint immediate = buffer & 0x01;
            immediate <<= 4;

            // 3 2 1
            buffer >>= 1;
            current = buffer & 0x07;
            immediate |= current;

            // 7
            buffer >>= 3;
            current = buffer & 0x01;
            current <<= 6;
            immediate |= current;

            // 6
            buffer >>= 1;
            current = buffer & 0x01;
            current <<= 5;
            immediate |= current;

            // 10
            buffer >>= 1;
            current = buffer & 0x01;
            current <<= 9;
            immediate |= current;

            // 9 8
            buffer >>= 1;
            current = buffer & 0x03;
            current <<= 7;
            immediate |= current;

            // 4
            buffer >>= 2;
            current = buffer & 0x03;
            current <<= 3;
            immediate |= current;

            // 11
            buffer >>= 1;
            current = buffer & 0x01;
            current <<= 10;
            immediate |= current;


            immediate <<= 1;
            var result = MathHelper.GetSignedInteger(immediate, InstructionType.J_Type);

            // Signed? -> Mathhelper
            return result;
        }

        private int DecodeLoadStoreW(int immediateCoding)
        {
            // 5 4 3 2 6

            int immediate;
            int buffer = immediateCoding;

            // 6
            immediate = buffer & 0x01;
            immediate <<= 4;

            // 2 3 4 5
            buffer >>= 1;
            immediate |= buffer & 0xF;


            immediate <<= 2;
            return immediate;
        }

        private int DecodeLoadStoreSp (int immediateCoding)
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
