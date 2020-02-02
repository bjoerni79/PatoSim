using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class RvcDecoder
    {
        private Architecture architecture;

        public RvcDecoder(Architecture architecture)
        {
            this.architecture = architecture;
        }

        public RvcPayload Decode (IEnumerable<byte> rvcCoding)
        {
            RvcPayload payload = null;

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
    }
}
