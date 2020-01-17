using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class PlaygroundTest
    {
        private BootstrapCore32 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32(Architecture.Rv64I);
        }


        [Test]
        public void AddITest1()
        {
            //var program = new byte[] { 0x00, 0x81, 0x00, 0x93,  };
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 2, 8); // addi : rd(1) = rs(2) + 8
            var inst2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMaddi, 1, 2); // addi : rd(3) = rs(1) + 2

            var program = inst1.Concat(inst2);
            core.Run(program);

            var x1Content = core.Register.ReadUnsignedInt(1);
            var x3Content = core.Register.ReadUnsignedInt(3);
            Assert.AreEqual(x1Content, 8);
            Assert.AreEqual(x3Content, 10);
        }
    }
}
