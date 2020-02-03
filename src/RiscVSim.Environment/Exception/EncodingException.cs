using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Exception
{
    public class EncodingException : RiscVSimException
    {
        public EncodingException(string message) : base(message)
        {

        }

        public EncodingException(string message, System.Exception inner) : base (message,inner)
        {

        }

    }
}
