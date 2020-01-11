using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    /// <summary>
    /// Tests all operation based on the opcode = 0x0C (
    /// </summary>
    public class Opcode0CTest
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }



        [Test]
        public void AddSubTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 4);
            // x2 = 5, x3 = 4;
            var instAdd = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opOPaddAndSub, 2, 3, Constant.opOPf7Add);
            var instSub1 = InstructionTypeFactory.CreateRType(Constant.opOP, 5, Constant.opOPaddAndSub, 2, 3, Constant.opOPf7Sub);
            var instSub2 = InstructionTypeFactory.CreateRType(Constant.opOP, 6, Constant.opOPaddAndSub, 3, 2, Constant.opOPf7Sub);
            // x4 = x2 + x3
            // x5 = x3 - x2;
            // x6 = x2 - x3;

            List<byte> program = new List<byte>();
            program.AddRange(instAddi1);
            program.AddRange(instAddi2);
            program.AddRange(instAdd);
            program.AddRange(instSub1);
            program.AddRange(instSub2);

            core.Run(program);

            var x2Value = core.Register.ReadUnsignedInt(2);
            var x3Value = core.Register.ReadUnsignedInt(3);
            var x4Value = core.Register.ReadUnsignedInt(4);
            var x5Value = core.Register.ReadUnsignedInt(5);
            var x6Value = core.Register.ReadUnsignedInt(6);

            // Check if X2 and X3 are correct
            Assert.AreEqual(x2Value, 5);
            Assert.AreEqual(x3Value, 4);
            // Check if X4 is correct
            Assert.AreEqual(x4Value, 9);
            // Check if x5 and x6 are correct
            Assert.AreEqual(x5Value, 0xffffffff);
            Assert.AreEqual(x6Value, 1); 

        }

        [Test]
        public void SltTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 4);
            var instSlt1 = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opOPslt, 2, 3, 0);
            var instSlt2 = InstructionTypeFactory.CreateRType(Constant.opOP, 5, Constant.opOPslt, 3, 2, 0);
            var program = instAddi1.Concat(instAddi2).Concat(instSlt1).Concat(instSlt2);

            core.Run(program);

            var x2Value = core.Register.ReadSignedInt(2);
            var x3Value = core.Register.ReadSignedInt(3);
            var x4Value = core.Register.ReadSignedInt(4);
            var x5Value = core.Register.ReadSignedInt(5);

            Assert.AreEqual(x2Value, 5);
            Assert.AreEqual(x3Value, 4);
            Assert.AreEqual(x4Value, 0); // 5 < 4 = false
            Assert.AreEqual(x5Value, 1); // 4 < 5 = true

        }

        [Test]
        public void SltuTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 4);
            var instSlt1 = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opOPsltu, 2, 3, 0);
            var instSlt2 = InstructionTypeFactory.CreateRType(Constant.opOP, 5, Constant.opOPsltu, 3, 2, 0);
            var instSlt3 = InstructionTypeFactory.CreateRType(Constant.opOP, 6, Constant.opOPsltu, 0, 2, Constant.opOPf2sra);
            var instSlt4 = InstructionTypeFactory.CreateRType(Constant.opOP, 7, Constant.opOPsltu, 0, 10, Constant.opOPf2sra);
            var program = instAddi1.Concat(instAddi2).Concat(instSlt1).Concat(instSlt2).Concat(instSlt3).Concat(instSlt4);

            core.Run(program);

            var x2Value = core.Register.ReadUnsignedInt(2);
            var x3Value = core.Register.ReadUnsignedInt(3);
            var x4Value = core.Register.ReadUnsignedInt(4);
            var x5Value = core.Register.ReadUnsignedInt(5);
            var x6Value = core.Register.ReadUnsignedInt(6);
            var x7Value = core.Register.ReadUnsignedInt(7);

            Assert.AreEqual(x2Value, 5);
            Assert.AreEqual(x3Value, 4);
            Assert.AreEqual(x4Value, 0); // 5 < 4 = false
            Assert.AreEqual(x5Value, 1); // 4 < 5 = true
            Assert.AreEqual(x6Value, 1); // x1 != 0 -> true
            Assert.AreEqual(x7Value, 0); // x10 == 0 -> false
        }


        [Test]
        public void LogicalAddTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 4);
            var instand1 = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opOPand, 2, 3, 0);
            var program = instAddi1.Concat(instAddi2).Concat(instand1);

            core.Run(program);

            var x2Value = core.Register.ReadUnsignedInt(2);
            var x3Value = core.Register.ReadUnsignedInt(3);
            var x4Value = core.Register.ReadUnsignedInt(4);

            Assert.AreEqual(x2Value, 5);
            Assert.AreEqual(x3Value, 4);
            Assert.AreEqual(x4Value, 4);
        }

        [Test]
        public void LogicalOrTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 5);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 10);
            var instor1 = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opORor, 2, 3, 0);
            var program = instAddi1.Concat(instAddi2).Concat(instor1);

            core.Run(program);

            var x2Value = core.Register.ReadUnsignedInt(2);
            var x3Value = core.Register.ReadUnsignedInt(3);
            var x4Value = core.Register.ReadUnsignedInt(4);

            Assert.AreEqual(x2Value, 5);
            Assert.AreEqual(x3Value, 10);
            Assert.AreEqual(x4Value, 0x0F);
        }

        [Test]
        public void LogicalXorTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 1, 9);
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 1, 6);
            var instor1 = InstructionTypeFactory.CreateRType(Constant.opOP, 4, Constant.opOPxor, 2, 3, 0);
            var program = instAddi1.Concat(instAddi2).Concat(instor1);

            core.Run(program);

            var x2Value = core.Register.ReadUnsignedInt(2);
            var x3Value = core.Register.ReadUnsignedInt(3);
            var x4Value = core.Register.ReadUnsignedInt(4);

            Assert.AreEqual(x2Value, 9);
            Assert.AreEqual(x3Value, 6);
            Assert.AreEqual(x4Value, 0x0F);
        }

        [Test]
        public void SllTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 1, Constant.opOPIMMaddi, 0, 1);    // x1 = 1
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 0, 1);   // x2 = 1
            var instsll1 = InstructionTypeFactory.CreateRType(Constant.opOP, 3, Constant.opOPsll, 1, 2, 0); //  x1 left shift (x2)
            var program = instAddi1.Concat(instAddi2).Concat(instsll1);
            
            core.Run(program);

            var x1Value = core.Register.ReadSignedInt(1);
            var x2Value = core.Register.ReadSignedInt(2);
            var x3Value = core.Register.ReadSignedInt(3);


            Assert.AreEqual(x1Value, 1);
            Assert.AreEqual(x2Value, 1);
            Assert.AreEqual(x3Value, 2);
        }

        [Test]
        public void SllTest2()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 1, Constant.opOPIMMaddi, 0, 1);    // x1 = 1
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 0, 4);   // x2 = 1
            var instsll1 = InstructionTypeFactory.CreateRType(Constant.opOP, 3, Constant.opOPsll, 1, 2, 0); //  x1 left shift (x2)
            var program = instAddi1.Concat(instAddi2).Concat(instsll1);

            core.Run(program);

            var x1Value = core.Register.ReadSignedInt(1);
            var x2Value = core.Register.ReadSignedInt(2);
            var x3Value = core.Register.ReadSignedInt(3);


            Assert.AreEqual(x1Value, 1);
            Assert.AreEqual(x2Value, 4);
            Assert.AreEqual(x3Value, 0x10);

            //TODO:  More testing!
        }

        [Test]
        public void SrlTest1()
        {
            // Not tested by this test. Please go to Opcode04 test if this does not work as expected
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 1, Constant.opOPIMMaddi, 0, 0x10);    // x1 = 1
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMaddi, 0, 1);   // x2 = 1
            var instsll1 = InstructionTypeFactory.CreateRType(Constant.opOP, 3, Constant.opOPsrlsra, 1, 2, 0); //  x1 left shift (x2)
            var program = instAddi1.Concat(instAddi2).Concat(instsll1);

            core.Run(program);

            var x1Value = core.Register.ReadSignedInt(1);
            var x2Value = core.Register.ReadSignedInt(2);
            var x3Value = core.Register.ReadSignedInt(3);


            Assert.AreEqual(x1Value, 0x10);
            Assert.AreEqual(x2Value, 1);
            Assert.AreEqual(x3Value, 0x08);

            //TODO:  More testing!
        }

        [Test]
        public void SraTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 1, Constant.opOPIMMaddi, 0, 0x01); // x1 = 0 + 1;
            var instSll1 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 2, Constant.opOPIMMslli, 1, 0x1F);  //x2 = x1 << 32;
            var instAddi2 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 3, Constant.opOPIMMaddi, 2, 0x01); // x1 = 0 + 1;
            var instAddi3 = InstructionTypeFactory.CreateIType(Constant.opOPIMM, 4, Constant.opOPIMMaddi, 0, 0x01); // x1 = 0 + 1;
            var instsra1 = InstructionTypeFactory.CreateRType(Constant.opOP, 5, Constant.opOPsrlsra, 3, 4, Constant.opOPf2sra);


            // OK..  We have a 1 at the MSB and 1 at LSB.  x4 has the shift counter with 1

            var program = instAddi1.Concat(instSll1).Concat(instAddi2).Concat(instAddi3).Concat(instsra1);

            core.Run(program);

            var x1Value = core.Register.ReadBlock(1);
            var x2Value = core.Register.ReadBlock(2);
            var x3Value = core.Register.ReadBlock(3);
            var x4Value = core.Register.ReadBlock(4);
            var x5Value = core.Register.ReadBlock(5);

            Assert.AreEqual(x1Value, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2Value, new byte[] { 0x00, 0x00, 0x00, 0x80 });
            Assert.AreEqual(x3Value, new byte[] { 0x01, 0x00, 0x00, 0x80 });
            Assert.AreEqual(x4Value, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x5Value, new byte[] { 0x00, 0x00, 0x00, 0xC0 });

        }
    }
}
