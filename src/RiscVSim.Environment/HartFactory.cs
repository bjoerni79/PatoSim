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
            IHart hart = null;

            switch (configuration.Architecture)
            {
                case Architecture.Rv32E:
                    hart = new HartCore32(configuration.Architecture);
                    break;

                case Architecture.Rv32I:
                    hart = new HartCore32(configuration.Architecture);
                    break;

                case Architecture.Rv64I:
                    hart = new HartCore64();
                    break;

                default:
                    throw new RiscVSimException("Unsupported architecture detected : " + configuration.Architecture);


            }

            return hart;
        }
    }
}
