using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IMemory
    {
        Architecture Architecture { get; }

        /// <summary>
        /// Gets a half word (= 16 Bit, 2 Byte) from the memory
        /// </summary>
        /// <param name="address">the address</param>
        /// <returns>An IEnumerable with the bytes</returns>
        IEnumerable<byte> GetHalfWord(ulong address);

        /// <summary>
        /// Gets a word (= 32 Bit, 4 Byte = Instruction) from memory
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IEnumerable<byte> GetWord(ulong address);

        IEnumerable<byte> GetByte(ulong address);

        IEnumerable<byte> GetDoubleWord(ulong address);

        void Write(ulong baseAddress, IEnumerable<byte> content);

        IEnumerable<byte> Read(ulong baseAddress, int count);

    }
}
