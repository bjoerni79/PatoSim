using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test
{
    public static class C
    {
        //
        //  OpCode OP-IMM = 04
        //
        public const uint OPIMM = 4;
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
        public const uint OPOP = 0x0C;
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
        public const int OPAUIPC = 0x5;

        //
        // OpCode OP = 0D
        //
        public const int OPLUI = 0x0D;

        //
        // OpCode OP = 19
        //
        public const int OPJALR = 0x19;

        // OpCode OP = 18
        public const int OPB = 0x18;
        public const int OPBbeq = 0;
        public const int OPBbne = 1;
        public const int OPBblt = 4;
        public const int OPBbge = 5;
        public const int OPBbltu = 6;
        public const int OPBbgeu = 7;

        // Load, Opcode = 0
        public const int OPLOAD = 0;
        public const int OPLOADlh = 1;
        public const int OPLOADlw = 2;
        public const int OPLOADlhu = 5;
        public const int OPLOADlb = 0;
        public const int OPLOADlbu = 4;
        public const int OPLOADlwu = 6;
        public const int OPLOADld = 3;

        // Store, Opcode 08
        public const int OPSTORE = 0x08;
        public const int OPSTOREsb = 0;
        public const int OPSTOREsh = 1;
        public const int OPSTOREsw = 2;
        public const int OPSTOREsd = 3;

        // OP-IMM32, OpCode 06 (64 Bit)
        public const int OPIMM32 = 0x06;
        public const int OPIMM32_addiw = 0;
        public const int OPIMM32_slliw = 1;
        public const int OPIMM32_srliw_sraiw = 5;

        public const int OPSYSTEM = 0x1C;
    }
}
