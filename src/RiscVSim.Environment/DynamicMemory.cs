using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment
{
    internal class DynamicMemory : IMemory
    {
        private Architecture architecture;

        private Dictionary<ulong, byte> memoryDict;

        internal DynamicMemory(Architecture architecture)
        {
            memoryDict = new Dictionary<ulong, byte>();
            this.architecture = architecture;
        }

        #region IMemory implementation

        public Architecture Architecture
        {
            get
            {
                return architecture;
            }
        }


        public IEnumerable<byte> GetByte(ulong address)
        {
            var singleByte = ReadBlock(address, 1);
            return singleByte;
        }

        public IEnumerable<byte> GetHalfWord(ulong address)
        {
            var word = ReadBlock(address, 2);
            return word;
        }

        public IEnumerable<byte> GetWord(ulong address)
        {
            var word = ReadBlock(address, 4);
            return word;
        }

        public IEnumerable<byte> GetDoubleWord(ulong address)
        {
            var doubleWord = ReadBlock(address, 8);
            return doubleWord;
        }

        public IEnumerable<byte> Read(ulong baseAddress, int count)
        {
            var buffer = ReadBlock(baseAddress, count);
            return buffer;
        }

        public void Write(ulong baseAddress, IEnumerable<byte> content)
        {
            WriteBlock(baseAddress, content);
        }

        #endregion


        #region Internal implementation

        private IEnumerable<byte> ReadBlock(ulong baseAddress, int count)
        {
            //TODO:  Consider a review...
            var buffer = new byte[count];
            var toULong = Convert.ToUInt64(count);
            for (ulong offset = 0; offset < toULong; offset++)
            {
                ulong curAddress = baseAddress + offset;
                buffer[offset] = ReadByte(curAddress);
            }

            return buffer;
        }

        private void WriteBlock (ulong baseAddress, IEnumerable<byte> content)
        {
            //TODO:  Consider a review...
            int length = content.Count();
            var toUlong = Convert.ToUInt64(length);
            for (ulong offset = 0; offset < toUlong; offset++)
            {
                var curAddress = baseAddress + offset;

                var toInt = Convert.ToInt32(offset);
                WriteByte(curAddress, content.ElementAt(toInt));
            }
        }

        private void WriteByte(ulong address, byte value)
        {
            // If the the key (the address) already exists, update the value or create a new one otherwise.

            if (memoryDict.ContainsKey(address))
            {
                memoryDict[address] = value;
            }
            else
            {
                memoryDict.Add(address, value);
            }
        }

        private byte ReadByte(ulong address)
        {
            // If the memory is "blank" create a new one with zero as default value
            if (!memoryDict.ContainsKey(address))
            {
                memoryDict.Add(address, 0);
            }

            return memoryDict[address];
        }




        #endregion
    }
}
