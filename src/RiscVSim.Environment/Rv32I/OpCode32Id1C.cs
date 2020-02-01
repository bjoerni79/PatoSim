using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class OpCode32Id1C : OpCodeCommand
    {
        private ICsrRegister csrRegister;
        private IHartEnvironment environment;

        public OpCode32Id1C(IMemory memory, IRegister register, ICsrRegister csrRegister, IHartEnvironment environment) : base(memory,register)
        {
            this.csrRegister = csrRegister;
            this.environment = environment;
        }

        /*
         *  # SYSTEM
            ecall     11..7=0 19..15=0 31..20=0x000 14..12=0 6..2=0x1C 1..0=3
            ebreak    11..7=0 19..15=0 31..20=0x001 14..12=0 6..2=0x1C 1..0=3
            uret      11..7=0 19..15=0 31..20=0x002 14..12=0 6..2=0x1C 1..0=3
            sret      11..7=0 19..15=0 31..20=0x102 14..12=0 6..2=0x1C 1..0=3
            mret      11..7=0 19..15=0 31..20=0x302 14..12=0 6..2=0x1C 1..0=3
            dret      11..7=0 19..15=0 31..20=0x7b2 14..12=0 6..2=0x1C 1..0=3
            sfence.vma 11..7=0 rs1 rs2 31..25=0x09  14..12=0 6..2=0x1C 1..0=3
            wfi       11..7=0 19..15=0 31..20=0x105 14..12=0 6..2=0x1C 1..0=3
            csrrw     rd      rs1      imm12        14..12=1 6..2=0x1C 1..0=3
            csrrs     rd      rs1      imm12        14..12=2 6..2=0x1C 1..0=3
            csrrc     rd      rs1      imm12        14..12=3 6..2=0x1C 1..0=3
            csrrwi    rd      rs1      imm12        14..12=5 6..2=0x1C 1..0=3
            csrrsi    rd      rs1      imm12        14..12=6 6..2=0x1C 1..0=3
            csrrci    rd      rs1      imm12        14..12=7 6..2=0x1C 1..0=3
         * 
         */

        private const int csrrw = 1;
        private const int csrrs = 2;
        private const int csrrc = 3;
        private const int csrrwi = 5;
        private const int csrrsi = 6;
        private const int csrrci = 7;

        public override int Opcode => 0x1C;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var f3 = payload.Funct3;
            var f12 = payload.UnsignedImmediate;

            Logger.Info("Opcode 1C : rd = {rd}, rs1 = {rs1}, f3 = 0x{f3:X}, f12 = 0x{f12:x}",rd,rs1,f3,f12);

            if (f3 == 0)
            {
                HandleSystemCall(payload);
            }
            else
            {
                HandleCsr(payload);
            }

            return true;
        }

        private void HandleSystemCall(InstructionPayload payload)
        {
            Logger.Info("System Call detected");

            environment.NoitfySystemCall(payload.UnsignedImmediate);
        }

        private void HandleCsr(InstructionPayload payload)
        {
            Logger.Info("CSR Call detected.");

            var csrIndex = payload.SignedImmediateComplete;
            var rs1 = payload.Rs1;
            var rd = payload.Rd;

            var rs1Value = Register.ReadSignedInt(rs1);
            var csrValue = ReadAndExtendCsr(csrIndex);

            // CSR Value is 5 Bit and gets zero extended to the register length

            switch (payload.Funct3)
            {
                //
                // Atomic Read Write CSR : An atomic Swap of CSR register content and integer register
                //
                case csrrw:
                    DoCsrrw(rd, csrIndex, csrValue, rs1Value);
                    break;

                //
                //  Atomic Read and Set Bits in CSR
                //
                case csrrs:
                    DoCsrrs(rd, rs1, csrIndex, csrValue, rs1Value);
                    break;

                //
                //  Atomic Read and Clear Bits in CSR
                //
                case csrrc:
                    DoCsrrc(rd, rs1, csrIndex, csrValue, rs1Value);
                    break;

                case csrrwi:
                    DoCsrrw(rd, csrIndex, csrValue, rs1);
                    break;

                case csrrsi:
                    DoCsrrs(rd, rs1, csrIndex, csrValue, rs1);
                    break;

                case csrrci:
                    DoCsrrc(rd, rs1, csrIndex, csrValue, rs1);
                    break;

                default:
                    throw new RiscVSimException("Unknown CSR instruction detected!");
            }
        }

        private void DoCsrrw(int rd, int csrIndex, int csrValue, int value)
        {
            WriteToCsr(csrIndex, value);
            if (rd != 0)
            {
                WriteToRegister(rd, csrValue);
            }
        }

        private void DoCsrrs(int rd, int rs1,int csrIndex, int csrValue, int value)
        {
            int buffer = csrValue;
            // Logical OR with the content of rs1
            buffer &= value;

            if (rs1 != 0)
            {
                WriteToCsr(csrIndex, buffer);
            }

            if (rd != 0)
            {
                WriteToRegister(rd, csrValue);
            }
        }

        private void DoCsrrc(int rd, int rs1,int csrIndex, int csrValue, int value)
        {
            int buffer = csrValue;
            // Logical OR with the inverse content of rs1
            buffer &= (~value);

            if (rs1 != 0)
            {
                WriteToCsr(csrIndex, buffer);
            }

            if (rd != 0)
            {
                WriteToRegister(rd, csrValue);
            }
        }

        private int ReadAndExtendCsr(int csrIndex)
        {
            int buffer = 0;
            var value = csrRegister.Read(csrIndex);

            buffer |= value;
            return buffer;
        }

        private void WriteToCsr(int csrIndex, int value)
        {
            var buffer = value & 0x1F; // Get the last 5 bits
            byte csrValue = Convert.ToByte(buffer);

            csrRegister.Write(csrIndex, csrValue);
        }

        private void WriteToRegister(int registerIndex, int value)
        {
            int buffer = value;
            buffer &= 0x1F; // Only the last 5 Bits

            Register.WriteSignedInt(registerIndex, buffer);
        }

    }
}
