using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    public interface ICpu
    {
        void Execute(Instruction instruction, InstructionPayload payload);

        void AssignMemory(IMemory memory);

        void AssignRegister(Register register);

        void Init();
    }
}
