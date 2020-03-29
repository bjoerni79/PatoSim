using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    /// <summary>
    /// Tests all functions of OpCode = 0x04 (addi...)
    /// </summary>
    public class Opcode04Test
    {
        private BootstrapCore32 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
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

        [Test]
        public void AddITest2()
        {
            var program = new List<byte>();

            // addi :  rd = rd-1 + 1
            for (uint rd = 1; rd < 32; rd++)
            {
                var instruction = InstructionTypeFactory.CreateIType(C.OPIMM, rd, C.opOPIMMaddi, rd - 1, 1);
                program.AddRange(instruction);
            }

            core.Run(program);

            for (int registerIndex = 0; registerIndex < 32; registerIndex++)
            {
                var value = core.Register.ReadUnsignedInt(registerIndex);
                Assert.AreEqual(value, registerIndex);
            }

        }

        [Test]
        public void AddITest3()
        {
            //var program = new byte[] { 0x00, 0x81, 0x00, 0x93,  };
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 2, 0x7FF); // addi : rd(1) = rs(2) + 0x7FF


            var program = inst1;
            core.Run(program);

            var x1Content = core.Register.ReadUnsignedInt(1);
            var x3Content = core.Register.ReadUnsignedInt(3);
            Assert.AreEqual(x1Content, 0x7FF);
        }

        [Test]
        public void AddITest4()
        {
            //var program = new byte[] { 0x00, 0x81, 0x00, 0x93,  };
            var inst1 = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 2, 0x7FF); // addi : rd(1) = rs(2) + 0x7FF
            var inst2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMaddi, 1, 0xFFF); // addi : rd(1) = rs(2) + 0x7FF


            var program = inst1.Concat(inst2);
            core.Run(program);

            var x1Content = core.Register.ReadSignedInt(1);
            var x3Content = core.Register.ReadSignedInt(3);
            Assert.AreEqual(x1Content, 0x7FF);
            Assert.AreEqual(x3Content, 0x7FE);  // 0x7FF -1  = 0x7FE
        }

        [Test]
        public void SltiTest1()
        {
            var instAddi  = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 5); // x1 = 0 + 5;
            var instSlti1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMslti, 1, 4); // x2 = x1 < 4 ? 
            var instSlti2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMslti, 1, 5); // x3 = x1 < 5 ?
            var instSlti3 = InstructionTypeFactory.CreateIType(C.OPIMM, 4, C.opOPIMMslti, 1, 6); // x4 = x1 < 6 ?

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSlti1);
            program.AddRange(instSlti2);
            program.AddRange(instSlti3);

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadSignedInt(1);
            var x2 = register.ReadSignedInt(2);
            var x3 = register.ReadSignedInt(3);
            var x4 = register.ReadSignedInt(4);

            Assert.AreEqual(x1, 5);
            Assert.AreEqual(x2, 0); // 5 < 4 => 0
            Assert.AreEqual(x3, 0); // 5 < 5 => 0
            Assert.AreEqual(x4, 1); // 5 < 6 => 1

        }

        [Test]
        public void SltiuTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 5); // x1 = 0 + 5;
            var instSlti1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMsltiu, 1, 4); // x2 = x1 < 4 ? 
            var instSlti2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMsltiu, 1, 5); // x3 = x1 < 5 ?
            var instSlti3 = InstructionTypeFactory.CreateIType(C.OPIMM, 4, C.opOPIMMsltiu, 1, 6); // x4 = x1 < 6 ?

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSlti1);
            program.AddRange(instSlti2);
            program.AddRange(instSlti3);

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadSignedInt(1);
            var x2 = register.ReadSignedInt(2);
            var x3 = register.ReadSignedInt(3);
            var x4 = register.ReadSignedInt(4);

            Assert.AreEqual(x1, 5);
            Assert.AreEqual(x2, 0); // 5 < 4 => 0
            Assert.AreEqual(x3, 0); // 5 < 5 => 0
            Assert.AreEqual(x4, 1); // 5 < 6 => 1
        }


        [Test]
        public void SltiuTest2()
        {
            //
            //  Tests the SEQZ rd,rs function (...if Immediate = 1...)
            //

            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 5); // x1 = 0 + 5;
            var instSlti1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMsltiu, 1, 1); // assembler pseudoinstruction SEQZ rd, rs
            var instSlti2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMsltiu, 0, 1); // assembler pseudoinstruction SEQZ rd, rs

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSlti1);
            program.AddRange(instSlti2);

            core.Run(program);

            var register = core.Register;
            var x1 = register.ReadSignedInt(1);
            var x2 = register.ReadSignedInt(2);
            var x3 = register.ReadSignedInt(3);

            Assert.AreEqual(x1, 5);
            Assert.AreEqual(x2, 0); // X1 != 0 and I = 1 => 0
            Assert.AreEqual(x3, 1); // X0 = 0 and I = 1 => 1 

        }

        [Test]
        public void AndITest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x105); 
            var instAndi = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMandi, 1, 0x304);

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instAndi);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);

            Assert.AreEqual(x1Block, new byte[] { 0x05, 0x01, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x04, 0x01, 0x00, 0x00 });
        }

        [Test]
        public void OrITest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x105); 
            var instAndi = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMor, 1, 0x031A);

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instAndi);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);

            Assert.AreEqual(x1Block, new byte[] { 0x05, 0x01, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x1F, 0x03, 0x00, 0x00 });
        }

        [Test]
        public void XorITest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x43C); 
            var instAndi = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMxor, 1, 0x4C3);

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instAndi);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);

            Assert.AreEqual(x1Block, new byte[] { 0x3C, 0x04, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0xFF, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void XorIBitwiseComplementTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x05);
            var instAndi = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMxor, 1, 0x0F); 

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instAndi);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);

            Assert.AreEqual(x1Block, new byte[] { 0x05, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x0A, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void SlliTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x01);
            var instSlli1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMslli, 1, 0x01);  // Left Shift of x1 with 1
            var instSlli2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMslli, 1, 0x1F);  // Left shift of x1 with 0x1F = 31

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSlli1);
            program.AddRange(instSlli2);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);
            var x3Block = register.ReadBlock(3);

            Assert.AreEqual(x1Block, new byte[] { 0x01, 0x00, 0x00, 0x00 }); 
            Assert.AreEqual(x2Block, new byte[] { 0x02, 0x00, 0x00, 0x00 }); // Just a simple left shift
            Assert.AreEqual(x3Block, new byte[] { 0x00, 0x00, 0x00, 0x80 }); // Last bit of the register must have 1!
        }

        [Test]
        public void SrliTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x01);
            var instSll1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMslli, 1, 0x1F);  // Left shift of x1 with 0x1F = 31
            var instSrl1 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMsrlisrai, 2, 0x1F);  // And the same back to 01...

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSll1);
            program.AddRange(instSrl1);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);
            var x3Block = register.ReadBlock(3);

            Assert.AreEqual(x1Block, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x00, 0x00, 0x00, 0x80 }); // Shift the byte to the highest bit...
            Assert.AreEqual(x3Block, new byte[] { 0x01, 0x00, 0x00, 0x00 }); // ...and shift it back!
        }

        [Test]
        public void SraiTest1()
        {
            var instAddi = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x02);
            var instSrai = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMsrlisrai, 1, 0x401);

            var program = new List<byte>();
            program.AddRange(instAddi);
            program.AddRange(instSrai);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);

            Assert.AreEqual(x1Block, new byte[] { 0x02, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x01, 0x00, 0x00, 0x00 });

        }

        [Test]
        public void SraiTest2()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 1, C.opOPIMMaddi, 0, 0x01); // x1 = 0 + 1;
            var instSll1 = InstructionTypeFactory.CreateIType(C.OPIMM, 2, C.opOPIMMslli, 1, 0x1F);  //x2 = x1 << 32;
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 3, C.opOPIMMaddi, 2, 0x01); // x1 = 0 + 1;
            // OK..  We have a 1 at the MSB and 1 at LSB.  Run the two shift commands now.
            var instSrai1 = InstructionTypeFactory.CreateIType(C.OPIMM, 4, C.opOPIMMsrlisrai, 3, 0x01);  // slri (logical mode)
            var instSrai2 = InstructionTypeFactory.CreateIType(C.OPIMM, 5, C.opOPIMMsrlisrai, 3, 0x401); // srai (arithmetic mode)

            var program = new List<byte>();
            program.AddRange(instAddi1);
            program.AddRange(instSll1);
            program.AddRange(instAddi2);
            program.AddRange(instSrai1);
            program.AddRange(instSrai2);

            core.Run(program);

            var register = core.Register;
            var x1Block = register.ReadBlock(1);
            var x2Block = register.ReadBlock(2);
            var x3Block = register.ReadBlock(3);
            var x4Block = register.ReadBlock(4);
            var x5Block = register.ReadBlock(5);

            Assert.AreEqual(x1Block, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x2Block, new byte[] { 0x00, 0x00, 0x00, 0x80 });
            Assert.AreEqual(x3Block, new byte[] { 0x01, 0x00, 0x00, 0x80 }); // MSB = 1, LSB = 1  
            Assert.AreEqual(x4Block, new byte[] { 0x00, 0x00, 0x00, 0x40 }); // Right Shift byte 1 as logical mode
            Assert.AreEqual(x5Block, new byte[] { 0x00, 0x00, 0x00, 0xC0 }); // Right Shift byte 1 as arithmetic mode


        }
    }
}
