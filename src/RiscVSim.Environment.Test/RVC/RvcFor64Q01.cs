using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor64Q01
    {
        private RvcTestEnvironment te;
        private int architecture = 64;

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
                ExpectedPayload = te.LoadCJ(1, 0x3FF, 5),
                Coding = te.ToBytes(0xFD, 0xBF)
            };

            te.Test(pairCj);

            // C.JAL (not testable on RV64I)
            //var pairCjal = new RvcTestPair(architecture,false)
            //{
            //    //ExpectedPayload = te.LoadCJ(1, 0x3FF, 1),
            //    Coding = te.ToBytes(0xFD, 0x3F)
            //};

            //te.Test(pairCjal);

            // C.BEQZ
            var pairBeqz = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1, 1, 0xFF, 6),
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
        public void IntegerContstantGenerationTest()
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
            var pairAddiw = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x30),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 1)
            };

            te.Test(pairAddiw);

            //C.ADDI16SP
            var pairAddi16sp = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x7D, 0x71),
                ExpectedPayload = te.LoadCI(1, 0x3F, 2, 3)
            };

            te.Test(pairAddi16sp);

            // C.SRLI
            var pairSrli = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x90),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x3F, 00, 4)
            };

            te.Test(pairSrli);

            // C.SRAI
            var pairSrai = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x94),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x3F, 01, 4)
            };

            te.Test(pairSrai);

            // C.ANDI
            var pairAndi = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x98),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x3F, 02, 4)
            };

            te.Test(pairAndi);



            // C.AND with 11 (Reserved, but good for testing and skip
            var pairCA11 = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x9C),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x3F, 03, 4)
            };

            te.Test(pairCA11);
        }

    }
}
