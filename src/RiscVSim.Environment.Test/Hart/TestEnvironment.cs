using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Hart
{
    public class TestEnvironment
    {
        public TestEnvironment()
        {

        }

        public void LoadTest1(IHart hart)
        {
            /*
             *  / ! = Start Program Counter 
                / # = Program Counter Space for content (data and instruction)
                ! 100
                # 100
                / ADDI x10 = x0 + FF
                ;13 05 F0 0F
                / ADDI x11 = x0 + FF
                ;93 05 F0 0F
                / ADD x12 = x10 + x11
                ;33 06 B5 00
                // Nop
                nop
                / Jump to 200 using JALR direct jump
                ;E7 00 00 20
                / Addi x15 = 0 + 1
                ;93 07 10 00


                # 200
                / ADDI x14 = x0 + 1
                ;13 07 23 00
                / JALR rd=0, rs1 = 1 => POP
                ;67 80 00 00
             * 
             */

            var blockAddress100 = new List<byte>()
            {
                0x13, 0x05, 0xF0, 0x0F,
                0x93, 0x05, 0xF0, 0x0F,
                0x33, 0x06, 0xB5, 0x00,
                0xE7, 0x00, 0x00, 0x20,
                0x93, 0x07, 0x10, 0x00
            };

            var blockAddress200 = new List<byte>()
            {
                0x13, 0x07, 0x23, 0x00,
                0x67, 0x80, 0x00, 0x00
            };

            hart.Load(0x100, blockAddress100);
            hart.Load(0x200, blockAddress200);

        }

        public void LoadRvc32Test1(IHart hart)
        {

        }


    }
}
