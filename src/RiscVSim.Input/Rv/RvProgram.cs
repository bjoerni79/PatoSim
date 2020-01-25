using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Input.Rv
{
    public class RvProgram
    {
        private List<byte> opCodeList;

        private StringBuilder code;

        public RvProgram()
        {
            opCodeList = new List<byte>();
            code = new StringBuilder();
        }

        public void AddOpcode(IEnumerable<byte> opCode)
        {
            opCodeList.AddRange(opCode);
        }

        public void AddOpCodeLine (string opcode, string comment)
        {
            code.AppendFormat("- {0}\t# {1}\n", opcode, comment);
        }

        public string GetOpcodeLines()
        {
            return code.ToString();
        }

        public IEnumerable<byte> GetOpcodes()
        {
            return opCodeList;
        }
    }
}
