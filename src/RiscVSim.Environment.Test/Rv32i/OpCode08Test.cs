using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class OpCode08Test
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        [Test]
        public void SbTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0xAB);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsb, 10, 11, 100);
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADlbu, 10, 100);

            var program = new List<byte>();
            program.AddRange(instAddi1);    // x10 = x0 + 200
            program.AddRange(instAddi2);    // x11 = <Test Content>
            program.AddRange(instStore1);   // STORE x11 to address 100 + x10
            program.AddRange(instLoad1);    // x12 = LOAD (x10 + 100)        


            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xAB, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xAB, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void ShTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0x0BCD);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsh, 10, 11, 100);
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADlhu, 10, 100);

            var program = new List<byte>();
            program.AddRange(instAddi1);    // x10 = x0 + 200
            program.AddRange(instAddi2);    // x11 = <Test Content>
            program.AddRange(instStore1);   // STORE x11 to address 100 + x10
            program.AddRange(instLoad1);    // x12 = LOAD (x10 + 100)        


            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xCD, 0x0B, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xCD, 0x0B, 0x00, 0x00 });
        }

        [Test]
        public void SwTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0x0FFF);
            var instLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 11, 0xFFFF);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsw, 10, 11, 100);
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADlwu, 10, 100);

            var program = new List<byte>();
            program.AddRange(instAddi1);    // x10 = x0 + 200
            program.AddRange(instAddi2);    // x11 = <Test Content>
            program.AddRange(instLui1);
            program.AddRange(instStore1);   // STORE x11 to address 100 + x10
            program.AddRange(instLoad1);    // x12 = LOAD (x10 + 100)        


            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x00, 0xF0, 0xFF, 0x0F });
            Assert.AreEqual(x12, new byte[] { 0x00, 0xF0, 0xFF, 0x0F });
        }
    }
}
