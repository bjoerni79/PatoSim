using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test
{
    public static class Constant
    {
        //
        //  OpCode OP-IMM = 04
        //
        public const uint opOPIMM = 4;
        public const uint opOPIMMaddi = 0;
        public const uint opOPIMMslli = 1;
        public const uint opOPIMMslti = 2;
        public const uint opOPIMMsltiu = 3;
        public const uint opOPIMMxor = 4;
        public const uint opOPIMMsrlisrai = 5;
        public const uint opOPIMMor = 6;
        public const uint opOPIMMandi = 7;

        //
        //  Opcode OP = 0C
        //
        public const uint opOP = 0x0C;
        public const uint opOPaddAndSub = 0;
        public const uint opOPsll = 1;
        public const uint opOPslt = 2;
        public const uint opOPsltu = 3;
        public const uint opOPxor = 4;
        public const uint opOPsrlsra = 5;
        public const uint opORor = 6;
        public const uint opOPand = 7;


        public const uint opOPf7Add = 0;
        public const uint opOPf7Sub = 0x20;
        public const uint opOPf2sra = 0x20;

        //
        //  OpCode OP = 05
        //
        public const int opOPAUIPC = 0x5;

        //
        // OpCode OP = 0D
        //
        public const int opOPLUI = 0x0D;

        //
        // OpCode OP = 19
        //
        public const int OPJALR = 0x19;
    }
}
