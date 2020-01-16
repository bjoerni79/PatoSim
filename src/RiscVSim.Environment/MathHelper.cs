using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    internal static class MathHelper
    {
        internal static uint Add(uint baseAddress, int offset)
        {
            uint result;
            uint unsignedOffset = Convert.ToUInt32(offset);
            if (offset > 0)
            {
                result = baseAddress + unsignedOffset;
            }
            else
            {
                result = baseAddress - unsignedOffset;

            }

            return result;
        }
    }
}
