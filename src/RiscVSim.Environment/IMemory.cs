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
        IEnumerable<byte> GetHalfWord(uint address);

        /// <summary>
        /// Gets a word (= 32 Bit, 4 Byte = Instruction) from memory
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IEnumerable<byte> GetWord(uint address);

        IEnumerable<byte> GetByte(uint address);

        IEnumerable<byte> GetDoubleWord(uint address);

        void Write(uint baseAddress, IEnumerable<byte> content);

        IEnumerable<byte> Read(uint baseAddress, int count);

    }
}
