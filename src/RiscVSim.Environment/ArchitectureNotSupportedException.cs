using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public class ArchitectureNotSupportedException : RiscVSimException
    {
        public ArchitectureNotSupportedException(string message) : base(message)
        {

        }

        public ArchitectureNotSupportedException(string message, Exception inner) : base(message,inner)
        {

        }
    }
}
