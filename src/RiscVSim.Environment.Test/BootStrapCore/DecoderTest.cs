using NUnit.Framework;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.BootStrapCore
{
    public class DecoderTest
    {
        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }


        [Test]
        public void BootStrapCoreInstructionDetectionTest1()
        {
            var program = new byte[] { 0x00, 0x23, 0x01, 0xB3 }; // ADD (R-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count() == 1);

            var instruction1 = instructions.First();
            Assert.AreEqual(instruction1.OpCode, 0x0C);
            Assert.AreEqual(instruction1.RegisterDestination, 3);
            Assert.AreEqual(instruction1.Type, InstructionType.R_Type);
            Assert.IsFalse(instruction1.IsHint);

        }

        [Test]
        public void BootStrapCoreInstructionPayloadTest1()
        {
            var program = new byte[] { 0x00, 0x23, 0x01, 0xB3 }; // ADD (R-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count() == 1);
            var payload1 = payloads.First();

            // Verify instruction1  (ADD R-Type)
            Assert.AreEqual(payload1.Rd, 3);
            Assert.AreEqual(payload1.Rs1, 6);
            Assert.AreEqual(payload1.Rs2, 2);
            Assert.AreEqual(payload1.Funct3, 0);
            Assert.AreEqual(payload1.Funct7, 0);
        }

        [Test]
        public void BootStrapCoreInstructionDetectionTest2()
        {
            var program = new byte[] { 0x00, 0x23, 0x01, 0xB3, 0x00, 0x01, 0x00, 0x93 }; // ADD (R-Type) , ADDI (I-Type)

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count() == 2);

            var instruction1 = instructions.First();
            Assert.AreEqual(instruction1.OpCode, 0x0C);
            Assert.AreEqual(instruction1.RegisterDestination, 3);
            Assert.AreEqual(instruction1.Type, InstructionType.R_Type);
            Assert.IsFalse(instruction1.IsHint);

            var instruction2 = instructions.ElementAt(1);
            Assert.AreEqual(instruction2.OpCode, 0x04);
            Assert.AreEqual(instruction2.RegisterDestination, 1);
            Assert.AreEqual(instruction2.Type, InstructionType.I_Type);
            Assert.IsFalse(instruction2.IsHint);
        }

        [Test]
        public void BootStrapCoreInstructionPayloadTest2()
        {
            var program = new byte[] { 0x00, 0x23, 0x01, 0xB3, 0x00, 0x81, 0x00, 0x93 }; // ADD (R-Type) , ADDI (I-Type)

            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count() == 2);

            // Verify instruction1  (ADD R-Type)
            var payload1 = payloads.First();
            Assert.AreEqual(payload1.Rd, 3);
            Assert.AreEqual(payload1.Rs1, 6);
            Assert.AreEqual(payload1.Rs2, 2);
            Assert.AreEqual(payload1.Funct3, 0);
            Assert.AreEqual(payload1.Funct7, 0);

            // Verify Instruction2 (ADDI, I-Type)
            var payload2 = payloads.ElementAt(1);
            Assert.AreEqual(payload2.Rd, 1);
            Assert.AreEqual(payload2.Funct3, 0);
            Assert.AreEqual(payload2.Rs1, 2);
            Assert.AreEqual(payload2.SignedImmediate,8);
        }

        [Test]
        public void BootStrapCoreInstructionDetectionTest3()
        {
            var program = new byte[] {0x00,0x00,0x10,0xB7 }; // LUI => U-Type

            core.Run(program);
            var instructions = core.InstructionsProcessed;

            Assert.IsTrue(instructions.Count() == 1);

            var ins1 = instructions.First();
            Assert.AreEqual(ins1.OpCode, 0x0D);
            Assert.AreEqual(ins1.Type, InstructionType.U_Type);
            Assert.AreEqual(ins1.RegisterDestination, 1);
        }

        [Test]
        public void BootStrapCoreInstructionPayloadTest3()
        {
            var program = new byte[] 
            { 
                0x00, 0x00, 0x10, 0xB7, 
                0x00, 0x00, 0x20, 0xB7,
                0x00, 0x01, 0x00, 0xB7,
                0x0F, 0xFF, 0xF0, 0xB7,
                0xFF, 0xFF, 0xF0, 0xB7
            }; 


            core.Run(program);
            var payloads = core.InstructionPayloads;

            Assert.IsTrue(payloads.Count() == 5);

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
    }
}
