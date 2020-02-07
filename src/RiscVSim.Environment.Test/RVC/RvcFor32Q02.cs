using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor32Q02
    {
        private RvcTestEnvironment te;
        private int architecture = 32;

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
                ExpectedPayload = te.LoadCI(2, 0x3F,1, 2),
                Coding = new byte[] { 0xFE, 0x50 }
            };

            te.Test(pairLwsp);

            // C.LDSP (RV64 / 128) 
            var pairLdsp = new RvcTestPair(architecture, false)
            {
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
            var pairSdSp = new RvcTestPair(architecture,false)
            {
                //ExpectedPayload = te.LoadCSS(2, 0x3F, 01, 7)
                Coding = te.ToBytes(0x86, 0xFF)
            };

            te.Test(pairSdSp);

            // C.SQSP
            // C.FSWSP
            // C.FSDSP
        }


    }
}
