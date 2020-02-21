using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class OpCode18Test
    {

        private BootstrapCore32 core;

        private IEnumerable<byte> initBlock;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();

            // Adds a jump point with a simple add operation
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi,10,1); // x11 = x10 + 1;
            core.Load(0x210, instAddi); 

            // Define some values for branch testing
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 15, C.opOPIMMaddi, 0, 5); // x15 = 5
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 16, C.opOPIMMaddi, 0, 5); // x16 = 5
            var instAddi3 = InstructionTypeFactory.CreateIType(C.OPIMM, 17, C.opOPIMMaddi, 0, 1); // x17 = 1
            var instAddi4 = InstructionTypeFactory.CreateIType(C.OPIMM, 18, C.opOPIMMaddi, 0, 10); // x18 = 10

            var program = new List<byte>();
            program.AddRange(instAddi1);
            program.AddRange(instAddi2);
            program.AddRange(instAddi3);
            program.AddRange(instAddi4);
            initBlock = program;
        }

        /*
         *  beq     bimm12hi rs1 rs2 bimm12lo 14..12=0 6..2=0x18 1..0=3
            bne     bimm12hi rs1 rs2 bimm12lo 14..12=1 6..2=0x18 1..0=3
            blt     bimm12hi rs1 rs2 bimm12lo 14..12=4 6..2=0x18 1..0=3
            bge     bimm12hi rs1 rs2 bimm12lo 14..12=5 6..2=0x18 1..0=3
            bltu    bimm12hi rs1 rs2 bimm12lo 14..12=6 6..2=0x18 1..0=3
            bgeu    bimm12hi rs1 rs2 bimm12lo 14..12=7 6..2=0x18 1..0=3
         * 
         */

        [Test]
        public void beqTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 15, 16, C.OPBbeq, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void beqTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 15, 17, C.OPBbeq, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }

        [Test]
        public void beqTest3()
        {
            var instBeq = new byte[] { 0xE3, 0x8F, 0x6A, 0xC0 };
            var program = initBlock.Concat(instBeq);

            core.BaseAddres = 0x30000;
            core.Run(program);
            var register = core.Register;

            var pc = register.ReadUnsignedInt(register.ProgramCounter);
            Assert.AreEqual(pc, 0x2fc2e);
        }

        [Test]
        public void bneTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 15, 17, C.OPBbne, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void bneTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 15, 16, C.OPBbne, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }

        [Test]
        public void bltTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 17, 18, C.OPBblt, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void bltTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 18, 17, C.OPBblt, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }

        [Test]
        public void bgeTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 17, 18, C.OPBbge, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }

        [Test]
        public void bgeTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 18, 17, C.OPBbge, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void bltuTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 17, 18, C.OPBbltu, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void bltuTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 18, 17, C.OPBbltu, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }

        [Test]
        public void bgeuTest1()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 18, 17, C.OPBbgeu, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 1);
        }

        [Test]
        public void bgeuTest2()
        {
            var instBeq = InstructionTypeFactory.CreateBType(C.OPB, 17, 18, C.OPBbgeu, 0x100);
            var program = initBlock.Concat(instBeq);

            core.Run(program);

            var register = core.Register;
            var x11 = register.ReadSignedInt(11);
            Assert.AreEqual(x11, 0);
        }
    }

}
