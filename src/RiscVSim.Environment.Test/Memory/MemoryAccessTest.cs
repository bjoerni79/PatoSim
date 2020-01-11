using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Memory
{
    public class MemoryAccessTest
    {
        private IMemory memory;
        private uint baseAddress = 10;

        [SetUp]
        public void Setup()
        {
            memory = Factory.CreateDynamicMemory(Architecture.Rv32I);
        }

        [Test]
        public void FetchInstructionTest1()
        {
            var instruction = new byte[4] { 1, 2, 3, 4 };
            var instructionEmpty = new byte[4];

            // The memory is empty. 0 0 0 0 is expected.
            var instruction1 = memory.FetchInstruction(baseAddress);
            Assert.AreEqual(instruction1, instructionEmpty);

            // Now fill the memory and test again..

            memory.Write(baseAddress, instruction);
            var instruction2 = memory.FetchInstruction(baseAddress);
            Assert.AreEqual(instruction2, instruction);
        }


        [Test]
        public void ReadAndWriteBlockTest()
        {
            var buffer = new byte[] { 0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F};

            memory.Write(baseAddress, buffer);

            var buffer1 = memory.Read(baseAddress, buffer.Count());

            Assert.AreEqual(buffer, buffer1);
        }
    }
}
