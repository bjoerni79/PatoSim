using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public static class HartFactory
    {
        public static IHart CreateHart(HartConfiguration configuration)
        {
            var hart = new HartVersion1();
            return hart;

        }
    }
}
