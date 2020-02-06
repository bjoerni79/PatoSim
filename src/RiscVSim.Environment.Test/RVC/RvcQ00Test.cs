using NUnit.Framework;
using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcQ00Test
    {
        private RvcTestEnvironment te;

        [SetUp]
        public void Setup()
        {
            te = new RvcTestEnvironment();
        }


        [Test]
        public void TestQ10Load()
        {
            var pair010 = new RvcTestPair(32)
            {
                Coding = new byte[] { 0xFE, 0x50 },
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 2)
            };

            var pair001 = new RvcTestPair(32)
            {
                Coding = new byte[] { 0xFE, 0x30 },
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 1)
            };

            var pair011 = new RvcTestPair(64)
            {
                Coding = new byte[] { 0xFE, 0x70 },
                ExpectedPayload = te.LoadCI(2, 0x3F, 1, 3)
            };

            te.Test(pair001);
            te.Test(pair010);
            te.Test(pair011);
        }

        [Test]
        public void TestQ10Store()
        {
            var pair101 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xBF },
                ExpectedPayload = te.LoadCSS(2, 0x3F, 10, 5)
            };

            var pair110 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xDF },
                ExpectedPayload = te.LoadCSS(2, 0x3F, 10, 6)
            };

            var pair111 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xFF },
                ExpectedPayload = te.LoadCSS(2, 0x3F, 10, 7)
            };

            te.Test(pair101);
            te.Test(pair110);
            te.Test(pair111);
        }


    }
}
