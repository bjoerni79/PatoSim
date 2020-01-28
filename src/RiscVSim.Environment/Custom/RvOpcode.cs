using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Custom
{
    public class RvOpcode : OpCodeCommand
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IHartEnvironment environment;

        public RvOpcode(IMemory memory, IRegister register, IHartEnvironment environment) : base(memory,register)
        {
            this.environment = environment;
        }

        public override int Opcode => 0x00;



        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            // Forward the request to the EEI 
            environment.RvConsoleAction(payload);
            return true;
        }
    }
}
