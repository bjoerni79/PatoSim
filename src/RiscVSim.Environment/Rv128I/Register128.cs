using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv128I
{
    public class Register128 : IRegister
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private RegisterEntry[] register;
        private int pcRegister;

        public Register128()
        {
            register = new RegisterEntry[33];  // Register 0,1,2...32
            pcRegister = 32;

            for (int index = 0; index <= 32; index++)
            {
                register[index] = new RegisterEntry(Architecture.Rv128I);
            }
        }

        public int ProgramCounter => 32;

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

        public long ReadSignedLong(RegisterName name)
        {
            throw new NotImplementedException();
        }

        public long ReadSignedLong(int index)
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

        public ulong ReadUnsignedLong(RegisterName name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUnsignedLong(int name)
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

        public void WriteSignedLong(RegisterName registerName, long value)
        {
            throw new NotImplementedException();
        }

        public void WriteSignedLong(int index, long value)
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

        public void WriteUnsignedLong(RegisterName name, ulong value)
        {
            throw new NotImplementedException();
        }

        public void WriteUnsignedLong(int index, ulong value)
        {
            throw new NotImplementedException();
        }

        private void CheckForX0(int index)
        {
            if (index == 0)
            {
                Logger.Error("X0 is readonly!");
                throw new RiscVSimException("Register X0 is only readable!");
            }
        }

        private int ToInt(RegisterName name)
        {
            var index = Convert.ToInt32(name);
            return index;
        }
    }
}
