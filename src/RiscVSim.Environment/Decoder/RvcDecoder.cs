using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class RvcDecoder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Architecture architecture;


        public RvcDecoder(Architecture architecture)
        {
            this.architecture = architecture;
        }

        public RvcPayload Decode (IEnumerable<byte> rvcCoding)
        {
            RvcPayload payload = null;

            if (rvcCoding == null)
            {
                throw new ArgumentNullException("rvcCoding");
            }

            // Start the parsing with the the opcode and the F3

            var firstByte = rvcCoding.First();
            var secondByte = rvcCoding.ElementAt(1);
            var opCode = firstByte & 0x03;
            var f3 = secondByte >> 5;

            Logger.Info("RVC = {0}, OpCode = {0:X}, F3 = {1:X}", BitConverter.ToString(rvcCoding.ToArray(), 0), opCode, f3);

            if (opCode==0x00)
            {
                payload = DecodeGroup00(rvcCoding, opCode, f3);
            }

            if (opCode == 0x01)
            {
                payload = DecodeGroup01(rvcCoding, opCode, f3);
            }

            if (opCode == 0x02)
            {
                payload = DecodeGroup10(rvcCoding, opCode, f3);
            }

            return payload;
        }


        /*
         *  The formats were designed to keep bits for the two register source specifiers in the same place in all
            instructions, while the destination register field can move. When the full 5-bit destination register
            specifier is present, it is in the same place as in the 32-bit RISC-V encoding. Where immediates
            are sign-extended, the sign-extension is always from bit 12. Immediate fields have been scrambled,
            as in the base specification, to reduce the number of immediate muxes required.

            For many RVC instructions, zero-valued immediates are disallowed and x0 is not a valid 5-bit
            register specifier. These restrictions free up encoding space for other instructions requiring fewer
            operand bits.
         * 
         * 
         *  Format  Meaning                      15 14 13 12 11 10 09 08 07 06 05 04 03 02 01 00
            CR      Register                     funct4      rd/rs1         rs2            op
            CI      Immediate                    funct3   i  rd/rs1         imm            op
            CSS     Stack-relative Store         funct3   i                 rs2            op
            CIW     Wide Immediate               funct3   i                       rd ′     op
            CL      Load                         funct3   i        rs1′     imm   rd ′     op
            CS      Store                        funct3   i        rs1′     imm   rs2′     op
            CA      Arithmetic                   funct6            rd′      f2    rs2 ′    op
                                                                   /rs1′ 
            CB      Branch                       funct3   offset   rs1 ′    offset         op
            CJ      Jump                         funct3   jump target                      op


            Table 16.1: Compressed 16-bit RVC instruction formats.
            RVC Register Number               000 001 010 011 100 101 110 111
            Integer Register Number           x8  x9  x10 x11 x12 x13 x14 x15
            Integer Register ABI Name         s0  s1  a0  a1  a2  a3  a4  a5
            Floating-Point Register Number    f8  f9  f10 f11 f12 f13 f14 f15
            Floating-Point Register ABI Name  fs0 fs1 fa0 fa1 fa2 fa3 fa4 fa5

            Table 16.2: Registers specified by the three-bit rs1 ′, rs2 ′, and rd ′ fields of the CIW, CL, CS, CA,
            and CB formats.
         * 
         */
        // ...........................

        #region Group decoder

        private RvcPayload DecodeGroup00(IEnumerable<byte> rvcCoding, int opcode, int f3)
        {
            var payload = new RvcPayload();

            //
            // C.LW = 010
            // C.LD = 011
            // C.FLW = 011
            // C.FLD = 001
            bool isClType = (f3 == 0x02) || (f3 == 0x03) || (f3 == 0x01);
            if (isClType)
            {
                payload = DecodeCL(rvcCoding);
            }

            //
            // C.SW = 110
            // C.SD = 111
            // C.SQ = 101
            // C.FSW = 111
            // C.FSD = 101
            bool isCsType = (f3 == 0x06) || (f3 == 0x07) || (f3 == 0x05);
            if (isCsType)
            {
                payload = DecodeCS(rvcCoding);
            }

            // C.ADDI4SPN 000
            bool isCiwType = (f3 == 0x00);
            if (isCiwType)
            {
                payload = DecodeCIW(rvcCoding);
            }

            return payload;
        }

        private RvcPayload DecodeGroup01(IEnumerable<byte> rvcCoding, int opcode, int f3)
        {
            var payload = new RvcPayload();

            //
            //  C.J = 101 
            //  C.JAL = 001 (only RV32I)
            bool isCjType = (f3 == 0x05) || (f3 == 0x01);
            if (isCjType)
            {
                throw new RiscVSimException("Not implemented yet!");

                payload = DecodeCJ(rvcCoding);
            }

            //
            // C.BEQZ
            // C.BNEZ
            var isCbType = (f3 == 0x06) || (f3 == 0x07);
            if (isCbType)
            {
                payload = DecodeCB_Branch(rvcCoding);
            }

            // CB Format:
            // C.SRLI 100
            // C.SRAI 100
            // C.ANDI 
            // 
            // CA Format:
            // C.AND / C.OR / C.XOR / C.SUB / C.ADDW / C.SUBW 
            var isCbTypeOrCaType = (f3 == 0x04);
            if (isCbTypeOrCaType)
            {
                throw new RiscVSimException("Not implemented yet!");

                var bit11to10 = rvcCoding.ElementAt(1) & 0xC0;
                if (bit11to10 == 0x03)
                {
                    // CA coding
                }
                else
                {
                    // CB coding
                }
            }

            //
            // C.LI 010
            // C.LUI 011
            // C.ADDI 000
            // C.ADDIW 001
            // C.ADDI16SP 011
            var isCiType = (f3 == 0x02) || (f3 == 0x03) || (f3 == 0x01) || (f3 == 0x00);
            if (isCiType)
            {
                payload = DecodeCI(rvcCoding);
            }

            return payload;
        }

        private RvcPayload DecodeGroup10(IEnumerable<byte> rvcCoding, int opcode, int f3)
        {
            var payload = new RvcPayload();

            //
            // Decode the CI Type opcodes
            //
            // C.SLLI 000
            // C.SLLI64 0000
            // C.LWSP = 010
            // C.LQSP = 001 
            // C.FLDSP = 001
            // C.FLWSP = 011
            // C.LDSP = 011
            bool isCiType = (f3 == 0x02) || (f3 == 0x01) || (f3 == 0x03) || (f3 == 0x00);
            if (isCiType)
            {
                payload = DecodeCI(rvcCoding);
            }

            // C.SWSP
            // C.SDSP
            // C.SQSP
            // C.FSWPSP
            // C.FSDSP
            bool isCssType = (f3 == 0x06) || (f3 == 0x07) || (f3 == 0x05);
            if (isCssType)
            {
                payload = DecodeCSS(rvcCoding);
            }

            // C.JR 100
            // C.JALR 
            // C.MV 100
            // C.ADD 100
            bool isCrType = f3 == 0x04;
            if (isCrType)
            {
                payload = DecodeCR(rvcCoding);
            }

            return payload;
        }

        #endregion

        #region Type decoder

        private RvcPayload DecodeCI(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            var immediate = 0;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            // Read the opcode
            var opCode = buffer & 0x3;

            // Read the Immediate coding Bit 2..6
            buffer >>= 2;
            immediate = buffer & 0x1F;

            // Read the rd register
            buffer >>= 5;
            var rd = buffer & 0x1F;

            // Read the imm. Bit 12
            buffer >>= 5;
            var imm12 = (buffer & 0x01) << 5;
            immediate |= imm12;

            // read F3
            buffer >>= 1;
            var f3 = buffer & 0x7;

            payload.LoadCI(opCode, immediate,rd,f3);
            return payload;
        }

        private RvcPayload DecodeCSS(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            var immediate = 0;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // Read RS2
            buffer >>= 2;
            var rs2 = buffer & 0x1F;

            // Immediates
            buffer >>= 5;
            immediate = buffer & 0x3F;

            // F3
            buffer >>= 6;
            var f3 = buffer & 0x7;

            payload.LoadCSS(opCode, rs2, immediate, f3);
            return payload;
        }

        private RvcPayload DecodeCL(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // Rd'
            buffer >>= 2;
            var rdc = buffer & 0x07;

            // Imm
            buffer >>= 3;
            immediate = buffer & 0x03;

            // Rs1'
            buffer >>= 2;
            var rs1c = buffer & 0x07;

            // Imme
            buffer >>= 3;
            var imm2 = buffer & 0x07;
            immediate = immediate | (imm2 >> 2);

            // f3
            buffer >>= 3;
            var f3 = buffer & 0x07;

            payload.LoadCL(opCode, rdc, immediate, rs1c, f3);
            return payload;
        }

        private RvcPayload DecodeCS(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // rs2'
            buffer >>= 2;
            var rs2c = opCode & 0x07;

            // imm
            buffer >>= 3;
            immediate = buffer & 0x03;

            // rs1'
            buffer >>= 3;
            var rs1c = buffer & 0x07;

            // immm 2
            buffer >>= 3;
            var imm2 = buffer & 0x07;
            immediate = immediate | (imm2 >> 2);

            // f3
            buffer >>= 3;
            var f3 = buffer & 0x07;

            payload.LoadCS(opCode, rs2c, immediate, rs1c, f3);
            return payload;
        }

        private RvcPayload DecodeCA(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();



            return payload;
        }

        private RvcPayload DecodeCJ(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // imm
            immediate = 0x3FF;

            // f3
            buffer >>= 11;
            var f3 = buffer & 0x07;

            payload.LoadCJ(opCode, immediate, f3);
            return payload;
        }

        private RvcPayload DecodeCR(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // rs2
            buffer >>= 2;
            var rs2 = buffer & 0x1F;

            // rs1
            buffer >>= 5;
            var rs1 = buffer & 0x1F;

            // funct 4
            buffer >>= 5;
            var f4 = buffer & 0x0F;

            payload.LoadCR(opCode, rs1, rs2, f4);
            return payload;
        }

        private RvcPayload DecodeCB_Branch(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // Imm 1
            buffer >>= 2;
            immediate = buffer & 0x1F;

            // Rs1'
            buffer >>= 5;
            var rs1c = buffer & 0x7;

            // Imm 2
            buffer >>= 3;
            var imm2 = buffer & 0x07;
            immediate |= (imm2 << 5);

            // f3
            buffer >>= 3;
            var f3 = buffer & 0x07;

            payload.LoadCB(opCode, immediate, rs1c, f3);
            return payload;
        }

        private RvcPayload DecodeCIW(IEnumerable<byte> rvcCoding)
        {
            var payload = new RvcPayload();
            int immediate;

            int buffer = rvcCoding.ElementAt(1);
            buffer <<= 8;
            buffer |= rvcCoding.First();

            var opCode = buffer & 0x03;

            // rd'
            buffer >>= 2;
            var rdc = buffer & 0x07;

            // immm
            buffer >>= 3;
            immediate = 0xFF;

            // f3
            buffer >>= 8;
            var f3 = buffer & 0x07;

            payload.LoadCIW(opCode, rdc, immediate, f3);
            return payload;
        }

        #endregion

    }
}
