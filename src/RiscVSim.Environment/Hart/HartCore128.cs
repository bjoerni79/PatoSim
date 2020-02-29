using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartCore128 : HartBase
    {
        internal HartCore128() : base(Architecture.Rv128I)
        {

        }


        public override void Load(ulong address, IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        protected override void BootCpu()
        {
            throw new NotImplementedException();
        }

        protected override void ExecuteOpcode(Instruction instruction, InstructionPayload payload)
        {
            throw new NotImplementedException();
        }

        protected override void ExecuteRvcOpcode(RvcPayload payload)
        {
            throw new NotImplementedException();
        }

        protected override void InitDetails(ulong programCounter)
        {
            throw new NotImplementedException();
        }
    }
}
