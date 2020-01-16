using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    /// <summary>
    ///  Tests for Opcode 05 (auipc instruction)
    /// </summary>
    public class Opcode05Test
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        [Test]
        public void AuipcTest1()
        {
            var insAuipc1 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 1, 0x01);

            var program = insAuipc1;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 100;

            var register = core.Register;
            var x1 = register.ReadUnsignedInt(1);
            Assert.AreEqual(x1, 0x1100);
        }


        [Test]
        public void AuipcTest2()
        {
            var insAuipc2 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 2, 0x100);
            var program = insAuipc2;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 100;

            var register = core.Register;
            var x2 = register.ReadUnsignedInt(2);

            Assert.AreEqual(x2, 0x100100);

        }

        [Test]
        public void AuipcTest3()
        {
            var insAuipc3 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 3, 0xFFF);

            var program = insAuipc3;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 100;

            var register = core.Register;
            var x3 = register.ReadUnsignedInt(3);

            Assert.AreEqual(x3, 0xFFF100);
        }
    }
}
