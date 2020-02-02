using NUnit.Framework;
using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcQ00Test
    {
        private RvcDecoder decoder32;
        private RvcDecoder decoder64;

        [SetUp]
        public void Setup()
        {
            decoder32 = new RvcDecoder(Architecture.Rv32I);
            decoder64 = new RvcDecoder(Architecture.Rv64I);
        }

        [Test]
        public void PlaygroundTest()
        {
            var pair = new RvcTestPair()
            {
                Target = Architecture.Rv32I,
                Coding = new byte[] { 0x01, 0x00 },
                ExpectedPayload = LoadCI(0x01)
            };

            Test(pair);
        }

        private RvcPayload LoadCI(int op)
        {
            var payload = new RvcPayload();
            payload.LoadCI(op);

            return payload;
        }

        private void Test(RvcTestPair pair)
        {
            RvcDecoder decoderUT = null;

            if (pair.Target== Architecture.Rv64I)
            {
                decoderUT = decoder64;
            }

            if (pair.Target==Architecture.Rv32I)
            {
                decoderUT = decoder32;
            }

            var payloadUT = decoderUT.Decode(pair.Coding);

            Assert.AreEqual(payloadUT.Type, pair.ExpectedPayload.Type);
            Assert.AreEqual(payloadUT.Op, pair.ExpectedPayload.Op);
            //TODO!
        }
    }
}
