using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    internal class Cpu64 : ICpu
    {
        internal Cpu64()
        {

        }

        public void AssignHint(Hint hint)
        {

        }

        public void AssignMemory(IMemory memory)
        {

        }

        public void AssignRegister(IRegister register)
        {

        }

        public void Execute(Instruction instruction, InstructionPayload payload)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            // Init the OpCodes here!
        }
    }
}
