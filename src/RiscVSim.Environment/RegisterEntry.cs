using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment
{
    public class RegisterEntry
    {
        private byte[] content;
        private int registerSize;
        private Architecture architecture;

        public RegisterEntry(Architecture architecture)
        {
            if (architecture == Architecture.Unknown)
            {
                throw new ArgumentException("architecture");
            }

            this.architecture = architecture;
            registerSize = (int)architecture;

            Clear();
        }

        public void WriteInteger(int value)
        {
            // Store it in a RISC - V compatible way
            var bytes = BitConverter.GetBytes(value);
            //var coding = CutTrailingZero(bytes);

            WriteBlock(bytes);
        }

        public void WriteUnsignedInteger(uint value)
        {
            // Store it in a RISC - V compatible way
            var bytes = BitConverter.GetBytes(value);
            //var coding = CutTrailingZero(bytes);

            WriteBlock(bytes);
        }

        public void WriteBlock(IEnumerable<byte> blockValue)
        {
            // Store it in a RISC - V compatible way
            if (blockValue == null)
            {
                throw new ArgumentNullException("blockValue");
            }

            var count = blockValue.Count();
            if (count <= registerSize)
            {
                Clear();

                for (int i = 0; i<count; i++)
                {
                    content[i] = blockValue.ElementAt(i);
                }
            }
        }

        private void Clear()
        {
            content = new byte[registerSize];
        }

        /// <summary>
        /// TODO: This works for now , but must be improved later!!!
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEnumerable<byte> CutTrailingZero(IEnumerable<byte> content)
        {
            var result = new List<byte>();
            var reversed = content; //.Reverse();
            var keepZeroes = false;

            foreach (var curByte in reversed)
            {
                if (curByte != 0)
                {
                    result.Add(curByte);
                    keepZeroes = true;
                }
                else
                {
                    // Zero detected)
                    if (keepZeroes)
                    {
                        result.Add(00);
                    }
                }
            }

            return result;
        }


        public IEnumerable<byte> ReadBlock()
        {
            // Create a new instance of the register content and return it.
            var copyOfContent = new List<byte>(content);
            return copyOfContent;
        }

        public int ReadInteger()
        {
            var value = BitConverter.ToInt32(content, 0);
            return value;
        }

        public uint ReadUnsignedInteger()
        {
            var value = BitConverter.ToUInt32(content, 0);

            return value;
        }

    }
}
