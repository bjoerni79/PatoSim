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
        }

        public DebugMode Debug { get; private set; }

        public MemoryConfiguration Memory { get; private set; }

        public Architecture Architecture { get; private set; }

        public string Source { get; set; }
    }
}
