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

        private IRvcComposer composer32;
        private IRvcComposer composer64;

        public RvcTestEnvironment()
        {
            decoder32 = new RvcDecoder(Architecture.Rv32I);
            decoder64 = new RvcDecoder(Architecture.Rv64I);

            composer32 = new Rv32I.RvcComposer32();
            composer64 = new Rv64I.RvcComposer64();

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

        public RvcPayload LoadJCR(int op, int rs1, int rs2, int funct4, int f3)
        {
            var payload = new RvcPayload();
            payload.LoadCR(op, rs1, rs2, funct4, f3);

            return payload;
        }

        public RvcPayload LoadCB(int op, int rs1c, int imm, int funct3)
        {
            var payload = new RvcPayload();

            payload.LoadCB_Branch(1, imm, rs1c, funct3);
            return payload;
        }

        public RvcPayload LoadCB_Integer (int op, int rs1crdc, int imm, int f2, int f3)
        {
            var payload = new RvcPayload();

            payload.LoadCB_Integer(op, imm, rs1crdc, f2, f3);
            return payload;
        }

        public RvcPayload LoadCA(int op, int rs1crdc, int rs2c,int f2, int ca, int funct6)
        {
            var payload = new RvcPayload();

            var f3 = funct6 >>= 3;
            payload.LoadCA(op, rs2c, f2, ca, rs1crdc, funct6, f3);

            return payload;
        }

        public RvcPayload LoadCIW(int op, int rdc, int imm, int f3)
        {
            var payload = new RvcPayload();

            payload.LoadCIW(op, rdc, imm, f3);
            return payload;
        }

        public InstructionPayload BuildSType(int opcode, int f3, int rs1, int rs2, int imm)
        {
            var instruction = new Instruction(InstructionType.S_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);
            payload.Funct3 = f3;
            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.SignedImmediate = imm;

            return payload;
        }
        public InstructionPayload BuildIType(int opcode, int rd, int f3, int rs1, int imm)
        {
            var instruction = new Instruction(InstructionType.I_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);
            payload.Rd = rd;
            payload.Funct3 = f3;
            payload.Rs1 = rs1;
            payload.SignedImmediate = imm;


            return payload;
        }

        public InstructionPayload BuildIType_Unsigned(int opcode, int rd, int f3, int rs1, uint imm)
        {
            var instruction = new Instruction(InstructionType.I_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);
            payload.Rd = rd;
            payload.Funct3 = f3;
            payload.Rs1 = rs1;
            payload.UnsignedImmediate = imm;


            return payload;
        }

        public InstructionPayload BuildJType(int opcode, int rd, int imm)
        {
            var instruction = new Instruction(InstructionType.J_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);
            payload.Rd = rd;
            payload.SignedImmediate = imm;

            return payload;
        }

        public InstructionPayload BuildBType(int opcode, int rs1, int rs2, int f3, int immediate)
        {
            var instruction = new Instruction(InstructionType.B_Type, opcode,2);
            var payload = new InstructionPayload(instruction, null);

            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.Funct3 = f3;
            payload.SignedImmediate = immediate;
            return payload;
        }

        public InstructionPayload BuildUType(int opcode, int rd, uint uimmediate)
        {
            var instruction = new Instruction(InstructionType.U_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);

            payload.Rd = rd;
            payload.UnsignedImmediate = uimmediate;

            return payload;
        }

        public InstructionPayload BuildRType(int opcode, int rs1, int rs2, int rd, int f3, int f7)
        {
            var instruction = new Instruction(InstructionType.R_Type, opcode, 2);
            var payload = new InstructionPayload(instruction, null);

            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.Rd = rd;
            payload.Funct3 = f3;
            payload.Funct7 = f7;

            return payload;
        }

        public void Test(RvcTestPair pair)
        {
            RvcDecoder decoderUT = null;
            IRvcComposer compuserUT = null;

            if (pair.TargetArchitecture == Architecture.Rv32I)
            {
                decoderUT = decoder32;
                compuserUT = composer32;
            }

            if (pair.TargetArchitecture == Architecture.Rv64I)
            {
                decoderUT = decoder64;
                compuserUT = composer64;
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

            // If InstructionPayload is available generate and compare it via the composer
            if (pair.ExpectedPayload32 != null)
            {
                var payloadUT = decoderUT.Decode(pair.Coding);
                var instruction = compuserUT.ComposeInstruction(payloadUT);
                var instructionPayload = compuserUT.Compose(instruction, payloadUT);

                Assert.AreEqual(pair.ExpectedPayload32.OpCode, instructionPayload.OpCode);
                Assert.AreEqual(pair.ExpectedPayload32.Rd, instructionPayload.Rd);
                Assert.AreEqual(pair.ExpectedPayload32.Rs1, instructionPayload.Rs1);
                Assert.AreEqual(pair.ExpectedPayload32.Rs2, instructionPayload.Rs2);
                Assert.AreEqual(pair.ExpectedPayload32.Funct3, instructionPayload.Funct3);
                Assert.AreEqual(pair.ExpectedPayload32.Funct7, instructionPayload.Funct7);
                Assert.AreEqual(pair.ExpectedPayload32.SignedImmediate, instructionPayload.SignedImmediate);
                Assert.AreEqual(pair.ExpectedPayload32.UnsignedImmediate, instructionPayload.UnsignedImmediate);
                Assert.AreEqual(pair.ExpectedPayload32.SignedImmediateComplete, instructionPayload.SignedImmediateComplete);

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
