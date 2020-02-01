using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    public class HartConfiguration
    {
        public HartConfiguration()
        {
            Debug = DebugMode.Disabled;
            Memory = MemoryConfiguration.Dynamic;
            Architecture = Architecture.Rv64I;
            Source = String.Empty;
            RvMode = false;
        }

        public DebugMode Debug { get;  set; }

        public bool RvMode { get; set; }

        public bool VerboseMode { get; set; }

        public int RvLoadOffset { get; set; }

        public MemoryConfiguration Memory { get;  set; }

        public Architecture Architecture { get;  set; }

        public string Source { get; set; }
    }
}
