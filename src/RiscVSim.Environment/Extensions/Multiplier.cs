using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment.Extensions
{
    /// <summary>
    /// Implements the Multiplier Part of the RISC-V M Extension
    /// </summary>
    internal class Multiplier
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Architecture architecture;
        private IRegister register;
        private int boundary;
        private int defaultBufferSize;

        internal Multiplier(Architecture architecture, IRegister register)
        {
            this.architecture = architecture;
            this.register = register;

            if (architecture == Architecture.Rv64I)
            {
                boundary = 64;
                defaultBufferSize = 8;
            }
            else
            {
                boundary = 32;
                defaultBufferSize = 4;
            }
        }

        /// <summary>
        /// Implements the mul operation
        /// </summary>
        /// <param name="rd">the target register</param>
        /// <param name="rs1Coding">the little endian coding of the rs1</param>
        /// <param name="rs2Coding">the little endian coding of the rs2</param>
        internal void ExecuteMulw(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            // Compute the result using the BigInteger type
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = rs1 * rs2;

            // Use the lower 4 Bytes only
            WriteToRegister(rd, result, 4);
        }

        /// <summary>
        /// Implements the mul operation
        /// </summary>
        /// <param name="rd">the target register</param>
        /// <param name="rs1Coding">the little endian coding of the rs1</param>
        /// <param name="rs2Coding">the little endian coding of the rs2</param>
        internal void ExecuteMul(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            // Compute the result using the BigInteger type
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = rs1 * rs2;

            WriteToRegister(rd, result, defaultBufferSize);
        }

        /// <summary>
        /// Implements the Mulh implementation 
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="rs1Coding"></param>
        /// <param name="rs2Coding"></param>
        internal void ExecuteMulh(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            // Compute the result using the BigInteger type
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = rs1 * rs2;
            // Shift the lower part to the right
            result >>= boundary;

            WriteToRegister(rd, result, defaultBufferSize);
        }



        private void WriteToRegister(int rd, BigInteger result, int bufferSize)
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
