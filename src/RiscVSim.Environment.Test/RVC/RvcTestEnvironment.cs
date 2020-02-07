using NUnit.Framework;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcTestEnvironment
    {
        private RvcDecoder decoder32;
        private RvcDecoder decoder64;

        public RvcTestEnvironment()
        {
            decoder32 = new RvcDecoder(Architecture.Rv32I);
            decoder64 = new RvcDecoder(Architecture.Rv64I);
        }

        public RvcPayload LoadCI(int op, int imm, int rd, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCI(op, imm, rd, f3);

            return payload;
        }

        public RvcPayload LoadCSS(int op, int imm, int rs2, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCSS(op, rs2, imm, f3);

            return payload;
        }

        public RvcPayload LoadCL(int op, int rdc, int imm, int rs1c, int funct3)
        {
            var payload = new RvcPayload();
            payload.LoadCL(op, rdc, imm, rs1c, funct3);

            return payload;
        }

        public RvcPayload LoadCS(int op, int rs2c, int imm, int rs1c, int funct3)
        {
            var payload = new RvcPayload();
            payload.LoadCS(op, rs2c, imm, rs1c, funct3);

            return payload;
        }

        public RvcPayload LoadCJ(int op, int offset, int f3)
        {
            var payload = new RvcPayload();

            payload.LoadCJ(op, offset, f3);
            return payload;
        }

        public RvcPayload LoadJCR(int op, int rs1, int rs2, int funct4)
        {
            var payload = new RvcPayload();
            payload.LoadCR(op, rs1, rs2, funct4);

            return payload;
        }

        public RvcPayload LoadCB(int op, int rs1c, int imm, int funct3)
        {
            var payload = new RvcPayload();

            payload.LoadCB(1, imm, rs1c, funct3);
            return payload;
        }

        public RvcPayload LoadCLI(int op, int rd, int imm, int f3)
        {
            var payload = new RvcPayload();

            //payload.LoadCLI( )
            return payload;
        }

        public void Test(RvcTestPair pair)
        {
            RvcDecoder decoderUT = null;
            if (pair.TargetArchitecture == Architecture.Rv32I)
            {
                decoderUT = decoder32;
            }

            if (pair.TargetArchitecture == Architecture.Rv64I)
            {
                decoderUT = decoder64;
            }

            if (pair.IsValid)
            {
                var payloadUT = decoderUT.Decode(pair.Coding);
                Assert.AreEqual(payloadUT.Type, pair.ExpectedPayload.Type);
                Assert.AreEqual(payloadUT.Op, pair.ExpectedPayload.Op);
                Assert.AreEqual(payloadUT.Funct3, pair.ExpectedPayload.Funct3);
                Assert.AreEqual(payloadUT.Rd, pair.ExpectedPayload.Rd);
                Assert.AreEqual(payloadUT.Rs1, pair.ExpectedPayload.Rs1);
                Assert.AreEqual(payloadUT.Rs2, pair.ExpectedPayload.Rs2);
                Assert.AreEqual(payloadUT.Immediate, pair.ExpectedPayload.Immediate);
            }
            else
            {
                bool excpeptionCaught = false;
                try
                {
                    var payload = decoderUT.Decode(pair.Coding);
                }
                catch (RiscVSimException)
                {
                    excpeptionCaught = true;
                }
                catch (System.Exception ex)
                {
                    Assert.Fail("Invalid exception caught!");
                }

                Assert.IsTrue(excpeptionCaught,"Invalid opcode for this architecture!");

            }
        }

        public IEnumerable<byte> ToBytes(byte b0,byte b1)
        {
            var coding = new List<byte>();
            coding.Add(b0);
            coding.Add(b1);
            return coding;
        }
    }
}
