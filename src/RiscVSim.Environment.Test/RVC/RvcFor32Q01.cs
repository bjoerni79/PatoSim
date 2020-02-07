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
        public void IntegerComputationalTest()
        {

        }
    }
}
