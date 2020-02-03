using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Exception
{
    public class RvcFormatException : RiscVSimException
    {
        public RvcFormatException(string message) : base(message)
        {

        }

        public RvcFormatException(string message, System.Exception e) : base(message, e)
        {

        }
    }
}
