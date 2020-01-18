using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// TODO: Implements a fixed byte array memory! (faster but requires more RAM..)
    /// </summary>
    internal class FixedMemory : IMemory
    {
        private byte[] memoryArray;

        internal FixedMemory(Architecture architecture, long memoryLength)
        {
            Architecture = architecture;

            if (memoryLength <= 0)
            {
                throw new RiscVSimException("Please assign a memory value hight than 0");
            }

            var offset = memoryLength % 2;
            if (offset != 0)
            {
                throw new RiscVSimException("Please provide a memory length based on 2");
            }

            memoryArray = new byte[memoryLength];
        }

        public Architecture Architecture { get; private set; }

        public IEnumerable<byte> GetByte(uint address)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> GetWord(uint address)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> GetHalfWord(uint address)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> Old_Fetch(uint address)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> Read(uint baseAddress, int count)
        {
            throw new NotImplementedException();
        }

        public void Write(uint baseAddress, IEnumerable<byte> content)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> GetDoubleWord(uint address)
        {
            throw new NotImplementedException();
        }
    }
}
