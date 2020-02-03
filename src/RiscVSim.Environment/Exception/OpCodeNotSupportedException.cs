using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Exception
{
    public class OpCodeNotSupportedException : RiscVSimException    
    {

        public OpCodeNotSupportedException(string message) : base(message)
        {

        }

        public OpCodeNotSupportedException(string message, System.Exception inner) : base(message,inner)
        {

        }

        public InstructionType Type
        {
            get;set;
        }

        public int OpCode
        {
            get; set;
        }

        public int RegisterDestination
        {
            get; set;
        }

        public IEnumerable<byte> Coding
        {
            get; set;
        }
    }
}
