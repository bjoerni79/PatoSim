using System;
using System.Collections.Generic;
using System.Linq;
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

        internal static byte[] Prepare(byte[] buffer, int bytesRequired, bool isSigned)
        {
            var bufferLength = buffer.Length;
            // Take care of the signed bit. Save the information and remove it
            bool signBitDetected = false;
            if (isSigned)
            {
                var last = buffer.Last();
                if ((last & 0x80) == 0x80)
                {
                    // Save this info and remove the signed bit.
                    buffer[buffer.Length - 1] &= 0x7F;
                    signBitDetected = true;
                }
            }

            // Fill up to 4 bytes
            var diff = bytesRequired - bufferLength;
            List<byte> preparedBuffer = new List<byte>();
            preparedBuffer.AddRange(buffer);

            for (int zeroIndex = 0; zeroIndex < diff; zeroIndex++)
            {
                preparedBuffer.Add(0x00);
            }

            // Add the signed bit at the last position
            if (isSigned && signBitDetected)
            {
                preparedBuffer[3] |= 0x80;
            }

            return preparedBuffer.ToArray();
        }
    }
}
