using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class OpCode1CTest
    {
        private BootstrapCore64 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore64();
        }

        [Test]
        public void CsrrwTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 5);
            var instCsrrw = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);

            var app = instAddi.Concat(instCsrrw);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x10, 5);
            Assert.AreEqual(x11, 0);
            Assert.AreEqual(csr100, 5);
        }

        [Test]
        public void CsrrwTest4()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 5);
            var instCsrrw = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 4095);

            var app = instAddi.Concat(instCsrrw);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var csrMax = csrRegister.Read(4095);

            Assert.AreEqual(x10, 5);
            Assert.AreEqual(x11, 0);
            Assert.AreEqual(csrMax, 5);
        }

        [Test]
        public void CsrrwTest2()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 0xFF);
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);

            var app = instAddi.Concat(instCsrrw1);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x10, 0xFF);
            Assert.AreEqual(x11, 0);
            Assert.AreEqual(csr100, 0x1F);
        }

        [Test]
        public void CsrrwTest3()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 0xFF);
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);
            var instCsrrw2 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);

            var app = instAddi.Concat(instCsrrw1).Concat(instCsrrw2);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x10, 0xFF);
            Assert.AreEqual(x11, 0x1F);
            Assert.AreEqual(csr100, 0x1F);
        }

        [Test]
        public void CsrrsTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 0xFF);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 12, 0, 0, 0x02);
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);
            var instCsrrs1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 13, 2, 12, 100);

            // 1. Fill CSR(100) with 0x1F
            // 2. Run csrrs from RS1=12

            var app = new List<byte>();
            app.AddRange(instAddi1);
            app.AddRange(instAddi2);
            app.AddRange(instCsrrw1);
            app.AddRange(instCsrrs1);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x10, 0xFF);
            Assert.AreEqual(x12, 0x2);
            Assert.AreEqual(x13, 0x1F);

            Assert.AreEqual(x11, 0x00);
            Assert.AreEqual(csr100, 0x02);
        }

        [Test]
        public void CsrrcTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, 0, 0, 0xFF);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 12, 0, 0, 0x02);
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 1, 10, 100);
            var instCsrrc1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 13, 3, 12, 100);

            // 1. Fill CSR(100) with 0x1F
            // 2. Run csrrs from RS1=12

            var app = new List<byte>();
            app.AddRange(instAddi1);
            app.AddRange(instAddi2);
            app.AddRange(instCsrrw1);
            app.AddRange(instCsrrc1);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x10, 0xFF);
            Assert.AreEqual(x12, 0x02);
            Assert.AreEqual(x13, 0x1F);

            Assert.AreEqual(x11, 0x00);
            Assert.AreEqual(csr100, 0x1D);
        }

        [Test]
        public void CsrrwiTest1()
        {
            var instCsrrw = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 5, 5, 100);

            var app = instCsrrw;

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var csr100 = csrRegister.Read(100);

            Assert.AreEqual(x11, 0);
            Assert.AreEqual(csr100, 5);
        }

        [Test]
        public void CsrrsiTest1()
        {
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 5, 0x1F, 100);
            var instCsrrs1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 13, 6, 0x02, 100);

            // 1. Fill CSR(100) with 0x1F
            // 2. Run csrrs from RS1=12

            var app = new List<byte>();
            app.AddRange(instCsrrw1);
            app.AddRange(instCsrrs1);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);
            var csr100 = csrRegister.Read(100);


            Assert.AreEqual(x13, 0x1F);

            Assert.AreEqual(x11, 0x00);
            Assert.AreEqual(csr100, 0x02);
        }

        [Test]
        public void CsrrciTest1()
        {
            var instCsrrw1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 11, 5, 0x1F, 100);
            var instCsrrs1 = InstructionTypeFactory.CreateIType(C.OPSYSTEM, 13, 7, 0x02, 100);

            // 1. Fill CSR(100) with 0x1F
            // 2. Run csrrs from RS1=12

            var app = new List<byte>();
            app.AddRange(instCsrrw1);
            app.AddRange(instCsrrs1);

            core.Run(app);

            var register = core.Register;
            var csrRegister = core.CsrRegister;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);
            var csr100 = csrRegister.Read(100);


            Assert.AreEqual(x13, 0x1F);

            Assert.AreEqual(x11, 0x00);
            Assert.AreEqual(csr100, 0x1D);
        }
    }
}
