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



        internal Register32()
        {
            register = new RegisterEntry[33];  // Register 0,1,2...32
            pcRegister = 32;

            for (int index=0; index <= 32; index++)
            {
                register[index] = new RegisterEntry(Architecture.Rv32I);
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

    }
}
