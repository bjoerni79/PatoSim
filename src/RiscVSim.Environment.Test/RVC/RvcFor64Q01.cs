﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcFor64Q01
    {
        private RvcTestEnvironment te;
        private int architecture = 64;

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
                ExpectedPayload = te.LoadCJ(1, 0x3FF, 5),
                Coding = te.ToBytes(0xFD, 0xBF),
                ExpectedPayload32 = te.BuildJType(0x1B, 0, 0x7FE)
            };

            te.Test(pairCj);


            // C.BEQZ
            var pairBeqz = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1, 1, 0xFF, 6),
                Coding = te.ToBytes(0xFD, 0xDC),
                ExpectedPayload32 = te.BuildBType(0x18, 9, 0, 0, 0x1FE)
            };

            te.Test(pairBeqz);

            //// C.BNEZ
            var pairBnez = new RvcTestPair(architecture)
            {
                ExpectedPayload = te.LoadCB(1, 1, 0xFF, 7),
                Coding = te.ToBytes(0xFD, 0xFC),
                ExpectedPayload32 = te.BuildBType(0x18, 9, 0, 1, 0x1FE)
            };

            te.Test(pairBnez);
        }

        [Test]
        public void IntegerContstantGenerationTest()
        {
            // Test for positive numbers
            // C.LI Pos
            var pairLiPos = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x40),
                ExpectedPayload = te.LoadCI(1, 0x1F, 1, 2),
                ExpectedPayload32 = te.BuildIType(4, 1, 0, 0, 0x1F)
            };

            te.Test(pairLiPos);

            // C.LI Neg
            var pairLiNeg = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x50),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 2),
                ExpectedPayload32 = te.BuildIType(4, 1, 0, 0, -1)
            };

            te.Test(pairLiNeg);


            // C.LUI
            // Inst32 Opcode 0x0D
            var pairLui1 = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x70),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 3),
                ExpectedPayload32 = te.BuildUType(0x0D, 1, 0xFFFFF000)
            };

            te.Test(pairLui1);

            var pairLui2 = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x60),
                ExpectedPayload = te.LoadCI(1, 0x1F, 1, 3),
                ExpectedPayload32 = te.BuildUType(0x0D, 1, 0x1F000)
            };

            te.Test(pairLui2);

        }

        [Test]
        public void IntegerRegisterImmediateTest()
        {
            //C.ADDI
            var pairAddi = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x10),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 0),
                ExpectedPayload32 = te.BuildIType(0x04, 1, 0, 1, 0x3F)
            };

            te.Test(pairAddi);

            //C.ADDIW
            var pairAddiw = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x30),
                ExpectedPayload = te.LoadCI(1, 0x3F, 1, 1),
                ExpectedPayload32 = te.BuildIType(0x06,1,0,1,-1)
            };

            te.Test(pairAddiw);

            //C.ADDI16SP
            var pairAddi16sp = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x7D, 0x71),
                ExpectedPayload = te.LoadCI(1, 0x3F, 2, 3),
                ExpectedPayload32 = te.BuildIType(0x04, 2, 0, 2, 0x03F0)
            };

            te.Test(pairAddi16sp);

            // C.SRLI
            var pairSrli = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x80),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x1F, 00, 4),
                ExpectedPayload32 = te.BuildIType(0x04, 9, 5, 9, 0x1F)
            };

            te.Test(pairSrli);

            // C.SRAI
            var pairSrai = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x84),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x1F, 01, 4),
                ExpectedPayload32 = te.BuildIType(0x04, 9, 5, 9, 0x41F)
            };

            te.Test(pairSrai);


            // C.ANDI
            var pairAndi = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xFD, 0x98),
                ExpectedPayload = te.LoadCB_Integer(1, 1, 0x3F, 02, 4),
                ExpectedPayload32 = te.BuildIType(0x04, 9, 7, 9, -1)
            };

            te.Test(pairAndi);


            // add rd rs1 rs2 31..25 = 0  14..12 = 0 6..2 = 0x0C 1..0 = 3
            // sub rd rs1 rs2 31..25 = 32 14..12 = 0 6..2 = 0x0C 1..0 = 3
            // sll rd rs1 rs2 31..25 = 0  14..12 = 1 6..2 = 0x0C 1..0 = 3
            // slt rd rs1 rs2 31..25 = 0  14..12 = 2 6..2 = 0x0C 1..0 = 3
            // sltu rd rs1 rs2 31..25 = 0  14..12 = 3 6..2 = 0x0C 1..0 = 3
            // xor rd rs1 rs2 31..25 = 0  14..12 = 4 6..2 = 0x0C 1..0 = 3
            // srl rd rs1 rs2 31..25 = 0  14..12 = 5 6..2 = 0x0C 1..0 = 3
            // sra rd rs1 rs2 31..25 = 32 14..12 = 5 6..2 = 0x0C 1..0 = 3
            // or rd rs1 rs2 31..25 = 0  14..12 = 6 6..2 = 0x0C 1..0 = 3
            // and rd rs1 rs2 31..25 = 0  14..12 = 7 6..2 = 0x0C 1..0 = 3

            // C.SUB
            var pairCASub = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x89, 0x8C),
                ExpectedPayload = te.LoadCA(1, 1, 2, 0, 0, 0x23),
                ExpectedPayload32 = te.BuildRType(0x0C, 9, 10, 9, 0, 0x32)
            };

            te.Test(pairCASub);

            // C.XOR
            var pairCAXor = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xA9, 0x8C),
                ExpectedPayload = te.LoadCA(1, 1, 2, 1, 1, 0x23),
                ExpectedPayload32 = te.BuildRType(0x0C, 9, 10, 9, 4, 0)
            };

            te.Test(pairCAXor);

            // C.OR
            var pairCAor = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xC9, 0x8C),
                ExpectedPayload = te.LoadCA(1, 1, 2, 2, 2, 0x23),
                ExpectedPayload32 = te.BuildRType(0x0C, 9, 10, 9, 6, 0)
            };

            te.Test(pairCAor);

            // C.AND
            var pairCAand = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xE9, 0x8C),
                ExpectedPayload = te.LoadCA(1, 1, 2, 3, 3, 0x23),
                ExpectedPayload32 = te.BuildRType(0x0C, 9, 10, 9, 7, 0)
            };

            te.Test(pairCAand);

            // C.SUBW
            var pairSubW = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x89, 0x9D),
                ExpectedPayload = te.LoadCA(0x01, 3, 2, 3, 0, 0x27),
                ExpectedPayload32 = te.BuildRType(0x0E, 11, 10, 11, 0, 0x32)
            };

            te.Test(pairSubW);

            // C.ADDW
            var pairAddW = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0xA9, 0x9D),
                ExpectedPayload = te.LoadCA(0x01, 3, 2, 3, 1, 0x27),
                ExpectedPayload32 = te.BuildRType(0x0E, 11, 10, 11, 0, 0)
            };

            te.Test(pairAddW);
        }

        [Test]
        public void NopTest()
        {
            // C.NOP
            var pairNop = new RvcTestPair(architecture)
            {
                Coding = te.ToBytes(0x01, 0x00),
                ExpectedPayload = te.LoadCI(0x01, 0, 0, 0),
                ExpectedPayload32 = te.BuildIType(0x04, 0, 0, 0, 0)
            };

            te.Test(pairNop);
        }

    }
}
