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
    }
}
