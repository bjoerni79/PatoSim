using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class Opcode0DTest
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        [Test]
        public void LuiTest1()
        {
            var insLui1 = InstructionTypeFactory.CreateUType(Constant.opOPLUI, 1, 0x01);
            var insLui2 = InstructionTypeFactory.CreateUType(Constant.opOPLUI, 2, 0x100);
            var insLui3 = InstructionTypeFactory.CreateUType(Constant.opOPLUI, 3, 0xFFF);

            var program = insLui1.Concat(insLui2).Concat(insLui3);

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadUnsignedInt(1);
            var x2 = register.ReadUnsignedInt(2);
            var x3 = register.ReadUnsignedInt(3);

            Assert.AreEqual(x1, 0x1000);
            Assert.AreEqual(x2, 0x100000);
            Assert.AreEqual(x3, 0xFFF000);
            
        }
    }
}
