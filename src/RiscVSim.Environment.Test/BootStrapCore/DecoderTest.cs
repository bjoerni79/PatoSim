using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using RiscVSim.Environment.Decoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.BootStrapCore
{
    public class DecoderTest
    {
        private BootstrapCore32 core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
        }


        /// <summary>
        /// Test for the R-Type detection test based on Opcode 0C
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionDetectionTest1()
        {
            //var program = new byte[] { 0x00, 0x23, 0x01, 0xB3 }; // ADD (R-Type)
            var program = new byte[] { 0xB3, 0x01, 0x23, 0x00 }; // ADD (R-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count == 1);

            var instruction1 = instructions.First();
            Assert.AreEqual(instruction1.OpCode, 0x0C);
            Assert.AreEqual(instruction1.Type, InstructionType.R_Type);
            Assert.AreEqual(instruction1.InstructionLength, 4);

        }

        /// <summary>
        /// Test for the R-Type decoding test based on Opcode 0C
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest1()
        {
            //var program = new byte[] { 0x00, 0x23, 0x01, 0xB3 }; // ADD (R-Type)
            var program = new byte[] { 0xB3, 0x01, 0x23, 0x00 }; // ADD (R-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);
            var payload1 = payloads.First();

            // Verify instruction1  (ADD R-Type)
            Assert.AreEqual(payload1.Rd, 3);
            Assert.AreEqual(payload1.Rs1, 6);
            Assert.AreEqual(payload1.Rs2, 2);
            Assert.AreEqual(payload1.Funct3, 0);
            Assert.AreEqual(payload1.Funct7, 0);
        }

        /// <summary>
        /// Test for the R-Type and I-Type detection test based on Opcode 0C and 04
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionDetectionTest2()
        {
            //var program = new byte[] { 0x00, 0x23, 0x01, 0xB3, 0x00, 0x01, 0x00, 0x93 }; // ADD (R-Type) , ADDI (I-Type)
            var program = new byte[] { 0x93, 0x00, 0x01, 0x00, 0xB3, 0x01, 0x23, 0x00 }; // ADD (R-Type) , ADDI (I-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count == 2);

            var instruction1 = instructions.ElementAt(0);
            Assert.AreEqual(instruction1.OpCode, 0x04);
            Assert.AreEqual(instruction1.Type, InstructionType.I_Type);

            var instruction2 = instructions.ElementAt(1);
            Assert.AreEqual(instruction2.OpCode, 0x0C);
            Assert.AreEqual(instruction2.Type, InstructionType.R_Type);


        }

        /// <summary>
        /// Test for the R-Type and I-Type decoding test based on Opcode 0C and 04
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest2()
        {
            //var program = new byte[] { 0x00, 0x23, 0x01, 0xB3, 0x00, 0x81, 0x00, 0x93 }; // ADD (R-Type) , ADDI (I-Type)
            var program = new byte[] {0x93, 0x00, 0x81, 0x00, 0xB3, 0x01, 0x23, 0x00 }; //  ADDI (I-Type), ADD (R-Type) ,

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 2);

            // Verify instruction1  (ADD R-Type)
            var payload1 = payloads.ElementAt(1);
            Assert.AreEqual(payload1.Rd, 3);
            Assert.AreEqual(payload1.Rs1, 6);
            Assert.AreEqual(payload1.Rs2, 2);
            Assert.AreEqual(payload1.Funct3, 0);
            Assert.AreEqual(payload1.Funct7, 0);

            // Verify Instruction2 (ADDI, I-Type)
            var payload2 = payloads.ElementAt(0);
            Assert.AreEqual(payload2.Rd, 1);
            Assert.AreEqual(payload2.Funct3, 0);
            Assert.AreEqual(payload2.Rs1, 2);
            Assert.AreEqual(payload2.SignedImmediate,8);
        }

        /// <summary>
        /// Test for the U-Type detection test based on LUI
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionDetectionTest3()
        {
            //var program = new byte[] {0x00,0x00,0x10,0xB7 }; // LUI => U-Type
            var program = new byte[] { 0xB7,0x10,0x00,0x00 }; // LUI => U-Type

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count == 1);

            var ins1 = instructions.First();
            Assert.AreEqual(ins1.OpCode, 0x0D);
            Assert.AreEqual(ins1.Type, InstructionType.U_Type);
        }

        /// <summary>
        /// Test for the U-Type decoding test based on LUI
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest3()
        {
            var program = new byte[]
{
                0xB7, 0x10, 0x00, 0x00,
                0xB7, 0x20, 0x00, 0x00,
                0xB7, 0x00, 0x01, 0x00,
                0xB7, 0xF0, 0xFF, 0x0F,
                0xB7, 0xF0, 0xFF, 0xFF
 
            };


            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 5);

            // Verify instruction 1
            var payload1 = payloads.First();
            Assert.AreEqual(payload1.Rd, 1);
            Assert.AreEqual(payload1.UnsignedImmediate, 1);

            // Verify instruction 2
            var payload2 = payloads.ElementAt(1);
            Assert.AreEqual(payload2.Rd, 1);
            Assert.AreEqual(payload2.UnsignedImmediate, 2);

            // Verify instruction 3
            var payload3 = payloads.ElementAt(2);
            Assert.AreEqual(payload3.Rd, 1);
            Assert.AreEqual(payload3.UnsignedImmediate, 0x010);

            // Verify instruction 4
            var payload4 = payloads.ElementAt(3);
            Assert.AreEqual(payload4.Rd, 1);
            Assert.AreEqual(payload4.UnsignedImmediate, 0x0FFFF);

            // Verify instruction 5
            var payload5 = payloads.ElementAt(4);
            Assert.AreEqual(payload5.Rd, 1);
            Assert.AreEqual(payload5.UnsignedImmediate, 0x000fffff);
        }

        /// <summary>
        /// B-Type detection test with BNE and OpCode 18
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionDetectionTest4()
        {
            var program = new byte[] { 0x63, 0x8F, 0x20, 0x00 };

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count == 1);

            var ins1 = instructions.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
        }

        /// <summary>
        /// B-Type payload test with BNE and OpCode 18
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest4()
        {
            var program = new byte[] { 0x63, 0x8F, 0x20, 0x00};

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0x1E);
        }

        /// <summary>
        /// B-Type payload test with BNE and OpCode 18
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest5()
        {
            var program = new byte[] { 0xE3, 0x8F, 0x20, 0x7E };

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0xFFE);
        }



        /// <summary>
        /// B-Type payload test with BNE and OpCode 18
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest6()
        {
            var program = new byte[] { 0xE3, 0x8F, 0x20, 0xFE };

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, -2);
        }


        /// <summary>
        /// B-Type payload test with BNE and OpCode 18. Same test like Nr.6 but here via the factory method
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest7()
        {
            //var program = new byte[] { 0xE3, 0x8F, 0x20, 0x7E };
            var program = InstructionTypeFactory.CreateBType(C.OPB, 1, 2, 0, 0xFFE);

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0xFFE);
        }

        /// <summary>
        /// B-Type payload test with BNE and OpCode 18. Same test like Nr.6 but here via the factory method
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest8()
        {
            //var program = new byte[] { 0xE3, 0x8F, 0x20, 0xFE };
            var program = InstructionTypeFactory.CreateBType(C.OPB, 1, 2, 0, 0xFFE);

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x18);
            Assert.AreEqual(ins1.Type, InstructionType.B_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0xFFE);
        }

        /// <summary>
        /// B-Type payload test with BNE and OpCode 18
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionDetectionTest9()
        {
            var program = new byte[] { 0xA3, 0x8F, 0x20, 0x7E };

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x08);
            Assert.AreEqual(ins1.Type, InstructionType.S_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0x7FF);
        }

        /// <summary>
        /// B-Type payload test with BNE and OpCode 18. Same test like Nr.6 but here via the factory method
        /// </summary>
        [Test]
        public void BootStrapCoreInstructionPayloadTest9()
        {
            var program = new byte[] { 0xA3, 0x8F, 0x20, 0x7E };

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count == 1);

            var ins1 = payloads.First();
            Assert.AreEqual(ins1.OpCode, 0x08);
            Assert.AreEqual(ins1.Type, InstructionType.S_Type);
            Assert.AreEqual(ins1.Rs1, 1);
            Assert.AreEqual(ins1.Rs2, 2);
            Assert.AreEqual(ins1.SignedImmediate, 0x7FF);
        }
    }
}
