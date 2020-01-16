using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    /// <summary>
    /// Tests the OpCode 1B with the implementation for JAL
    /// </summary>
    public class OpCode1BTest
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }


        [Test]
        public void JumpAddressCheckTest1()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x00, 0x01 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);

            Assert.AreEqual(x1, pc_old + 4); // Make sure, that the x1 (link) register is set!
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, pc_old + 16);
        }

        /// <summary>
        ///  BLock 1 Test (Imm [21..30])
        /// </summary>
        [Test]
        public void JumpAddressCheckBlock1Test()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x20, 0x40 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);

            Assert.AreEqual(x1, pc_old + 4); // Make sure, that the x1 (link) register is set!
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, pc_old + 0x402);
        }

        /// <summary>
        ///  BLock 2 Test (Imm [11])
        /// </summary>
        [Test]
        public void JumpAddressCheckBlock2Test()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x10, 0x00 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);

            Assert.AreEqual(x1, pc_old + 4); // Make sure, that the x1 (link) register is set!
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, pc_old + 0x200);
        }

        /// <summary>
        ///  BLock 3 Test (Imm [12...19])
        /// </summary>
        [Test]
        public void JumpAddressCheckBlock3Test()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x20, 0x40 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);

            Assert.AreEqual(x1, pc_old + 4); // Make sure, that the x1 (link) register is set!
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, pc_old + 0x402);
        }


        /// <summary>
        ///  BLock 4 Test (Imm [31])
        /// </summary>
        [Test]
        public void JumpAddressCheckBlock4Test()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x80, 0x80 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);

            //TODO: The decoder of the type has to take care of signed bit !

            Assert.AreEqual(x1, pc_old + 4); // Make sure, that the x1 (link) register is set!
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, pc_old - 0x8);
        }

        [Test]
        public void JumpTestWithX1Link()
        {
            var jal = new byte[] { 0xEF, 0x00, 0x80, 0x00 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            var rasStack = core.RasStack;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);
            uint x5 = register.ReadUnsignedInt(5);

            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(x1, pc_old + 4);
            Assert.AreEqual(x5, 0);

            Assert.AreEqual(rasStack.Count, 1);
            var rasValue = rasStack.Pop();
            Assert.AreEqual(rasValue, pc_old + 4);
        }

        [Test]
        public void JumpTestWithX5Link()
        {
            var jal = new byte[] { 0xEF, 0x02, 0x80, 0x00 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            var rasStack = core.RasStack;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);
            uint x5 = register.ReadUnsignedInt(5);

            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(x5, pc_old + 4);
            Assert.AreEqual(x1, 0);

            Assert.AreEqual(rasStack.Count, 1);
            var rasValue = rasStack.Pop();
            Assert.AreEqual(rasValue, pc_old + 4);
        }

        [Test]
        public void JumpTestWithX0Link()
        {
            var jal = new byte[] { 0x6F, 0x00, 0x80, 0x00 };
            uint pc_old = core.BaseAddres;

            var register = core.Register;
            var rasStack = core.RasStack;
            core.Run(jal);


            // The address of the next instruction to be read.  We have to substract 4,because we already read it in the CPU
            uint pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            uint x1 = register.ReadUnsignedInt(1);
            uint x5 = register.ReadUnsignedInt(5);

            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(x5, 0);
            Assert.AreEqual(x1, 0);

            Assert.AreEqual(rasStack.Count, 0);
        }
    }
}
