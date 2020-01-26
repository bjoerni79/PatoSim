using NUnit.Framework;
using RiscVSim.Environment.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class ExtensionMTest
    {
        private BootstrapCore32 core;

        /*
         * 
         * # RV32M
            mul     rd rs1 rs2 31..25=1 14..12=0 6..2=0x0C 1..0=3
            mulh    rd rs1 rs2 31..25=1 14..12=1 6..2=0x0C 1..0=3
            mulhsu  rd rs1 rs2 31..25=1 14..12=2 6..2=0x0C 1..0=3
            mulhu   rd rs1 rs2 31..25=1 14..12=3 6..2=0x0C 1..0=3
            div     rd rs1 rs2 31..25=1 14..12=4 6..2=0x0C 1..0=3
            divu    rd rs1 rs2 31..25=1 14..12=5 6..2=0x0C 1..0=3
            rem     rd rs1 rs2 31..25=1 14..12=6 6..2=0x0C 1..0=3
            remu    rd rs1 rs2 31..25=1 14..12=7 6..2=0x0C 1..0=3

            # RV64M
            mulw    rd rs1 rs2 31..25=1 14..12=0 6..2=0x0E 1..0=3
            divw    rd rs1 rs2 31..25=1 14..12=4 6..2=0x0E 1..0=3
            divuw   rd rs1 rs2 31..25=1 14..12=5 6..2=0x0E 1..0=3
            remw    rd rs1 rs2 31..25=1 14..12=6 6..2=0x0E 1..0=3
            remuw   rd rs1 rs2 31..25=1 14..12=7 6..2=0x0E 1..0=3
         * 
         */

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore32();
        }

        [Test]
        public void MulTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void MulhTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void MulhsuTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void MulhuTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void DivTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void DivuTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void RemTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void RemuTest1()
        {
            TestHelper.NotImplementedYet();
        }
    }
}
