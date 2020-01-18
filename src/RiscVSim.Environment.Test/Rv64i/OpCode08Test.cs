using NUnit.Framework;
using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv64i
{
    public class OpCode08Test
    {
        private BootstrapCore64 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore64();
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

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xAB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [Test]
        public void ShTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0x07CD);
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

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void SwTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0xFFF);
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

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x00, 0xF0, 0xFF, 0x0F, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0xF0, 0xFF, 0x0F, 0x00, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void SdTest1()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 200);
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0xFFF);
            var instLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 11, 0xFFFF);
            var instSlli1 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMslli, 11, 31);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsd, 10, 11, 100);
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADld, 10, 100);

            var program = new List<byte>();
            program.AddRange(instAddi1);    // x10 = x0 + 200
            program.AddRange(instAddi2);    // x11 = <Test Content>
            program.AddRange(instLui1);
            program.AddRange(instSlli1);
            program.AddRange(instStore1);   // STORE x11 to address 100 + x10
            program.AddRange(instLoad1);    // x12 = LOAD (x10 + 100)        


            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0xFF, 0x07 });
            Assert.AreEqual(x12, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0xFF, 0x07 });
        }

        [Test]
        public void ShWithMaxPositiveImmedateValueTest()
        {
            var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 0x7FF);   // 2K = 2047 = 0x7FF;
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0x07CD);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsh, 10, 11, 0x7FF); // 2K = 2047 = 0x7FF;
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADlhu, 10, 0x7FF);   // FFE = 7FF + 7FF = 4KB

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

            Assert.AreEqual(x10, new byte[] { 0xFF, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        //TODO: -2K Byte Jump Test

        [Test]
        public void ShWithMaxNegativeImmedateValueTest()
        {
            int value = 2048 * -1;
            var bytes = BitConverter.GetBytes(value);



            var instLui1 = InstructionTypeFactory.CreateUType(C.OPLUI, 10, 0xFF);
            //var instAddi1 = InstructionTypeFactory.CreateIType(C.OPIMM, 10, C.opOPIMMaddi, 0, 0x7FF);   // 2K = 2047 = 0x7FF;
            var instAddi2 = InstructionTypeFactory.CreateIType(C.OPIMM, 11, C.opOPIMMaddi, 0, 0x07CD);
            var instStore1 = InstructionTypeFactory.CreateSType(C.OPSTORE, C.OPSTOREsh, 10, 11, 0xFFFFF8);
            var instLoad1 = InstructionTypeFactory.CreateIType(C.OPLOAD, 12, C.OPLOADlhu, 10, 0xFFFFF8);

            var program = new List<byte>();
            program.AddRange(instLui1);
            //program.AddRange(instAddi1);    // x10 = x0 + 200
            program.AddRange(instAddi2);    // x11 = <Test Content>
            program.AddRange(instStore1);   // STORE x11 to address 100 + x10
            program.AddRange(instLoad1);    // x12 = LOAD (x10 + 100)        


            core.Run(program);

            var register = core.Register;
            var x10 = register.ReadBlock(10);
            var x11 = register.ReadBlock(11);
            var x12 = register.ReadBlock(12);

            Assert.AreEqual(x10, new byte[] { 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x11, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Assert.AreEqual(x12, new byte[] { 0xCD, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }
    }
}
