using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Custom
{
    public class RvOpcode : OpCodeCommand
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public RvOpcode(IMemory memory, IRegister register) : base(memory,register)
        {
        }

        public override int Opcode => 0x00;

        /*
         * The rv program supports the following non-RISC-V instructions:

                                                            funct7

           halt        halt                                             0
           nl          output newline to display                        1
           dout  reg   output reg in decimal to display                 2
           udout reg   output reg in unsigned decimal to display        3
           hout  reg   output reg in hex to display                     4
           aout  reg   output ASCII character in reg to display         5
           sout  reg   display string reg points to                     6
           din   reg   input dec number from keyboard into reg          7
           hin   reg   input hex number from keyboard into reg          8
           ain   reg   input character from keyboard into reg           9
           sin   reg   like sout but for input                          a
           m           display memory                                   b
           x           display registers                                c
           s           display stack                                    d
           bp          software breakpoint                              e
           ddout reg   doubleword (i.e., 64 bits) decimal out           f
           dudout reg  doubleword (i.e., 64 bits) unsigned dec out     10
           dhout reg   doubleword (i.e., 64 bits) hex out              11
         */

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var rs2 = payload.Rs2;
            var f3 = payload.Funct3;
            var f7 = payload.Funct7;

            var rs1ValueSigned = Register.ReadSignedInt(rs1);
            var rs1ValueUnsigned = Register.ReadSignedInt(rs1);

            Logger.Info("Opcode RV : rd = {rd}, rs1 = {rs1}, rs2 = {rs2}, f3 = {f3}, f7 = {f7}", rd, rs1, rs2, f3, f7);

            if (f7 == 0x01)
            {
                // nl          output newline to display 
                Console.WriteLine();
            }

            if (f7 == 0x02)
            {
                // dout  reg   output reg in decimal to display
                Console.Write(rs1ValueSigned);
            }

            if (f7 == 0x03)
            {
                // udout reg   output reg in unsigned decimal to display
                Console.Write(rs1ValueUnsigned);
            }

            if (f7 == 0x04)
            {
                // hout  reg   output reg in hex to display   
                Console.WriteLine("{0:X}", rs1ValueUnsigned);
            }

            if (f7 == 0x05)
            {
                // aout  reg   output ASCII character in reg to display
            }

            if (f7 == 0x06)
            {
                // sout reg   display string reg points to
            }

            if (f7 == 0x07)
            {
                // din   reg   input dec number from keyboard into reg 
            }

            if (f7 == 0x08)
            {
                // hin   reg   input hex number from keyboard into reg 
            }

            if (f7 == 0x09)
            {
                // ain   reg   input character from keyboard into reg
            }

            if (f7 == 0x0A)
            {
                // sin   reg   like sout but for input 
            }

            if (f7 == 0x0B)
            {
                // m           display memory       
            }

            if (f7 == 0x0C)
            {
                // x           display registers
            }

            if (f7 == 0x0D)
            {
                // s           display stack      
            }

            if (f7 == 0x0E)
            {
                // bp          software breakpoint
            }

            if (f7 == 0x0F)
            {
                // ddout reg   doubleword (i.e., 64 bits) decimal out 
            }

            if (f7 == 0x10)
            {
                // dudout reg  doubleword (i.e., 64 bits) unsigned dec out 
            }

            if (f7 == 0x11)
            {
                // dhout reg   doubleword (i.e., 64 bits) hex out     
            }

            return true;
        }
    }
}
