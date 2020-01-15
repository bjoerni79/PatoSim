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
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        [Test]
        public void LhTest1WithoutRsTest1()
        {
            core.Load(400, new byte[] { 0x01, 0x02 });
            core.Load(600, new byte[] { 0x03, 0x84 });


            var instLh1 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 10, Constant.OPLOADlh, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 11, Constant.OPLOADlh, 0, 600);
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

            var instAddi = InstructionTypeFactory.CreateIType(Constant.OPIMM, 9, Constant.opOPIMMaddi, 9, 100);
            var instLh1 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 10, Constant.OPLOADlh, 9, 300);
            var instLh2 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 11, Constant.OPLOADlh, 9, 500);
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


            var instLh1 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 10, Constant.OPLOADlhu, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 11, Constant.OPLOADlhu, 0, 600);
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


            var instLh1 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 10, Constant.OPLOADlb, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 11, Constant.OPLOADlb, 0, 600);
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


            var instLh1 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 10, Constant.OPLOADlw, 0, 400);
            var instLh2 = InstructionTypeFactory.CreateIType(Constant.OPLOAD, 11, Constant.OPLOADlwu, 0, 600);
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
