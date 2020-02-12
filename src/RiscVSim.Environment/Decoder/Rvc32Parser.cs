using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class Rvc32Parser
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Rvc32Parser()
        {

        }

        public void ReadLwSp(RvcPayload rvcPayload, InstructionPayload instructionPayload)
        {
            Logger.Info("Parsing C.LWSP");


        }
    }
}
