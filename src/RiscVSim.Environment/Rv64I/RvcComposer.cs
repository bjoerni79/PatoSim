using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class RvcComposer : IRvcComposer
    {
        public Instruction ComposeInstruction(RvcPayload payload)
        {
            throw new NotImplementedException();
        }

        public InstructionPayload ComposePayload(Instruction instruction, RvcPayload payload)
        {
            throw new NotImplementedException();
        }
    }
}
