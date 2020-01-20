using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    /// <summary>
    /// Represents a register set based on Rv32I ISA 
    /// </summary>
    public sealed class Register32 : IRegister
    {
        private RegisterEntry[] register;
        private int pcRegister;

        /*
         * 
         *  x?      31 ..... 0 
         *  x0      0000            Readonly
         *  x1      ????            
         *  x2      ????
         *  ...
         *  x31     ????
         *  x32     pc              program counter
         *  
         * 
         * 
         * 
         * as a
         * - collection of booleans
         * - two complement signed bytes
         * - two complement unsigned bytes
         */



        internal Register32(Architecture architecture)
        {
            register = new RegisterEntry[33];  // Register 0,1,2...32

            if (architecture == Architecture.Rv32I)
            {
                pcRegister = 32;
            }
            else
            {
                pcRegister = 16;
            }

            for (int index=0; index <= 32; index++)
            {
                register[index] = new RegisterEntry(architecture);
            }
        }

        public int ProgramCounter { get { return pcRegister; } }


        #region Signed Int

        public void WriteSignedInt(int index, int value)
        {
            if (index == 0)
            {
                throw new RiscVSimException("Register x0 is read only!");
            }

            register[index].WriteInteger(value);
        }

        public int ReadSignedInt(int index)
        {
            return register[index].ReadInteger();
        }

        #endregion

        #region Unsigned Int

        public uint ReadUnsignedInt (RegisterName name)
        {
            var index = ToInt(name);
            return ReadUnsignedInt(index);
        }

        public uint ReadUnsignedInt(int index)
        {
            return register[index].ReadUnsignedInteger();
        }

        public void WriteUnsignedInt(RegisterName name, uint value)
        {
            var index = ToInt(name);
            WriteUnsignedInt(index, value);
        }

        public void WriteUnsignedInt(int index, uint value)
        {
            if (index == 0)
            {
                throw new RiscVSimException("Register x0 is read only!");
            }

            register[index].WriteUnsignedInteger(value);
        }

        // Go to the next instruction (PC + offset)
        public void NextInstruction(int offset)
        {

            var current = ReadUnsignedInt(ProgramCounter);
            WriteUnsignedInt(ProgramCounter, current + Convert.ToUInt32(offset));
        }

        #endregion

        public IEnumerable<byte> ReadBlock(int index)
        {
            return register[index].ReadBlock();
        }

        public void WriteBlock(int index, IEnumerable<byte> block)
        {
            register[index].WriteBlock(block);
        }

        private int ToInt(RegisterName name)
        {
            var index = Convert.ToInt32(name);
            return index;
        }

        public long ReadSignedLong(RegisterName name)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public long ReadSignedLong(int index)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public void WriteSignedLong(RegisterName registerName, long value)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public void WriteSignedLong(int index, long value)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public ulong ReadUnsignedLong(RegisterName name)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public ulong ReadUnsignedLong(int name)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public void WriteUnsignedLong(RegisterName name, ulong value)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }

        public void WriteUnsignedLong(int index, ulong value)
        {
            throw new ArchitectureNotSupportedException("RV32I does not support 64 Bit values");
        }
    }
}
