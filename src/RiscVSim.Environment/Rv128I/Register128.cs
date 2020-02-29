﻿using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Numerics;
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
            //var currentProgrammCounter = ReadUnsignedLong(ProgramCounter);
            //ulong newProgramCounter = currentProgrammCounter + Convert.ToUInt64(offset);

            //WriteUnsignedLong(ProgramCounter, newProgramCounter);
        }

        public BigInteger ReadBigInteger(int index)
        {
            return register[index].ReadBigInteger();
        }

        public BigInteger ReadBigInteger(RegisterName name)
        {
            var index = ToInt(name);
            return register[index].ReadBigInteger();
        }

        public IEnumerable<byte> ReadBlock(int index)
        {
            return register[index].ReadBlock();
        }

        public int ReadSignedInt(int index)
        {
            return register[index].ReadInteger();
        }

        public long ReadSignedLong(RegisterName name)
        {
            var index = ToInt(name);
            return ReadSignedLong(index);
        }

        public long ReadSignedLong(int index)
        {
            return register[index].ReadLong();
        }

        public uint ReadUnsignedInt(RegisterName name)
        {
            var index = ToInt(name);
            return ReadUnsignedInt(index);
        }

        public uint ReadUnsignedInt(int index)
        {
            return register[index].ReadUnsignedInteger();
        }

        public ulong ReadUnsignedLong(RegisterName name)
        {
            var index = ToInt(name);
            return ReadUnsignedLong(index);
        }

        public ulong ReadUnsignedLong(int index)
        {
            return register[index].ReadUnsignedLong();
        }

        public void WriteBigInteger(int index, BigInteger bigInteger)
        {
            CheckForX0(index);
            register[index].WriteBigInteger(bigInteger);
        }

        public void WriteBigInteger(RegisterName name, BigInteger bigInteger)
        {
            var index = ToInt(name);
            WriteBigInteger(index, bigInteger);
        }

        public void WriteBlock(int index, IEnumerable<byte> block)
        {
            CheckForX0(index);

            register[index].WriteBlock(block);
        }

        public void WriteSignedInt(int index, int value)
        {
            CheckForX0(index);

            register[index].WriteInteger(value);
        }

        public void WriteSignedLong(RegisterName registerName, long value)
        {
            var index = ToInt(registerName);
            WriteSignedLong(index, value);
        }

        public void WriteSignedLong(int index, long value)
        {
            CheckForX0(index);

            register[index].WriteLong(value);
        }

        public void WriteUnsignedInt(RegisterName registerName, uint value)
        {
            var index = ToInt(registerName);
            WriteUnsignedInt(index, value);
        }

        public void WriteUnsignedInt(int index, uint value)
        {
            CheckForX0(index);
            register[index].WriteUnsignedInteger(value);
        }

        public void WriteUnsignedLong(RegisterName registerName, ulong value)
        {
            var index = ToInt(registerName);
            WriteUnsignedLong(index, value);
        }

        public void WriteUnsignedLong(int index, ulong value)
        {
            CheckForX0(index);

            register[index].WriteUnsignedLong(value);
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