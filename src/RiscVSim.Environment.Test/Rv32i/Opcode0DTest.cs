using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class Opcode0DTest
    {
        private BootstrapCore32 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
        }

        [Test]
        public void LuiTest1()
        {
            var insLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 1, 0x01);
            var insLui2 = InstructionTypeFactory.CreateUType(C.OPLUI, 2, 0x100);

            var program = insLui1.Concat(insLui2);

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadBlock(1);
            var x2 = register.ReadBlock(2);

            Assert.AreEqual(x1, new byte[] { 0x00, 0x10, 0x00, 0x00 });
            Assert.AreEqual(x2, new byte[] { 0x00, 0x00, 0x10, 0x00 });
            
        }

        [Test]
        public void LuiTest2()
        {
            var insLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 1, 0xFFF);
            var program = insLui1;

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadBlock(1);
            Assert.AreEqual(x1, new byte[] {0x00, 0xF0, 0xFF, 0x00 });

        }

        [Test]
        public void LuiTest3()
        {
            var insLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 1, 0xFFFFF);
            var program = insLui1;

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadBlock(1);
            Assert.AreEqual(x1, new byte[] { 0x00, 0xF0, 0xFF, 0xFF });
        }
    }
}
