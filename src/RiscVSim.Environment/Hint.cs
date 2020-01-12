using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Class that manages the HINT instructions...  how and what to do is still a TODO
    /// </summary>
    public sealed class Hint
    {
        internal Hint()
        {

        }

        internal void IncreaseNopCounter ()
        {
            NopCounter++;
        }

        public int NopCounter { get; private set; }
    }
}
