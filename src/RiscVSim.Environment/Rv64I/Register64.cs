using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class Register64 : IRegister
    {
        public int ProgramCounter => 32; // x0 ....x31, x32 = PC

        internal Register64()
        {

        }

        public void NextInstruction(int offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ReadBlock(int index)
        {
            throw new NotImplementedException();
        }

        public int ReadSignedInt(int index)
        {
            throw new NotImplementedException();
        }

        public uint ReadUnsignedInt(RegisterName name)
        {
            throw new NotImplementedException();
        }

        public uint ReadUnsignedInt(int index)
        {
            throw new NotImplementedException();
        }

        public void WriteBlock(int index, IEnumerable<byte> block)
        {
            throw new NotImplementedException();
        }

        public void WriteSignedInt(int index, int value)
        {
            throw new NotImplementedException();
        }

        public void WriteUnsignedInt(RegisterName name, uint value)
        {
            throw new NotImplementedException();
        }

        public void WriteUnsignedInt(int index, uint value)
        {
            throw new NotImplementedException();
        }
    }
}
