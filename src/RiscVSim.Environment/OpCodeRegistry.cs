using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    internal class OpCodeRegistry
    {
        private Dictionary<int, OpCodeCommand> opCodeDict;

        internal OpCodeRegistry()
        {
            opCodeDict = new Dictionary<int, OpCodeCommand>();
        }

        public void Add(int opCode, OpCodeCommand command)
        {
            if (opCodeDict.ContainsKey(opCode))
            {
                throw new OpCodeNotSupportedException(String.Format("OpCode {0:X2} already exist", opCode));
            }

            opCodeDict.Add(opCode, command);
        }

        public OpCodeCommand Get(int opCode)
        {
            if (!opCodeDict.ContainsKey(opCode))
            {
                throw new OpCodeNotSupportedException(String.Format("OpCode {0:X2} is not supported", opCode));
            }

            return opCodeDict[opCode];
        }

        public bool IsInitialized
        {
            get
            {
                return opCodeDict.Keys.Count > 0;
            }
        }
    }
}
