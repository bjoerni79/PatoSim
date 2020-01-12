using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IMemory
    {
        Architecture Architecture { get; }



        /// <summary>
        /// Fetches 4 Bytes ( = 32 Bit = 4 Byte Instruction).  Old Implementation and kept for backwards testing in the Unit Tests.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IEnumerable<byte> FetchInstruction(uint address);

        void Write(uint baseAddress, IEnumerable<byte> content);

        IEnumerable<byte> Read(uint baseAddress, int count);


    }
}
