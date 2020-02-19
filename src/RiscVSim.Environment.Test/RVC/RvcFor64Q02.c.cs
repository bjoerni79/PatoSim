using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor64Q02
    {
        private RvcTestEnvironment te;
        private int architecture = 64;

        [SetUp]
        public void Setup()
        {
            te = new RvcTestEnvironment();
        }

        [Test]
        public void LoadSpTest()
        {
            // C.LWSP
            var pairLwsp = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 2),
                Coding = new byte[] { 0xFE, 0x50 }
            };

            te.Test(pairLwsp);

            // C.LDSP (RV64 / 128) 
            var pairLdsp = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 3),
                Coding = new byte[] { 0xFE, 0x70 }
            };

            te.Test(pairLdsp);

            // C.LQSP



            // C.FLWSP



            // C.FLDSP
        }

        [Test]
        public void StoreSpTest()
        {
            // C.SWSP
            var pairSwSp = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCSS(2, 0x3F, 01, 6),
                Coding = te.ToBytes(0x86, 0xDF)
            };

            te.Test(pairSwSp);

            // C.SDSP
            var pairSdSp = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCSS(2, 0x3F, 01, 7),
                Coding = te.ToBytes(0x86, 0xFF)
            };

            te.Test(pairSdSp);

            // C.SQSP
            // C.FSWSP
            // C.FSDSP
        }

        [Test]
        public void ControlTransferInstructionTest()
        {

            // C.JR
            var pairCjr = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadJCR(2, 2, 1, 8, 4),
                Coding = te.ToBytes(0x06, 0x81)
            };

            te.Test(pairCjr);

            // C.JALR
            var pairCjalr = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadJCR(2, 2, 1, 9, 4),
                Coding = te.ToBytes(0x06, 0x91)
            };

            te.Test(pairCjalr);

        }

        [Test]
        public void IntegerRegisterImmediateTest()
        {
            // C.SLLI

            var pairSlli = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFE, 0x10),
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 0)
            };

            te.Test(pairSlli);
        }

        [Test]
        public void IntegerRegisterRegisterTest()
        {
            // C.MV
            var pairMv = new RvcTestPair(architecture)
            {
                // RS2 = 10, RD = 11
                Coding = te.ToBytes(0xAA, 0x85),
                ExpectedPayload = te.LoadJCR(2, 11, 10, 8, 4)
                //ExpectedPayload32 = te.BuildRType(0x0C, 0, 10, 11, 0, 0)
            };

            te.Test(pairMv);

            // C.ADD
            var pairAdd = new RvcTestPair(architecture)
            {
                // RS2 = 10, RD = 11
                Coding = te.ToBytes(0xAA, 0x95),
                ExpectedPayload = te.LoadJCR(2, 11, 10, 9, 4)
                //ExpectedPayload32 = te.BuildRType(0x0C, 11, 10, 11, 0, 0)

            };

            te.Test(pairAdd);
        }

        [Test]
        public void BreakpointIntrusionTest()
        {
            // C.EBREAK
            var pairBreak = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x02, 0x90),
                ExpectedPayload = te.LoadJCR(2, 0, 0, 9, 4)
                //ExpectedPayload32 = te.BuildIType_Unsigned(0x1C, 0, 0, 0, 1)
            };

            te.Test(pairBreak);
        }
    }
}
