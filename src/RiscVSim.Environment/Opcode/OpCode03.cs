using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    public class OpCode03 : OpCodeCommand32
    {
        public OpCode03(IMemory memory, IRegister register) : base(memory,register)
        {
            // base()
        }

        public override int Opcode => 0x03;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            /*
             *   Bit 31 .. 28 | 27 26 25 24 23 22 21 20
             *   
             *   28..31 = FM
             *   
             *   Successor
             *   20 = SW,
             *   21 = SR
             *   22 = SO
             *   23 = SI
             *   
             *   Predecessor
             *   24 = PW
             *   25 = PR
             *   26 = PO
             *   27 = PI
             */

            // No idea what to do right now..
            return true;
        }
    }
}
