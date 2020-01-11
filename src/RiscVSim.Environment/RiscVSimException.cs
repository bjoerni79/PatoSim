using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public class RiscVSimException : ApplicationException
    {
        public RiscVSimException(string message) : base(message)
        {

        }

        public RiscVSimException(string message, Exception inner) : base (message,inner)
        {

        }
    }
}
