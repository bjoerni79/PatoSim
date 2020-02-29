using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Specifies the methods for accessing different integer types from a register instance
    /// </summary>
    public interface IRegister
    {
        /// <summary>
        /// Gets the program counter index
        /// </summary>
        /// <remarks>Typical this is the index 32, except for RV32E.</remarks>
        int ProgramCounter  { get;  }


        /// <summary>
        /// Writes a signed int value to a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="value">the integer value</param>
        void WriteSignedInt(int index, int value);

        /// <summary>
        /// Reads a signed int value from a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the value of the register</returns>
        int ReadSignedInt(int index);

        /// <summary>
        /// Reads an unsigned int from a register name
        /// </summary>
        /// <param name="name">the register name</param>
        /// <returns>the value from the register</returns>
        uint ReadUnsignedInt(RegisterName name);

        /// <summary>
        /// Reads an unsigned int from a index
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the value of the register</returns>
        uint ReadUnsignedInt(int index);

        /// <summary>
        /// Writes an unsigned integer to a register
        /// </summary>
        /// <param name="name">the register name</param>
        /// <param name="value">the value</param>
        void WriteUnsignedInt(RegisterName name, uint value);

        /// <summary>
        /// Writes an unsigned integer to an index
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="value">the value</param>
        void WriteUnsignedInt(int index, uint value);

        /// <summary>
        /// Reads a signed long from a register
        /// </summary>
        /// <param name="name">the register name</param>
        /// <returns>the value</returns>
        long ReadSignedLong(RegisterName name);

        /// <summary>
        /// Reads a signed long from a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the value</returns>
        long ReadSignedLong(int index);

        /// <summary>
        /// Writes a signed long to a register
        /// </summary>
        /// <param name="registerName">the name of the register</param>
        /// <param name="value">the value</param>
        void WriteSignedLong(RegisterName registerName, long value);

        /// <summary>
        /// Writes a signed long to a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="value">the value</param>
        void WriteSignedLong(int index, long value);

        /// <summary>
        /// Reads an unsigned long from a register
        /// </summary>
        /// <param name="name">the register name</param>
        /// <returns>the value</returns>
        ulong ReadUnsignedLong(RegisterName name);

        /// <summary>
        /// Reads an unsigned long from a register
        /// </summary>
        /// <param name="name">tne name</param>
        /// <returns>the value</returns>
        ulong ReadUnsignedLong(int name);

        /// <summary>
        /// Writes an unsigned long to a register
        /// </summary>
        /// <param name="name">the register name</param>
        /// <param name="value">the value</param>
        void WriteUnsignedLong(RegisterName name, ulong value);

        /// <summary>
        /// Writes an unsigned long to a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="value">the value</param>
        void WriteUnsignedLong(int index, ulong value);

        /// <summary>
        /// Increases the program counter. Valid values are 2 for RVC and 4 for INST32.
        /// </summary>
        /// <param name="increment">the increment</param>
        void NextInstruction(int increment);

        /// <summary>
        /// Reads the register content as byte blocks
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>returns the bytes. The length depend on the architcture.</returns>
        IEnumerable<byte> ReadBlock(int index);

        /// <summary>
        /// Writes a big integer value to a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="bigInteger">the value</param>
        void WriteBigInteger(int index, BigInteger bigInteger);

        /// <summary>
        /// Writes a big integer value to a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="bigInteger">the value</param>
        void WriteBigInteger(RegisterName name, BigInteger bigInteger);

        /// <summary>
        /// Reads a big integer value from a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the big integer value</returns>
        BigInteger ReadBigInteger(int index);

        /// <summary>
        /// Reads a big integer value from a register
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the big integer value</returns>
        BigInteger ReadBigInteger(RegisterName name);

        /// <summary>
        /// Writes a byte collection to the register
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="block">the block</param>
        /// <remarks>
        /// The length can be less then the architecture side and gets adjusted.
        /// </remarks>
        void WriteBlock(int index, IEnumerable<byte> block);
    }
}
