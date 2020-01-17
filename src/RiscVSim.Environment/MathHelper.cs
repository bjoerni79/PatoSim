using RiscVSim.Environment.Decoder;
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
            if (offset > 0)
            {
                result = baseAddress + Convert.ToUInt32(offset);
            }
            else
            {
                int positiveOffset = offset * -1;
                result = baseAddress - Convert.ToUInt32(positiveOffset);
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
        internal static int GetSignedInteger (uint coding, InstructionType type)
        {
            // We get a coding from an instruction and this one has a signed bit. 
            // 1.  Scan for it
            // 2. Remove it
            // 3. Convert to Int32 and Multiply the value with -1 



            uint scanBitMask = 1;
            int bitLength = GetBitLength(type);
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

        private static int GetBitLength(InstructionType type)
        {
            int length;
            switch (type)
            {
                case InstructionType.J_Type:
                    length = 20;  // 1 Bit already set...
                    break;
                case InstructionType.I_Type:
                    length = 11; // 1 Bit already set, so 12 - 1 = 11
                    break;
                case InstructionType.B_Type:
                    length = 12; //  +/- 4KByte plus Signed
                    break;
                case InstructionType.S_Type:
                    length = 11; // Similar to I.  12 Bit Signed,  +/- 2KBytes
                    break;
                default:
                    length = 12;
                    break;
            }

            return length;
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

        // ---

        internal enum LogicalOp
        {
            Add,
            Or,
            Xor,
            BitwiseInversion
        }

        internal static IEnumerable<byte> ExecuteLogicalOp(LogicalOp op, IEnumerable<byte> rs1, int immediate, Architecture architecture)
        {
            byte[] buffer;
            if (architecture == Architecture.Rv32I)
            {
                buffer = new byte[4];
            }
            else
            {
                buffer = new byte[8];
            }

            var immediateBytes = BitConverter.GetBytes(immediate);
            var immediateBuffer = new byte[buffer.Length];
            Array.Copy(immediateBytes, 0, immediateBuffer, 0, immediateBytes.Length);
            //for (int )

            for (int index = 0; index < buffer.Length; index++)
            {
                switch (op)
                {
                    case LogicalOp.Add:
                        buffer[index] = Convert.ToByte(rs1.ElementAt(index) & immediateBuffer.ElementAt(index));
                        break;
                    case LogicalOp.Or:
                        buffer[index] = Convert.ToByte(rs1.ElementAt(index) | immediateBuffer.ElementAt(index));
                        break;
                    case LogicalOp.Xor:
                        buffer[index] = Convert.ToByte(rs1.ElementAt(index) ^ immediateBuffer.ElementAt(index));
                        break;
                    case LogicalOp.BitwiseInversion:
                        var value = rs1.ElementAt(index);
                        var complement = ~value;
                        buffer[index] = Convert.ToByte(complement & 0xFF);
                        break;
                }
            }

            return buffer;
        }
    }
}
