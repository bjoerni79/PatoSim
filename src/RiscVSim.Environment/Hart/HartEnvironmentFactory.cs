using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    /// <summary>
    /// !!Workaround!!  The goal is that one or more harts can share the environment. For now it's OK.
    /// </summary>
    public static class HartEnvironmentFactory
    {
        public static IHartEnvironment Build (Architecture architecture, IRegister register, IMemory memory, ICsrRegister csrRegister)
        {
            return new HartEnvironment(architecture, register, memory, csrRegister);
        }
    }
}
