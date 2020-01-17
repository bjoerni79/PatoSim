using NUnit.Framework;
using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class OpCode06Test
    {
        private BootstrapCore64 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore64();
        }

        [Test]
        public void AddiWTest1()
        {
            //var program = new byte[] { 0x00, 0x81, 0x00, 0x93,  };
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 1, C.OPIMM32_addiw, 0, 8); // addi : rd(1) = rs(2) + 8
            var inst2 = InstructionTypeFactory.CreateIType(C.OPIMM32, 2, C.OPIMM32_addiw, 1, 2); // addi : rd(3) = rs(1) + 2

            var program = inst1.Concat(inst2);
            core.Run(program);

            var x1Content = core.Register.ReadSignedLong(1);
            var x2Content = core.Register.ReadSignedLong(2);
            Assert.AreEqual(x1Content, 8);
            Assert.AreEqual(x2Content, 10);
        }

        [Test]
        public void AddiWTest2()
        {
            //var program = new byte[] { 0x00, 0x81, 0x00, 0x93,  };
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 1, C.OPIMM32_addiw, 0, 0xFFE); // addi : rd(1) = 0 - 2

            var program = inst1;
            core.Run(program);

            var x1Content = core.Register.ReadBlock(1);
            Assert.AreEqual(x1Content, new byte[] { 0x02, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [Test]
        public void SlliwTest()
        {
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 1, C.OPIMM32_addiw, 0, 1);
            var instslliw1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 2, C.OPIMM32_slliw, 1, 31);
            var instslliw2 = InstructionTypeFactory.CreateIType(C.OPIMM32, 3, C.OPIMM32_slliw, 2, 31);

            var program = new List<byte>();
            program.AddRange(inst1);
            program.AddRange(instslliw1);
            program.AddRange(instslliw2);

            core.Run(program);
            var register = core.Register;

            var x1 = register.ReadBlock(1);
            var x2 = register.ReadBlock(2);
            var x3 = register.ReadBlock(3);

            Assert.AreEqual(x1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2, new byte[] { 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x3, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void SrliwTest()
        {
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 1, C.OPIMM32_addiw, 0, 3);
            var instslli1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2,C.opOPIMMslli, 1, 31);
            var instsrliw1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 3, C.OPIMM32_srliw_sraiw, 2, 1);

            var program = new List<byte>();
            program.AddRange(inst1);
            program.AddRange(instslli1);
            program.AddRange(instsrliw1);

            core.Run(program);

            var register = core.Register;

            var x1 = register.ReadBlock(1);
            var x2 = register.ReadBlock(2);
            var x3 = register.ReadBlock(3);

            Assert.AreEqual(x1, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2, new byte[] { 0x00, 0x00, 0x00, 0x80, 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x3, new byte[] { 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void SraiwTest()
        {
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 1, C.OPIMM32_addiw, 0, 3);
            var instslli1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMslli, 1, 31);
            var instsrliw1 = InstructionTypeFactory.CreateIType(C.OPIMM32, 3, C.OPIMM32_srliw_sraiw, 2, 0x401);

            var program = new List<byte>();
            program.AddRange(inst1);
            program.AddRange(instslli1);
            program.AddRange(instsrliw1);

            core.Run(program);

            var register = core.Register;

            var x1 = register.ReadBlock(1);
            var x2 = register.ReadBlock(2);
            var x3 = register.ReadBlock(3);

            Assert.AreEqual(x1, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2, new byte[] { 0x00, 0x00, 0x00, 0x80, 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x3, new byte[] { 0x00, 0x00, 0x00, 0xC0, 0xFF, 0xFF, 0xFF, 0xFF });
        }
    }
}
