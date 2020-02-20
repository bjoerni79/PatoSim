using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class RvcComposer64 : IRvcComposer
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Rvc32Parser parser32;
        private Rvc64Parser parser64;

        private const int System = 0x1C;
        private const int Load = 0x00;
        private const int Immediate = 0x04;
        private const int Store = 0x08;
        private const int CondBrach = 0x018;
        private const int JumpAndLink = 0x1B;
        private const int JumpAndLinkRegister = 0x19;
        private const int Register = 0x0C;
        private const int Lui = 0x0D;

        private const int ADDI4SPN = 0;
        private const int CLWSP = 2;
        private const int CLW = 2;
        private const int CSWSP = 6;
        private const int CSW = 6;
        private const int CSLLI = 0;
        private const int CJAL = 1;
        private const int CJ = 5;
        private const int BEQZ = 6;
        private const int BNEZ = 7;

        public RvcComposer64()
        {
            parser32 = new Rvc32Parser();
            parser64 = new Rvc64Parser();
        }

        public Instruction ComposeInstruction(RvcPayload payload)
        {
            Logger.Debug("Compose Instruction for payload : Op = {op:X}, F3 = {f3:X}", payload.Op, payload.Funct3);
            int? opCode = null;





            if (!opCode.HasValue)
            {
                throw new RiscVSimException("Invalid RVC Opcode detected");
            }


            var instruction = new Instruction(payload.Type, opCode.Value, 2);
            return instruction;
        }

        public InstructionPayload Compose(Instruction instruction, RvcPayload payload)
        {
            throw new NotImplementedException();
        }
    }
}
