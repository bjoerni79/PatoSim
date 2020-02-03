using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Exception
{
    public class ArchitectureNotSupportedException : RiscVSimException
    {
        public ArchitectureNotSupportedException(string message) : base(message)
        {

        }

        public ArchitectureNotSupportedException(string message, System.Exception inner) : base(message,inner)
        {

        }
    }
}
