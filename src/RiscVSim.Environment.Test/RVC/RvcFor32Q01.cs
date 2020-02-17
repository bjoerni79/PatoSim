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
                Coding = te.ToBytes(0xFD, 0xBF),
                // 0x1B
                ExpectedPayload32 = te.BuildJType(0x1B,0,0x7FE)
            };

            te.Test(pairCj);

            // C.JAL
            var pairCjal = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCJ(1, 0x3FF, 1),
                Coding = te.ToBytes(0xFD, 0x3F),
                ExpectedPayload32 = te.BuildJType(0x1B, 1, 0x7FE)
            };

            te.Test(pairCjal);

            // C.BEQZ
            var pairBeqz = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1,1, 0xFF,6),
                Coding = te.ToBytes(0xFD, 0xDC),
                ExpectedPayload32 = te.BuildBType(0x18,9,0,0,0x1FE)
            };

            te.Test(pairBeqz);

            //// C.BNEZ
            var pairBnez = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1, 1, 0xFF, 7),
                Coding = te.ToBytes(0xFD, 0xFC),
                ExpectedPayload32 = te.BuildBType(0x18,9,0,1,0x1FE)
            };

            te.Test(pairBnez);
        }

        [Test]
        public void IntegerConstantGenerationTest()
        {
            // Test for positive numbers
            // C.LI Pos
            var pairLiPos = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x40),
                ExpectedPayload = te.LoadCI(1, 0x1F, 1, 2),
                ExpectedPayload32 = te.BuildIType(4, 1, 0, 0, 0x1F)
            };

            te.Test(pairLiPos);

            // C.LI Neg
            var pairLiNeg = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x50),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 2),
                ExpectedPayload32 = te.BuildIType(4,1,0,0,-31)
            };

            te.Test(pairLiNeg);


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
