using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Extensions
{
    internal class Divider
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Architecture architecture;
        private IRegister register;

        internal Divider (Architecture architecture, IRegister register)
        {
            this.architecture = architecture;
            this.register = register;
        }


    }
}
