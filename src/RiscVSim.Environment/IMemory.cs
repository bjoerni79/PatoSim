using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IMemory
    {
        Architecture Architecture { get; }

        IEnumerable<byte> FetchInstruction(uint address);

        void Write(uint baseAddress, IEnumerable<byte> content);

        IEnumerable<byte> Read(uint baseAddress, int count);


    }
}
