using RiscVSim.Environment.Exception;
using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public static class HartFactory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static IHart CreateHart(HartConfiguration configuration)
        {
            IHart hart;

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
                    Logger.Error("Unsupported architecture detected!  {arch}", configuration.Architecture);
                    throw new RiscVSimException("Unsupported architecture detected : " + configuration.Architecture);


            }

            return hart;
        }
    }
}
