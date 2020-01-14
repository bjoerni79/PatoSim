using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class OpCode19Test
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        /// <summary>
        /// Simple JALR Jump without any RAS (Return Address Stack) implications
        /// </summary>
        [Test]
        public void JalrTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 25, 0, 0, 0x04);
            var instJalr = InstructionTypeFactory.CreateIType(Constant.OPJALR, 26, 0, 25, 0x200);
            var program = instAddi.Concat(instJalr);

            var register = core.Register;
            var pc_old = register.ReadUnsignedInt(register.ProgramCounter);
            core.Run(program);

            var pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            var x26 = register.ReadSignedInt(26);

            Assert.AreEqual(x26, 0x108); // 100 + Addi (4) + next(4) = 108
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, 0x204);
        }

        /// <summary>
        /// JALR with RD=0 is valid ( goes to void!)
        /// </summary>
        [Test]
        public void JalrTest2()
        {
            var instAddi = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 25, 0, 0, 0x04);
            var instJalr = InstructionTypeFactory.CreateIType(Constant.OPJALR, 0, 0, 25, 0x200);
            var program = instAddi.Concat(instJalr);

            var register = core.Register;
            var pc_old = register.ReadUnsignedInt(register.ProgramCounter);
            core.Run(program);

            var pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, 0x204);
        }

        /// <summary>
        /// Same like Test2, but here the LSB bit needs to get adjusted!
        /// </summary>
        [Test]
        public void JalrTest3()
        {
            var instAddi = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 25, 0, 0, 0x05);
            var instJalr = InstructionTypeFactory.CreateIType(Constant.OPJALR, 0, 0, 25, 0x200);
            var program = instAddi.Concat(instJalr);

            var register = core.Register;
            var pc_old = register.ReadUnsignedInt(register.ProgramCounter);
            core.Run(program);

            var pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, 0x204);
        }


    }
}
