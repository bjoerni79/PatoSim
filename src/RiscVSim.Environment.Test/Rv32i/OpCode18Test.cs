using NUnit.Framework;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class OpCode18Test
    {

        private BootstrapCore core;

        [SetUp]
        public void Setup()
        {
            core = new BootstrapCore();
        }

        /*
         *  beq     bimm12hi rs1 rs2 bimm12lo 14..12=0 6..2=0x18 1..0=3
            bne     bimm12hi rs1 rs2 bimm12lo 14..12=1 6..2=0x18 1..0=3
            blt     bimm12hi rs1 rs2 bimm12lo 14..12=4 6..2=0x18 1..0=3
            bge     bimm12hi rs1 rs2 bimm12lo 14..12=5 6..2=0x18 1..0=3
            bltu    bimm12hi rs1 rs2 bimm12lo 14..12=6 6..2=0x18 1..0=3
            bgeu    bimm12hi rs1 rs2 bimm12lo 14..12=7 6..2=0x18 1..0=3
         * 
         */

        [Test]
        public void Playground1()
        {

        }


        [Test]
        public void beqTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void bneTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void bltTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void bgeTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void bltuTest1()
        {
            TestHelper.NotImplementedYet();
        }

        [Test]
        public void bgeuTest1()
        {
            TestHelper.NotImplementedYet();
        }
    }

}
