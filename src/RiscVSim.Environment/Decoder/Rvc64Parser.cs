using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class Rvc64Parser
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Rvc64Parser()
        {

        }

        public void ParseLd(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.LD");
            // rs1
            // rd
            // Offset :  5 4 3 7 6 (scaled by 8 (3 2 1 0))

            instructionPayload.Funct3 = 3;
            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.SignedImmediate = DecodeLoadStoreOffset(rvcPayload.Immediate);
        }

        public void ParseSd(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.SD");

            instructionPayload.Funct3 = 3;
            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Rs2 = rvcPayload.Rs2;
            instructionPayload.SignedImmediate = DecodeLoadStoreOffset(rvcPayload.Immediate);
        }

        public void ParseAddiW(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.AddiW");

            // addiw   rd rs1 imm12            14..12=0 6..2=0x06 1..0=3

            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Rs1 = rvcPayload.Rs1;

            var immediate = MathHelper.GetSignedInteger(rvcPayload.Immediate, 5);
            instructionPayload.SignedImmediate = immediate;
        }


        public void ParseAddWSubW(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.AddW / C.SubW");

            // addw rd rs1 rs2 31..25 = 0  14..12 = 0 6..2 = 0x0E 1..0 = 3
            // subw rd rs1 rs2 31..25 = 32 14..12 = 0 6..2 = 0x0E 1..0 = 3

            instructionPayload.Funct3 = 0;

            // C.SUBW
            if (rvcPayload.CAMode == 0)
            {
                instructionPayload.Funct7 = 0x32;
            }

            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Rs2 = rvcPayload.Rs2;
            instructionPayload.Rd = rvcPayload.Rd;
        }

        public void ParseSlli(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.SLLI");

            // slli    rd rs1 31..26=0  shamt 14..12=1 6..2=0x04 1..0=3
            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Rs1 = rvcPayload.Rs1;
            instructionPayload.Funct3 = 1;
            instructionPayload.SignedImmediate = rvcPayload.Immediate;

        }

        public void ParseLdSp(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.LDSP");

            // ld      rd rs1       imm12 14..12=3 6..2=0x00 1..0=3

            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Rs1 = 2;
            instructionPayload.Funct3 = 3;
            instructionPayload.SignedImmediate = DecodeLoadStoreSpOffset(rvcPayload.Immediate);
        }


        public void ParseSdSp(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.SDSP");

            // sd     imm12hi rs1 rs2 imm12lo 14..12=3 6..2=0x08 1..0=3

            instructionPayload.Rd = rvcPayload.Rd;
            instructionPayload.Rs1 = 2;
            instructionPayload.Rs2 = rvcPayload.Rs2;
            instructionPayload.Funct3 = 3;
            instructionPayload.SignedImmediate = DecodeLoadStoreSpOffset(rvcPayload.Immediate);
        }

        #region Helper

        private int DecodeLoadStoreOffset(int coding)
        {
            // Offset :  5 4 3 7 6 (scaled by 8 (3 2 1 0))

            int buffer = coding;
            
            // 7 6
            int immediate = buffer & 0x03;
            immediate <<= 6;

            // 5 4 3
            buffer >>= 2;
            int b543 = buffer & 0x07;
            b543 <<= 3;

            immediate |= b543;
            return immediate;

        }

        private int DecodeLoadStoreSpOffset(int coding)
        {
            // Offset 5 4 3 8 7 6

            int buffer = coding;

            // 8 7 6
            int immediate = buffer & 0x07;
            immediate <<= 6;

            // 5 4 3
            buffer >>= 3;
            int b54 = buffer & 0x07;
            b54 <<= 3;

            immediate |= b54;
            return immediate;
        }

        #endregion
    }
}
