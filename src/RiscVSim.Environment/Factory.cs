using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Factory methods for the Hart components 
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Creates a new Memory 
        /// </summary>
        /// <param name="architecture"></param>
        /// <returns></returns>
        public static IMemory CreateDynamicMemory(Architecture architecture)
        {
            var memory = new DynamicMemory(architecture);
            return memory;
        }

        public static IRegister CreateRegisterRv32()
        {
            return new Rv32I.Register32();
        }

        public static IRegister CreateRegisterRv64()
        {
            return new Rv64I.Register64();
        }

        public static Rv32I.Core CreateRv32ICore()
        {
            return new Rv32I.Core();
        }
    }
}
