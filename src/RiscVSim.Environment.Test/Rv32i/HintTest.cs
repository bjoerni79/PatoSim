using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class HintTest
    {

        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        [Test]
        public void NopDetectionTest()
        {
            var nop1 = InstructionTypeFactory.CreateNop();
            var nop2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 0, Constant.opOPIMMaddi, 0, 0);
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var nop3 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 0, Constant.opOPIMMaddi, 0, 0);

            // The CPU shall ignore the NOP instructions and execute the ADDI one
            var program = new List<byte>();
            program.AddRange(nop1);
            program.AddRange(nop2);
            program.AddRange(instAddi1);
            program.AddRange(nop3);

            core.Run(program);

            var register = core.Register;
            var x2 = register.ReadSignedInt(2);
            Assert.AreEqual(x2, 5);

            var hint = core.Hint;
            Assert.AreEqual(3, hint.NopCounter);
        }
    }
}
