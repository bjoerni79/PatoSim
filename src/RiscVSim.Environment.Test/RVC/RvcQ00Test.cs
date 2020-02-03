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
            var pair = new RvcTestPair()
            {
                Target = Architecture.Rv32I,
                Coding = new byte[] { 0xFE, 0x50 },
                ExpectedPayload = LoadCI(2, 0x3F, 1, 2)
            };

            Test(pair);
        }

        private RvcPayload LoadCI(int op, int imm, int rd, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCI(op, imm, rd, f3);
            
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
