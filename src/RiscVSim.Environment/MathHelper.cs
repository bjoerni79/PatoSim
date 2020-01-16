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
                // What about the signed bit???
                result = baseAddress + unsignedOffset;
            }
            else
            {
                result = baseAddress - unsignedOffset;

            }

            return result;
            return 0;
        }

        /// <summary>
        /// Helper method for dealing with 12 Bit Signed Integer values and converting them to .NET C# Int32 values
        /// </summary>
        /// <param name="coding">the uint coding (i.e a I-Type 12 Bit signed int)</param>
        /// <param name="bitLength">the bit length (12 for an I-Type)</param>
        /// <returns>a Int32 .NET representation of the coding</returns>
        internal static int GetSignedInteger (uint coding, int bitLength)
        {
            // We get a coding from an instruction and this one has a signed bit. 
            // 1.  Scan for it
            // 2. Remove it
            // 3. Convert to Int32 and Multiply the value with -1 

            uint scanBitMask = 1;
            scanBitMask <<= bitLength;

            uint filter = ~scanBitMask;

            int result;
            if ((coding & scanBitMask) == scanBitMask)
            {
                // Signed bit detected!
                uint preparedCoding = coding & filter;
                result = Convert.ToInt32(preparedCoding);

                // Signed bit removed and now multiply it with -1
                result *= -1;
            }
            else
            {
                result = Convert.ToInt32(coding);
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
