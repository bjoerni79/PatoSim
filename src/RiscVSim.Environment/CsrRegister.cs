using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    internal class CsrRegister : ICsrRegister
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private byte[] register;

        internal CsrRegister()
        {
            // Create registerindex 0 .. 4095
            register = new byte[4096];
        }

        public byte Read(int registerIndex)
        {
            var value = register[registerIndex];
            Logger.Info("CSR : Reading from register {index} and return value {value:X} hex", registerIndex, value);

            return value;
        }

        public void Write(int registerIndex, byte value)
        {
            Logger.Info("CSR: Writing {value:X} hex to register {index}", value, registerIndex);
            register[registerIndex] = value;
        }
    }
}
