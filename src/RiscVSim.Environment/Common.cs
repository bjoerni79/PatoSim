using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    internal class Common
    {
        private string[] registerNames32 = new string[]
            {
                "x0","ra","sp","gp","tp","t0","t1","t2",
                "s0","s1", "a0","a1","a2","a3","a4","a5",
                "a6","a7","s2","s3","s4","s5","s6","s7",
                "s8","s9","s10","s11","t3","t4","t5","t6",
                "pc"
            };

        internal string DecocdeRegisterIndex(int index)
        {
            return registerNames32[index];
        }
    }
}
