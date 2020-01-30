﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Extensions
{
    internal class AtomicInstruction
    {
        private IMemory memory;
        private IRegister register;

        /*
     *  # RV32A
        amoadd.w    rd rs1 rs2      aqrl 31..29=0 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amoxor.w    rd rs1 rs2      aqrl 31..29=1 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amoor.w     rd rs1 rs2      aqrl 31..29=2 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amoand.w    rd rs1 rs2      aqrl 31..29=3 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amomin.w    rd rs1 rs2      aqrl 31..29=4 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amomax.w    rd rs1 rs2      aqrl 31..29=5 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amominu.w   rd rs1 rs2      aqrl 31..29=6 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amomaxu.w   rd rs1 rs2      aqrl 31..29=7 28..27=0 14..12=2 6..2=0x0B 1..0=3
        amoswap.w   rd rs1 rs2      aqrl 31..29=0 28..27=1 14..12=2 6..2=0x0B 1..0=3
        lr.w        rd rs1 24..20=0 aqrl 31..29=0 28..27=2 14..12=2 6..2=0x0B 1..0=3
        sc.w        rd rs1 rs2      aqrl 31..29=0 28..27=3 14..12=2 6..2=0x0B 1..0=3


        *  # RV64A
           amoadd.d    rd rs1 rs2      aqrl 31..29=0 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amoxor.d    rd rs1 rs2      aqrl 31..29=1 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amoor.d     rd rs1 rs2      aqrl 31..29=2 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amoand.d    rd rs1 rs2      aqrl 31..29=3 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amomin.d    rd rs1 rs2      aqrl 31..29=4 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amomax.d    rd rs1 rs2      aqrl 31..29=5 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amominu.d   rd rs1 rs2      aqrl 31..29=6 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amomaxu.d   rd rs1 rs2      aqrl 31..29=7 28..27=0 14..12=3 6..2=0x0B 1..0=3
           amoswap.d   rd rs1 rs2      aqrl 31..29=0 28..27=1 14..12=3 6..2=0x0B 1..0=3
          lr.d        rd rs1 24..20=0 aqrl 31..29=0 28..27=2 14..12=3 6..2=0x0B 1..0=3
          sc.d        rd rs1 rs2      aqrl 31..29=0 28..27=3 14..12=3 6..2=0x0B 1..0=3
     */

        private int amoadd = 0;
        private int amoxor = 0x04;
        private int amoor = 0x08;
        private int amoand = 0x0C;
        private int amomin = 0x10;
        private int amomax = 0x14;
        private int amominu = 0x18;
        private int amomaxu = 0x1C;
        private int amoswap = 0x01;
        private int lrw = 0x02;
        private int scw = 0x03;

        internal AtomicInstruction(IMemory memory, IRegister register)
        {
            this.memory = memory;
            this.register = register;
        }

        internal void ExecuteW (int rd, int rs1, int rs2, int release, int acquire, int f5)
        {
            // RV32I
        }

        internal void ExecuteD(int rd, int rs1, int rs2, int release, int acquire, int f5)
        {
            // RV64I
        }
    }
}
