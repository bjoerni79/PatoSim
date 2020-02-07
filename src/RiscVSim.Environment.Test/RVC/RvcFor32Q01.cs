using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor32Q01
    {
        private RvcTestEnvironment te;
        private int architecture = 32;

        [SetUp]
        public void Setup()
        {
            te = new RvcTestEnvironment();
        }

        [Test]
        public void ControlTransferInstructionTest()
        {
            // C.J
            var pairCj = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCJ(1,0x3FF, 5),
                Coding = te.ToBytes(0xFD, 0xBF)
            };

            te.Test(pairCj);

            // C.JAL
            var pairCjal = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCJ(1, 0x3FF, 1),
                Coding = te.ToBytes(0xFD, 0x3F)
            };

            te.Test(pairCjal);

            // C.BEQZ
            var pairBeqz = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1,1, 0xFF,6),
                Coding = te.ToBytes(0xFD, 0xDC)
            };

            te.Test(pairBeqz);

            //// C.BNEZ
            var pairBnez = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1, 1, 0xFF, 7),
                Coding = te.ToBytes(0xFD, 0xFC)
            };

            te.Test(pairBnez);
        }

        [Test]
        public void IntegerConstantGenerationTest()
        {
            // C.LI
            var pairLi = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x50),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 2)
            };

            te.Test(pairLi);

            // C.LUI
            var pairLui = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x70),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 3)
            };

            te.Test(pairLui);
        }

        [Test]
        public void IntegerRegisterImmediateTest()
        {
            //C.ADDI
            var pairAddi = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x10),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 0)
            };

            te.Test(pairAddi);

            //C.ADDIW
            // Not testable on RV32I
            //var pairAddiw = new RvcTestPair(architecture,false)
            //{
            //    Coding = te.ToBytes(0xFD, 0x30),
            //    ExpectedPayload = te.LoadCI(1, 0x3F, 1, 1)
            //};

            //te.Test(pairAddiw);

            //C.ADDI16SP
            var pairAddi16sp = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x7D, 0x71),
                ExpectedPayload = te.LoadCI(1, 0x3F, 2, 3)
            };

            te.Test(pairAddi16sp);
        }
    }
}
