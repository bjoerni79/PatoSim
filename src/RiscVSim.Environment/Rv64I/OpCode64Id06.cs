using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id06 : OpCodeCommand
    {
        public OpCode64Id06(IMemory memory, IRegister register) : base(memory,register)
        {
            // base
        }

        public override int Opcode => 0x06;

        /*
         *  addiw   rd rs1 imm12            14..12=0 6..2=0x06 1..0=3
            slliw   rd rs1 31..25=0  shamtw 14..12=1 6..2=0x06 1..0=3
            srliw   rd rs1 31..25=0  shamtw 14..12=5 6..2=0x06 1..0=3
            sraiw   rd rs1 31..25=32 shamtw 14..12=5 6..2=0x06 1..0=3
         * 
         */

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {


            return true;
        }
    }
}
