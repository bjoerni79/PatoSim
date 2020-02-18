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
            Logger.Info("Parsing C.J");

            instructionPayload.Rd = 0;
            instructionPayload.SignedImmediate = DecodeJalOffset(payload.Immediate);
        }

        public void ParseJal (RvcPayload payload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.JAL");

            instructionPayload.Rd = 1;
            instructionPayload.SignedImmediate = DecodeJalOffset(payload.Immediate);
        }

        public void ParseBeqzAndBnez (RvcPayload payload, InstructionPayload instructionPayload, bool forEqual)
        {
            Logger.Info("Parsing C.BEQZ / C.BNEZ");

            if (forEqual)
            {
                instructionPayload.Funct3 = 0;
            }
            else
            {
                instructionPayload.Funct3 = 1;
            }

            instructionPayload.Rs1 = payload.Rs1;
            instructionPayload.SignedImmediate = DecodeCbOffset(payload.Immediate);
        }

        public void ParseLi (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.LI loads the sign-extended 6-bit immediate, imm, into register rd. C.LI expands into addi rd,
            // x0, imm[5:0].C.LI is only valid when rd̸ = x0; the code points with rd = x0 encode HINTs.
            //

            // rd = Signed Bit/5  4 3 2 1 0

            // TODO: rd == 0 -> Hint... how?
            // for now:  throw an error with an indicatatio that the C.LI hint mode is not supported

            // ---> MathHelper...

            Logger.Info("Parsing C.LI");

            if (payload.Rd == 0)
            {
                throw new RvcFormatException("C.LI using hint mode (rd=0) is currently not supported");
            }

            instructionPayload.Rd = payload.Rd;

            // bit 4 3 2 1 0
            int buffer = payload.Immediate;
            int signedImmediate = buffer & 0x1F;

            // signed bit 5
            buffer >>= 5; 
            var b5 = buffer & 0x01;
            b5 <<= 5;

            signedImmediate |= b5;
            instructionPayload.SignedImmediate = MathHelper.GetSignedInteger(signedImmediate, 5);

        }

        public void ParseLui (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.LUI loads the non-zero 6-bit immediate field into bits 17–12 of the destination register, clears the
            // bottom 12 bits, and sign-extends bit 17 into all higher bits of the destination.C.LUI expands into
            // lui rd, nzimm[17:12].C.LUI is only valid when rd̸ = { x0, x2 }, and when the immediate is not
            // equal to zero. The code points with nzimm = 0 are reserved; the remaining code points with rd = x0
            // are HINTs; and the remaining code points with rd = x2 correspond to the C.ADDI16SP instruction.
            //

            Logger.Info("Parsing C.LUI");

            // nzimm[17...12] = payload.immediate

            if (payload.Rd == 0 || payload.Rd == 2)
            {
                throw new RvcFormatException("C.LUI does not accept RD=0 or RD=2");
            }

            instructionPayload.Rd = payload.Rd;
            uint uimmediate = Convert.ToUInt32(payload.Immediate << 12);

            // Expand Bit 17 to the higher ones (31....17)
            uint bitmask = 0xFFFFC000;
            uint b17 = 0x020000;

            if ((uimmediate & b17) == b17)
            {
                uimmediate |= bitmask;
            }

            instructionPayload.UnsignedImmediate = uimmediate;
        }

        public void ParseAddi (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.ADDI adds the non-zero sign-extended 6-bit immediate to the value in register rd then writes
            // the result to rd. C.ADDI expands into addi rd, rd, nzimm[5:0].C.ADDI is only valid when
            // rd̸ = x0 and nzimm̸ = 0.The code points with rd = x0 encode the C.NOP instruction; the remaining
            // code points with nzimm = 0 encode HINTs.
            //

            Logger.Info("Parsing C.ADDI");

            instructionPayload.Rd = payload.Rd;
            instructionPayload.Rs1 = payload.Rs1;
            instructionPayload.SignedImmediate = payload.Immediate; // non-zero immediate.
        }


        public void ParseAddi16Sp (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.ADDI16SP shares the opcode with C.LUI, but has a destination field of x2. C.ADDI16SP adds
            // the non-zero sign - extended 6 - bit immediate to the value in the stack pointer(sp = x2), where the
            // immediate is scaled to represent multiples of 16 in the range(-512,496). C.ADDI16SP is used
            // to adjust the stack pointer in procedure prologues and epilogues. It expands into addi x2, x2,
            // nzimm[9:4].C.ADDI16SP is only valid when nzimm̸ = 0; the code point with nzimm = 0 is reserved.
            //

            Logger.Info("Parsing C.ADDI16SP");

            instructionPayload.Rd = 2;
            instructionPayload.Rs1 = 2;

            // b5
            int buffer = payload.Immediate;
            int immediate = buffer & 0x01;
            immediate <<= 5;

            // 8 7
            buffer >>= 1;
            int b87 = buffer & 0x03;
            b87 <<= 7;
            immediate |= b87;

            // 6
            buffer >>= 2;
            int b6 = buffer & 0x01;
            b6 <<= 6;
            immediate |= b6;

            // 4
            buffer >>= 1;
            int b4 = buffer & 0x01;
            b4 <<= 4;
            immediate |= b4;

            // 9
            buffer >>= 1;
            int b9 = buffer & 0x01;
            b9 <<= 9;
            immediate |= b9;

            instructionPayload.SignedImmediate = immediate;
            

        }

        public void ParseSrli (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.SRLI is a CB-format instruction that performs a logical right shift of the value in register rd ′
            // then writes the result to rd ′. The shift amount is encoded in the shamt field.For RV128C, a shift
            // amount of zero is used to encode a shift of 64.Furthermore, the shift amount is sign - extended for
            // RV128C, and so the legal shift amounts are 1–31, 64, and 96–127.C.SRLI expands into srli rd ′,
            // rd ′, shamt[5:0], except for RV128C with shamt = 0, which expands to srli rd ′, rd ′, 64.
            //
            // For RV32C, shamt[5] must be zero; the code points with shamt[5]=1 are reserved for custom
            // extensions.For RV32C and RV64C, the shift amount must be non - zero; the code points with
            // shamt = 0 are HINTs.

            Logger.Info("Parsing C.SRLI");

            if ((payload.Immediate & 0x20) == 0x20)
            {
                throw new RvcFormatException("Bit 5 for the RV32C C.SRLI must be zero");
            }

            // SRLI rd`,rd`,shamt[5:0]
            // srli    rd rs1 31..26=0  shamt 14..12=5 6..2=0x04 1..0=3


            instructionPayload.Funct3 = 5;
            instructionPayload.Rs1 = payload.Rs1;
            instructionPayload.Rd = payload.Rd;
            instructionPayload.SignedImmediate = payload.Immediate;


        }

        public void ParseSrai (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.SRAI is defined analogously to C.SRLI, but instead performs an arithmetic right shift. C.SRAI
            // expands to srai rd ′, rd ′, shamt[5:0].
            //

            // srai rd rs1 31..26 = 16 shamt 14..12 = 5 6..2 = 0x04 1..0 = 3
            Logger.Info("Parsing C.SRAI");

            if ((payload.Immediate & 0x20) == 0x20)
            {
                throw new RvcFormatException("Bit 5 for the RV32C C.SRAI must be zero");
            }

            instructionPayload.Funct3 = 5;
            instructionPayload.Rs1 = payload.Rs1;
            instructionPayload.Rd = payload.Rd;
            instructionPayload.SignedImmediate = payload.Immediate | 0x400;

        }

        public void ParseAndi (RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // C.ANDI is a CB-format instruction that computes the bitwise AND of the value in register rd ′ and
            // the sign-extended 6 - bit immediate, then writes the result to rd ′. C.ANDI expands to andi rd ′,
            // rd ′, imm[5:0].
            //
        }

        public void ParseCaGeneric(RvcPayload payload, InstructionPayload instructionPayload)
        {
            //
            // These instructions use the CA format.
            //
            // C.AND computes the bitwise AND of the values in registers rd ′ and rs2 ′, then writes the result to
            // register rd ′. C.AND expands into and rd ′, rd ′, rs2 ′.
            //
            // C.OR computes the bitwise OR of the values in registers rd ′ and rs2 ′, then writes the result to
            // register rd ′. C.OR expands into or rd ′, rd ′, rs2 ′.
            //
            // C.XOR computes the bitwise XOR of the values in registers rd ′ and rs2 ′, then writes the result to
            // register rd ′. C.XOR expands into xor rd ′, rd ′, rs2 ′.
            //
            // C.SUB subtracts the value in register rs2 ′ from the value in register rd ′, then writes the result to
            // register rd ′. C.SUB expands into sub rd ′, rd ′, rs2 ′.
            //
            // C.ADDW is an RV64C / RV128C - only instruction that adds the values in registers rd ′ and rs2 ′,
            // then sign - extends the lower 32 bits of the sum before writing the result to register rd ′. C.ADDW
            //
            // expands into addw rd ′, rd ′, rs2 ′.
            // C.SUBW is an RV64C / RV128C - only instruction that subtracts the value in register rs2 ′ from the
            // value in register rd ′, then sign-extends the lower 32 bits of the difference before writing the result
            // to register rd ′. C.SUBW expands into subw rd ′, rd ′, rs2 ′.
        }

        public void ParseNop(RvcPayload payload, InstructionPayload instructionPayload)
        {
            // C.NOP is a CI-format instruction that does not change any user-visible state, except for advancing
            // the pc and incrementing any applicable performance counters. C.NOP expands to nop. C.NOP is
            // only valid when imm = 0; the code points with imm̸ = 0 encode HINTs.
        }

        #region Helper

        private int DecodeCbOffset (int immediateCoding)
        {
            // 8 4 3 7 6 2 1 5 ..beginning with Index 1!
            int buffer = immediateCoding;
            int current;

            // 5
            int immediate = buffer & 0x01;
            immediate <<= 4;

            // 2 1
            buffer >>= 1;
            immediate |= buffer & 0x03;

            // 7 6
            buffer >>= 2;
            current = buffer & 0x03;
            current <<= 5;
            immediate |= current;

            // 4 3
            buffer >>= 2;
            current = buffer & 0x03;
            current <<= 2;
            immediate |= current;

            // 8
            buffer >>= 2;
            current = buffer & 0x01;
            current <<= 7;
            immediate |= current;

            immediate <<= 1;

            return MathHelper.GetSignedInteger(immediate, InstructionType.B_Type);
        }

        private int DecodeJalOffset(int immediateCoding)
        {
            // 11 4 9 8 10 6 7 3 2 1 5
            // There is no 0 index this time!

            int current;
            int buffer = immediateCoding;
            
            // 5
            int immediate = buffer & 0x01;
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
