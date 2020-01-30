using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment.Extensions
{
    internal static class ExtensionHelper
    {
        internal static void WriteToRegister(int rd, BigInteger result, int bufferSize, IRegister register)
        {
            // Write the lower part (4 Bytes for RV32I and 8 Bytes for RV64I) to the register
            var byteCount = result.GetByteCount();
            var bytes = result.ToByteArray();
            var registerResult = new byte[bufferSize];

            // Read at max. the buffer size
            int bytesToRead = byteCount;
            if (byteCount > bufferSize)
            {
                bytesToRead = bufferSize;
            }

            for (int index = 0; index < bytesToRead; index++)
            {
                registerResult[index] = bytes[index];
            }

            register.WriteBlock(rd, registerResult);
        }
    }
}
