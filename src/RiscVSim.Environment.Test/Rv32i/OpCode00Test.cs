using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class OpCode00Test
    {
        private BootstrapCore32 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
        }

        [Test]
        public void LhTest1WithoutRsTest1()
        {
            core.Load(400, new byte[] { 0x01, 0x02 });
            core.Load(600, new byte[] { 0x03, 0x84 });


            var instLh1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 10, C.OPLOADlh, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(C.OPLOAD, 11, C.OPLOADlh, 0, 600);
            var program = instLh1.Concat(instLh2);

            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);

            Assert.AreEqual(x10, new byte[] { 0x01, 0x02, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x03, 0x04, 0x00, 0x80 });

        }

        [Test]
        public void LhTestWithRSandImmTest1()
        {
            core.Load(400, new byte[] { 0x01, 0x02 });
            core.Load(600, new byte[] { 0x03, 0x84 });

            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 9, C.opOPIMMaddi, 9, 100);
            var instLh1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 10, C.OPLOADlh, 9, 300);
            var instLh2 = InstructionTypeFactory.CreateIType(C.OPLOAD, 11, C.OPLOADlh, 9, 500);
            var program = instAddi.Concat(instLh1).Concat(instLh2);

            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);

            Assert.AreEqual(x10, new byte[] { 0x01, 0x02, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x03, 0x04, 0x00, 0x80 });

        }

        [Test]
        public void LhuTest1()
        {
            core.Load(400, new byte[] { 0x01, 0x02 });
            core.Load(600, new byte[] { 0x03, 0x84 });


            var instLh1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 10, C.OPLOADlhu, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(C.OPLOAD, 11, C.OPLOADlhu, 0, 600);
            var program = instLh1.Concat(instLh2);

            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);

            Assert.AreEqual(x10, new byte[] { 0x01, 0x02, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x03, 0x84, 0x00, 0x00 });

        }

        [Test]
        public void LbTest1()
        {
            core.Load(400, new byte[] { 0x01 });
            core.Load(600, new byte[] { 0x83 });


            var instLh1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 10, C.OPLOADlb, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(C.OPLOAD, 11, C.OPLOADlb, 0, 600);
            var program = instLh1.Concat(instLh2);

            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);

            Assert.AreEqual(x10, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x03, 0x00, 0x00, 0x80 });

        }

        [Test]
        public void LwAndLwuTest1()
        {
            core.Load(400, new byte[] { 0x01, 0x02, 0x03, 0x04 });
            core.Load(600, new byte[] { 0x05, 0x06, 0x07, 0x08 });


            var instLh1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 10, C.OPLOADlw, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(C.OPLOAD, 11, C.OPLOADlwu, 0, 600);
            var program = instLh1.Concat(instLh2);

            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);

            Assert.AreEqual(x10, new byte[] { 0x01, 0x02, 0x03, 0x04 });
            Assert.AreEqual(x11, new byte[] { 0x05, 0x06, 0x07, 0x08 });

        }
    }
}
