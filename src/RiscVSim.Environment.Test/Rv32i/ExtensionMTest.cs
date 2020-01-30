using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class ExtensionMTest
    {
        private BootstrapCore32 core;
        private List<byte> program;

        /*
         * 
         * # RV32M
            mul     rd rs1 rs2 31..25=1 14..12=0 6..2=0x0C 1..0=3
            mulh    rd rs1 rs2 31..25=1 14..12=1 6..2=0x0C 1..0=3
            mulhsu  rd rs1 rs2 31..25=1 14..12=2 6..2=0x0C 1..0=3
            mulhu   rd rs1 rs2 31..25=1 14..12=3 6..2=0x0C 1..0=3
            div     rd rs1 rs2 31..25=1 14..12=4 6..2=0x0C 1..0=3
            divu    rd rs1 rs2 31..25=1 14..12=5 6..2=0x0C 1..0=3
            rem     rd rs1 rs2 31..25=1 14..12=6 6..2=0x0C 1..0=3
            remu    rd rs1 rs2 31..25=1 14..12=7 6..2=0x0C 1..0=3

            # RV64M
            mulw    rd rs1 rs2 31..25=1 14..12=0 6..2=0x0E 1..0=3
            divw    rd rs1 rs2 31..25=1 14..12=4 6..2=0x0E 1..0=3
            divuw   rd rs1 rs2 31..25=1 14..12=5 6..2=0x0E 1..0=3
            remw    rd rs1 rs2 31..25=1 14..12=6 6..2=0x0E 1..0=3
            remuw   rd rs1 rs2 31..25=1 14..12=7 6..2=0x0E 1..0=3
         * 
         */

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
            program = new List<byte>();
        }

        [Test]
        public void MulAndMulhTest1()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 2));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 10));
            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL  : x12 = 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 1)); // MULH : x13 = 2*10 = 20

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);

            Assert.AreEqual(x10, 2);
            Assert.AreEqual(x11, 10);
            Assert.AreEqual(x12, 20);
            Assert.AreEqual(x13, 0);
        }


        [Test]
        public void MulAndMulhTest3()
        {
            program.AddRange(InstructionTypeFactory.Lui(10, 0xFFFFF));
            program.AddRange(InstructionTypeFactory.Lui(11, 0x00FFF));

            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL: 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 1));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);
            var x13 = register.ReadBlock(13);

            Assert.AreEqual(x10, new byte[] { 0x00, 0xF0, 0xFF, 0xFF });
            Assert.AreEqual(x11, new byte[] { 0x00, 0xF0, 0xFF, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0x00, 0x00, 0x01 });
            Assert.AreEqual(x13, new byte[] { 0xF0, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void MulhsuTest1()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 2));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 10));
            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL  : x12 = 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 2)); // MULH : x13 = 2*10 = 20

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);

            Assert.AreEqual(x10, 2);
            Assert.AreEqual(x11, 10);
            Assert.AreEqual(x12, 20);
            Assert.AreEqual(x13, 0);
        }

        [Test]
        public void MulhsuTest2()
        {
            program.AddRange(InstructionTypeFactory.Lui(10, 0xFFFFF));
            program.AddRange(InstructionTypeFactory.Lui(11, 0x00FFF));

            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL: 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 2));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);
            var x13 = register.ReadBlock(13);

            Assert.AreEqual(x10, new byte[] { 0x00, 0xF0, 0xFF, 0xFF });
            Assert.AreEqual(x11, new byte[] { 0x00, 0xF0, 0xFF, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0x00, 0x00, 0x01 });
            Assert.AreEqual(x13, new byte[] { 0xF0, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void MulhuTest1()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 2));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 10));
            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL  : x12 = 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 3)); // MULH : x13 = 2*10 = 20

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadSignedInt(10);
            var x11 = register.ReadSignedInt(11);
            var x12 = register.ReadSignedInt(12);
            var x13 = register.ReadSignedInt(13);

            Assert.AreEqual(x10, 2);
            Assert.AreEqual(x11, 10);
            Assert.AreEqual(x12, 20);
            Assert.AreEqual(x13, 0);
        }

        [Test]
        public void MulhuTest2()
        {
            program.AddRange(InstructionTypeFactory.Lui(10, 0xFFFFF));
            program.AddRange(InstructionTypeFactory.Lui(11, 0x00FFF));

            program.AddRange(InstructionTypeFactory.MultiplyOP(12, 10, 11, 0)); // MUL: 2*10 = 20
            program.AddRange(InstructionTypeFactory.MultiplyOP(13, 10, 11, 3));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);
            var x13 = register.ReadBlock(13);

            Assert.AreEqual(x10, new byte[] { 0x00, 0xF0, 0xFF, 0xFF });
            Assert.AreEqual(x11, new byte[] { 0x00, 0xF0, 0xFF, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0x00, 0x00, 0x01 });
            Assert.AreEqual(x13, new byte[] { 0xF0, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void DivTest1()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 10));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 5));
            program.AddRange(InstructionTypeFactory.DivideOP(12, 10, 11, 4));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0x0A, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x05, 0x00, 0x00 ,0x00 });
            Assert.AreEqual(x12, new byte[] { 0x02, 0x00, 0x00, 0x00 });

        }

        [Test]
        public void DivTest2()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 3));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 2));
            program.AddRange(InstructionTypeFactory.DivideOP(12, 10, 11, 4));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0x03, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x02, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x01, 0x00, 0x00, 0x00 });

        }

        //[Test]
        //public void DivuTest1()
        //{
        //    // Postponed... we are using the same algorithm and working with byte arrays
        //}

        [Test]
        public void RemTest1()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 3));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 2));
            program.AddRange(InstructionTypeFactory.DivideOP(12, 10, 11, 6));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0x03, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x02, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x01, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void RemTest2()
        {
            program.AddRange(InstructionTypeFactory.Addi(10, 0, 4));
            program.AddRange(InstructionTypeFactory.Addi(11, 0, 2));
            program.AddRange(InstructionTypeFactory.DivideOP(12, 10, 11, 6));

            core.Run(program);
            var register = core.Register;

            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0x04, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x02, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        //[Test]
        //public void RemuTest1()
        //{
        //    // Postponed... we are using the same algorithm and working with byte arrays
        //}
    }
}
