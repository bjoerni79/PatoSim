﻿using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment
{
    public static class MathHelper
    {
        public static uint Add(uint baseAddress, int offset)
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
        }

        public static ulong Add(ulong baseAddress, int offset)
        {
            ulong result;
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
        }

        public static int GetSignedInteger(int coding,int bitLength)
        {
            // We get a coding from an instruction and this one has a signed bit. 
            // 1.  Scan for it
            // 2. Remove it
            // 3. Sign extend to FF and let the Bitconverter do the work

            int scanBitMask = 1;
            scanBitMask <<= bitLength;

            int result;
            if ((coding & scanBitMask) == scanBitMask)
            {
                //int bitmask = ~scanBitMask;
                //int posNumber = coding & bitmask;
                //int negNumber = posNumber * -1;

                //return negNumber;
                uint pattern = 0xFFFFFFFF;
                pattern <<= bitLength;

                uint unsignedCoding = Convert.ToUInt32(coding) | pattern;
                result = unchecked((int)unsignedCoding);

            }
            else
            {
                result = coding;
            }

            return result;
        }

        /// <summary>
        /// Helper method for dealing with 12 Bit Signed Integer values and converting them to .NET C# Int32 values
        /// </summary>
        /// <param name="coding">the uint coding (i.e a I-Type 12 Bit signed int)</param>
        /// <param name="bitLength">the bit length (12 for an I-Type)</param>
        /// <returns>a Int32 .NET representation of the coding</returns>
        public static int GetSignedInteger (int coding, InstructionType type)
        {
            var bitLength = GetBitLength(type);
            return GetSignedInteger(coding, bitLength);
        }

        public static int GetBitLength(InstructionType type)
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
                    length = 12; //  +/- 4KByte plus Signed , 13 with zero 0 index = 12
                    break;
                case InstructionType.S_Type:
                    length = 11; // Similar to I.  12 Bit Signed,  +/- 2KBytes
                    break;
                default:
                    throw new RiscVSimException("Could not decode immediate!");
            }

            return length;
        }

        public static byte[] PrepareLoad(byte[] buffer, int bytesRequired, bool useSignExtension)
        {
            //TODO:  PrepareLoad and SignExtension are very similar.  Consider a refactoring and use only one method.

            bool signBitDetected = false;
            var last = buffer.Last();

            if ((last & 0x80) == 0x80)
            {
                signBitDetected = true;
            }

            // Create a new buffer and set the default value to 0xFF
            var newBuffer = new byte[bytesRequired];
            if (useSignExtension && signBitDetected)
            {
                for (int i=0; i<bytesRequired;i++)
                {
                    newBuffer[i] = 0xFF;
                }
            }

            for (int i=0; i< buffer.Length; i++)
            {
                newBuffer[i] = buffer[i];
            }

            return newBuffer;
        }

        public static byte[] SignExtensionToLong(uint value)
        {
            // https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/sign-extension

            bool signedBitDetected = false;

            // Step one : Detect the signed bit and remove it
            uint signedBitFilter = 0x80000000;
            if ((value & signedBitFilter) == signedBitFilter)
            {
                // Signed bit detected
                signedBitDetected = true;
            }

            var result = new byte[8];
            var valueBytes = BitConverter.GetBytes(value);

            // If signed bit is detected, set all content to FF first.
            if (signedBitDetected)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0xFF;
                }
            }

            for (int i=0; i<valueBytes.Length; i++)
            {
                result[i] = valueBytes[i];
            }


            return result;
        }

        // ---

        public enum LogicalOp
        {
            Add,
            Or,
            Xor,
            BitwiseInversion
        }

        public static IEnumerable<byte> ExecuteLogicalOp(LogicalOp op, IEnumerable<byte> rs1, int immediate, Architecture architecture)
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
