using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Input
{
    public class ParserException : ApplicationException
    {
        public ParserException(string message) : base(message)
        {

        }

        public ParserException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
