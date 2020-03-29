using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class OpCode19Test
    {
        private BootstrapCore64 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore64();
        }

        /// <summary>
        /// Simple JALR Jump without any RAS (Return Address Stack) implications
        /// </summary>
        [Test]
        public void JalrTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 25, 0, 0, 0x04);
            var instJalr = InstructionTypeFactory.CreateIType(C.OPJALR, 26, 0, 25, 0x200);
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
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 25, 0, 0, 0x04);
            var instJalr = InstructionTypeFactory.CreateIType(C.OPJALR, 0, 0, 25, 0x200);
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
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 25, 0, 0, 0x05);
            var instJalr = InstructionTypeFactory.CreateIType(C.OPJALR, 0, 0, 25, 0x200);
            var program = instAddi.Concat(instJalr);

            var register = core.Register;
            var pc_old = register.ReadUnsignedInt(register.ProgramCounter);
            core.Run(program);

            var pc_new = register.ReadUnsignedInt(register.ProgramCounter);
            Assert.AreNotEqual(pc_old, pc_new);
            Assert.AreEqual(pc_new, 0x204);
        }

        private void InitJalrSubRoutine()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 1); // X10 =1
            var instJalr1 = InstructionTypeFactory.CreateIType(C.OPJALR, 0, 0, 1, 0); // pop and return

            var subRoutine = new List<byte>();
            subRoutine.AddRange(instAddi1);
            subRoutine.AddRange(instJalr1);

            core.Load(0x200, subRoutine);
        }

        [Test]
        public void JalrRasPopTest()
        {
            // Idee: 
            // 1. Springe via JAL zu einer Speicherstelle x1 und schreibe eine 1 in ein Register 10
            // 2. Von dort springe zurück (via POP)
            // 3. Springe via JAL zu einer Speicherstelle x2 und schreibe eine 2 in ein Register 11
            // 4. Von dort springe zurück
            // 5. Addiere x10 und x11 mit rd = 12

            // Testen der Register

            var instJal1 = new byte[] { 0xEF, 0x00, 0x00, 0x10 }; // Jump rd = x1 with Offset 0x100
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 1); // X11 = 1;

            var program = new List<byte>();
            program.AddRange(instJal1);
            program.AddRange(instAddi1);

            InitJalrSubRoutine();
            core.Run(program);

            // Test:  The subroutine writes x10 and after return x11 is written
            var register = core.Register;
            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x10, 1);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void JalrRasPushTest()
        {
            // Idee:
            // Push the 104 to the stack and jump to 200 (subroutine)

            var instJalr1 = InstructionTypeFactory.CreateIType(C.OPJALR, 1, 0, 0, 0x200);
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 1); // X11 = 1;

            var program = new List<byte>();
            program.AddRange(instJalr1);
            program.AddRange(instAddi1);

            InitJalrSubRoutine();
            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x10, 1);
            Assert.AreEqual(x11, 1);
        }

        //[Test]
        //public void JalrRasPopPushWithRdRS1REqZeroTest()
        //{
        //    TestHelper.NotImplementedYet();
        //}

        //[Test]
        //public void JalrPushWithRdRs1Eq1Test()
        //{
        //    TestHelper.NotImplementedYet();
        //}
    }
}
