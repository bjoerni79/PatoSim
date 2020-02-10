using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class RvcComposer : IRvcComposer
    {
        public RvcComposer()
        {

        }
        public Instruction ComposeInstruction(RvcPayload payload)
        {
            //Instruction ins = new Instruction()

            //return ins;

            // Q00
            // 000 C.ADDI4SPN
            // 001 C.FLD
            // 010 C.LW
            // 011 C.FLW
            // 101 C.FSD
            // 110 C.SW
            // 111 C.FSW
            //

            // Q01
            // 000 C.NOP / C.ADDI
            // 001 C.JAL
            // 010 C.LI
            // 011 C.ADDI16SP (RD=2)
            // 011 C.LUI (RD != 2)
            // 100 x 00 C.SRLI
            // 100 x 01 C.SRAI
            // 100 x 10 C.ANDI
            // 100 x 11 C.SUB, C.XOR, C.OR, C.AND
            // 101 C.J
            // 110 C.BEQZ
            // 111 C.BNEZ

            // Q02
            // 000 C.SLLI
            // 001 C.FLDSP
            // 010 C.LWSP
            // 011 C.FLWSP
            // 100 0 C.JR / C.MV
            // 100 1 C.EBREAK / C.JALR / C.ADD
            // 101 C.FSDSP
            // 110 C.SWSP
            // 111 C.FSWSP



            return null;
        }

        public InstructionPayload ComposePayload(Instruction ins, RvcPayload payload)
        {
            //InstructionPayload p = new InstructionPayload(ins, null);

            //return p;

            return null;
        }
    }
}
