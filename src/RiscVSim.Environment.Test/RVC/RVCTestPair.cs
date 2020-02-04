using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcTestPair
    {
        public RvcTestPair()
        {

        }

        public IEnumerable<byte> Coding { get; set; }

        public RvcPayload ExpectedPayload { get; set; }
    }
}
