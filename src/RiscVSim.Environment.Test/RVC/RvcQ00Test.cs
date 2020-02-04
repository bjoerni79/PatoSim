using NUnit.Framework;
using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcQ00Test
    {
        private RvcDecoder decoder;

        [SetUp]
        public void Setup()
        {
            decoder = new RvcDecoder();
        }

        [Test]
        public void PlaygroundTest()
        {

        }

        [Test]
        public void TestQ10Load()
        {
            var pair010 = new RvcTestPair()
            {
                Coding = new byte[] { 0xFE, 0x50 },
                ExpectedPayload = LoadCI(2, 0x3F, 1, 2)
            };

            var pair001 = new RvcTestPair()
            {
                Coding = new byte[] { 0xFE, 0x30 },
                ExpectedPayload = LoadCI(2, 0x3F, 1, 1)
            };

            var pair011 = new RvcTestPair()
            {
                Coding = new byte[] { 0xFE, 0x70 },
                ExpectedPayload = LoadCI(2, 0x3F, 1, 3)
            };

            Test(pair001);
            Test(pair010);
            Test(pair011);
        }

        [Test]
        public void TestQ10Store()
        {
            var pair101 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xBF },
                ExpectedPayload = LoadCSS(2, 0x3F, 10, 5)
            };

            var pair110 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xDF },
                ExpectedPayload = LoadCSS(2, 0x3F, 10, 6)
            };

            var pair111 = new RvcTestPair()
            {
                Coding = new byte[] { 0xAA, 0xFF },
                ExpectedPayload = LoadCSS(2, 0x3F, 10, 7)
            };

            Test(pair101);
            Test(pair110);
            Test(pair111);
        }

        private RvcPayload LoadCI(int op, int imm, int rd, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCI(op, imm, rd, f3);
            
            return payload;
        }

        private RvcPayload LoadCSS(int op, int imm, int rs2, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCSS(op, rs2, imm, f3);

            return payload;
        }

        private void Test(RvcTestPair pair)
        {
            var payloadUT = decoder.Decode(pair.Coding);

            Assert.AreEqual(payloadUT.Type, pair.ExpectedPayload.Type);
            Assert.AreEqual(payloadUT.Op, pair.ExpectedPayload.Op);
            Assert.AreEqual(payloadUT.Funct3, pair.ExpectedPayload.Funct3);
            Assert.AreEqual(payloadUT.Rd, pair.ExpectedPayload.Rd);
            Assert.AreEqual(payloadUT.Rs1, pair.ExpectedPayload.Rs1);
            Assert.AreEqual(payloadUT.Rs2, pair.ExpectedPayload.Rs2);
            Assert.AreEqual(payloadUT.Immediate, pair.ExpectedPayload.Immediate);
        }
    }
}
