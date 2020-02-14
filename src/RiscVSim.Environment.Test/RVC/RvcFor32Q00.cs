using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor32Q00
    {
        private RvcTestEnvironment te;
        private int architecture = 32;

        [SetUp]
        public void Setup()
        {
            te = new RvcTestEnvironment();
        }

        [Test]
        public void LoadRegisterTest()
        {
            // C.LW
            var pairLw = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCL(00, 1, 0x1F, 2, 2),
                Coding = te.ToBytes(0x64, 0x5D),
                ExpectedPayload32 = te.BuildIType(00,9,2,10,0x7C)
            };

            te.Test(pairLw);

            // C.LD
            var pairLd = new RvcTestPair(architecture,false)
            {
                //ExpectedPayload = te.LoadCL(00, 1, 0x1F, 2, 3),
                Coding = te.ToBytes(0x64, 0x7D)
            };

            te.Test(pairLd);

        }

        [Test]
        public void StoreRegisterTest()
        {
            // C.SW
            var pairLw = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCS(00, 1, 0x1F, 2, 6),
                Coding = te.ToBytes(0x64, 0xDD),
                ExpectedPayload32 = te.BuildSType(8, 2, 10, 9, 0x7C)
            };

            te.Test(pairLw);

            // C.SD
            var pairLd = new RvcTestPair(architecture, false)
            {
                //ExpectedPayload = te.LoadCS(00, 1, 0x1F, 2, 7),
                Coding = te.ToBytes(0x64, 0xFD)
            };

            te.Test(pairLd);
        }

        [Test]
        public void IntegerRegisterImmediateTest()
        {
            var pairAddi4Spn = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCIW(0, 1, 0xFFFF, 0),
                Coding = te.ToBytes(0xE4, 0x1F),
                ExpectedPayload32 = te.BuildIType(4,9,0,2,0x3FC)
            };

            te.Test(pairAddi4Spn);
        }
    }
}
