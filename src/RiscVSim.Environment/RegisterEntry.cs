using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment
{
    public class RegisterEntry
    {
        private byte[] content;
        private int registerSize;

        public RegisterEntry(Architecture architecture)
        {
            if (architecture == Architecture.Unknown)
            {
                throw new ArgumentException("architecture");
            }

            
            switch (architecture)
            {
                case Architecture.Rv128I:
                    registerSize = 16;
                    break;
                case Architecture.Rv64I:
                    registerSize = 8;
                    break;
                default:
                    registerSize = 4;
                    break;
            }

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

        public void WriteLong (long value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBlock(bytes);
        }

        public void WriteUnsignedLong (ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBlock(bytes);
        }

        public void WriteBigInteger (BigInteger bigInt)
        {
            var bytes = bigInt.ToByteArray();
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
                // Reset the byte array and write the result as little endian coding ( B0 B1 B2 B3 ...  where B0 ist the lowest byte)
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

        public long ReadLong()
        {
            var value = BitConverter.ToInt64(content, 0);
            return value;
        }

        public ulong ReadUnsignedLong()
        {
            var value = BitConverter.ToUInt64(content, 0);
            return value;
        }

        public BigInteger ReadBigInteger()
        {
            var bigInt = new BigInteger(content);
            return bigInt;
        }

    }
}
