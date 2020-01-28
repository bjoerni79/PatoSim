using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartEnvironment : IHartEnvironment
    {
        internal HartEnvironment()
        {

        }

        public int NopCounter { get; private set; }

        public void IncreaseNopCounter()
        {
            NopCounter++;
        }
    }
}
