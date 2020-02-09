using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IRvcComposer
    {
        Instruction ComposeInstruction(RvcPayload payload);

        InstructionPayload ComposePayload(Instruction instruction, RvcPayload payload);
    }
}
