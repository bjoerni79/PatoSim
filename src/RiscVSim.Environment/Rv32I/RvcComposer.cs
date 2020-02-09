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
