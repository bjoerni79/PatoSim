﻿using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartEnvironment : IHartEnvironment
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IRegister register;
        private ICsrRegister csrRegister;
        private IMemory memory;
        private Architecture architecture;

        private string[] registerNames32 = new string[]
{
            "x0","ra","sp","gp","tp","t0","t1","t2",
            "s0","s1", "a0","a1","a2","a3","a4","a5",
            "a6","a7","s2","s3","s4","s5","s6","s7",
            "s8","s9","s10","s11","t3","t4","t5","t6",
            "pc"
};

        internal HartEnvironment(Architecture architecture, IRegister register, IMemory memory, ICsrRegister csrRegister)
        {
            this.architecture = architecture;
            this.register = register;
            this.csrRegister = csrRegister;
            this.memory = memory;
        }

        public int NopCounter { get; private set; }

        public string GetMemoryState()
        {
            var sb = new StringBuilder();
            uint offset = 127;
            uint curIndex;

            sb.AppendLine("Memory in Little Endian:");
            int blockCount;
            int blockIndex;

            if (architecture == Architecture.Rv64I)
            {
                blockCount = 1;
                blockIndex = 0;

                var globalPointer64 = register.ReadUnsignedLong(3);
                for (curIndex = 0; curIndex < offset; curIndex += 8)
                {
                    var pos = Convert.ToUInt64(globalPointer64 + curIndex);
                    var data = memory.GetDoubleWord(globalPointer64 + curIndex);

                    if (blockIndex == 0)
                    {
                        sb.AppendFormat("{0:X16} : ", pos);
                    }

                    sb.AppendFormat("{0}    ", BitConverter.ToString(data.ToArray(), 0));

                    if (blockIndex < blockCount)
                    {
                        blockIndex++;
                    }
                    else
                    {
                        sb.AppendLine();
                        blockIndex = 0;
                    }
                }
            }
            else
            {
                blockCount = 3;
                blockIndex = 0;

                var globalPointer32 = register.ReadUnsignedInt(3);
                for (curIndex = 0; curIndex < offset; curIndex += 4)
                {
                    var pos = globalPointer32 + curIndex;
                    var data = memory.GetWord(globalPointer32 + curIndex);

                    if (blockIndex == 0)
                    {
                        sb.AppendFormat("{0:X8} : ", pos);
                    }

                    sb.AppendFormat("{0}    ", BitConverter.ToString(data.ToArray(), 0));

                    if (blockIndex < blockCount)
                    {
                        blockIndex++;
                    }
                    else
                    {
                        sb.AppendLine();
                        blockIndex = 0;
                    }
                }
            }

            return sb.ToString(); ;
        }

        public string GetRegisterStates()
        {
            var sb = new StringBuilder();

            string formatString;
            string formatString_Changed;
            if (architecture == Architecture.Rv64I)
            {
                // Show the register with 4 Byte lengths
                formatString = " {0} = {1:X16}\t";
                formatString_Changed = "!{0} = {1:X16}\t";
            }
            else
            {
                // Show the register with 8 Byte lengths
                formatString = " {0} = {1:X8}\t";
                formatString_Changed = "!{0} = {1:X8}\t";
            }

            sb.AppendLine("# Register States");
            int blockCount = 0;
            int registerLength = GetRegisterCount();
            for (int index = 0; index <= registerLength; index++)
            {
                var value = register.ReadUnsignedInt(index);

                if (value == 0)
                {
                    sb.AppendFormat(formatString, registerNames32[index], value);
                }
                else
                {
                    //TODO: Highlight this somehow...
                    sb.AppendFormat(formatString_Changed, registerNames32[index], value);
                }


                // Write 4 registers in a row.
                if (blockCount == 3)
                {
                    sb.AppendLine();
                    blockCount = 0;
                }
                else
                {
                    blockCount++;
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }

        public void IncreaseNopCounter()
        {
            NopCounter++;
        }

        public void RvConsoleAction(InstructionPayload payload)
        {
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

            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var rs2 = payload.Rs2;
            var f3 = payload.Funct3;
            var f7 = payload.Funct7;

            var rs1ValueSigned = register.ReadSignedInt(rs1);
            var rs1ValueUnsigned = register.ReadSignedInt(rs1);

            int result = 0;

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
                Console.WriteLine("Enter dec number : ");
                var decValue = Console.ReadLine();

                result = Int32.Parse(decValue);
                register.WriteSignedInt(rd, result);
            }

            if (f7 == 0x08)
            {
                // hin   reg   input hex number from keyboard into reg 
                Console.WriteLine("Enter hex number : ");
                var hexValue = Console.ReadLine();

                result = Int32.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                register.WriteSignedInt(rd, result);
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
                var register = GetRegisterStates();
                Console.WriteLine(register);
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
        }


        private int GetRegisterCount()
        {
            if (architecture == Architecture.Rv32E)
            {
                return 16;
            }

            return 32;
        }


    }
}