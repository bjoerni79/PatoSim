using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class OpCode05Test
    {
        private BootstrapCore64 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore64();
        }

        [Test]
        public void AuipcTest1()
        {
            var insAuipc1 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 1, 0x02);

            var program = insAuipc1;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 0x100;

            var register = core.Register;
            var x1 = register.ReadBlock(1);
            Assert.AreEqual(x1, new byte[] {0x00, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }


        [Test]
        public void AuipcTest2()
        {
            var insAuipc1 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 1, 0x100);
            var program = insAuipc1;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 0x100;

            var register = core.Register;
            var x1 = register.ReadBlock(1);

            Assert.AreEqual(x1, new byte[] {0x00, 0x01, 0x10,0x00, 0x00, 0x00, 0x00, 0x00 });

        }

        [Test]
        public void AuipcTest3()
        {
            var insAuipc3 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 1, 0xFFF);

            var program = insAuipc3;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 0x100;

            var register = core.Register;
            var x1 = register.ReadBlock(1);

            Assert.AreEqual(x1, new byte[] { 0x00, 0xF1, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void AuipcTest4()
        {
            var insAuipc3 = InstructionTypeFactory.CreateUType(C.OPAUIPC, 1, 0xFFFFF);

            var program = insAuipc3;

            // PC = 100
            core.Run(program);
            core.BaseAddres = 0x100;

            var register = core.Register;
            var x1 = register.ReadBlock(1);

            Assert.AreEqual(x1, new byte[] { 0x00, 0xF1, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }
    }
}
