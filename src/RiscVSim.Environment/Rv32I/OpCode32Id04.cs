using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public sealed class OpCode32Id04 : OpCodeCommand
    {
        private Hint hint;

        public OpCode32Id04(IMemory memory, IRegister register, Hint hint) : base(memory, register)
        {

            this.hint = hint;
            // memry and register are stored in the base class
        }

        /*
         * Variants of the Opcode 04:
         * 
         *  addi    rd rs1 imm12           14..12=0 6..2=0x04 1..0=3
            slli    rd rs1 31..26=0  shamt 14..12=1 6..2=0x04 1..0=3
            slti    rd rs1 imm12           14..12=2 6..2=0x04 1..0=3
            sltiu   rd rs1 imm12           14..12=3 6..2=0x04 1..0=3
            xori    rd rs1 imm12           14..12=4 6..2=0x04 1..0=3
            srli    rd rs1 31..26=0  shamt 14..12=5 6..2=0x04 1..0=3
            srai    rd rs1 31..26=16 shamt 14..12=5 6..2=0x04 1..0=3
            ori     rd rs1 imm12           14..12=6 6..2=0x04 1..0=3
            andi    rd rs1 imm12           14..12=7 6..2=0x04 1..0=3
         * 
         */

        private const int addi = 0;
        private const int slli = 1; // Shift Logical left ..
        private const int slti = 2;
        private const int sltiu = 3;
        private const int xori = 4;
        private const int srli_and_srai = 5;
        private const int ori = 6;
        private const int andi = 7;


        public override int Opcode => 0x04;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            if (instruction.RegisterDestination == 0)
            {
                // Test for a NOP operation
                var isNop = (payload.Rs1 == 0) && (payload.SignedImmediate ==0) && (payload.Funct3==0);
                if (isNop && hint != null)
                {
                    hint.IncreaseNopCounter();
                }
            }
            else
            {
                Run(instruction, payload);
            }

            return true;
        }

        private void Run(Instruction instruction, InstructionPayload payload)
        {
            // preload some content ...
            int rd = payload.Rd;
            int rs1 = payload.Rs1;
            int immediate = payload.SignedImmediate;

            // and define space to compute...
            int rs1ValueSigned = Register.ReadSignedInt(rs1);
            uint rs1ValueUnsigned = Register.ReadUnsignedInt(rs1);
            IEnumerable<byte> rs1block = Register.ReadBlock(rs1);
            IEnumerable<byte> resultBuffer;

            int resultSigned;
            uint resultUnsigned;


            var funct3 = payload.Funct3;
            switch (funct3)
            {
                // addi:
                // ADDI adds the sign-extended 12-bit immediate to register rs1. Arithmetic overflow is ignored and
                // the result is simply the low XLEN bits of the result. ADDI rd, rs1, 0 is used to implement the MV
                // rd, rs1 assembler pseudoinstruction.
                case addi:
                    resultSigned = immediate + rs1ValueSigned;
                    Register.WriteSignedInt(rd, resultSigned);
                    break;

                // slti  (set less than immediate)
                // SLTI (set less than immediate) places the value 1 in register rd if register rs1 is less than the signextended
                // immediate when both are treated as signed numbers, else 0 is written to rd. 
                case slti:
                    if (rs1ValueSigned < immediate)
                    {
                        Register.WriteSignedInt(rd, 1); // true
                    }
                    else
                    {
                        Register.WriteSignedInt(rd, 0); // false
                    }
                    break;

                // sltiu (set less then immediate unsigned)
                // SLTIU is similar but compares the values as unsigned numbers (i.e., the immediate is first sign-extended to
                // XLEN bits then treated as an unsigned number). 
                // Note, SLTIU rd, rs1, 1 sets rd to 1 if rs1 equals zero, otherwise sets rd to 0 (assembler pseudoinstruction SEQZ rd, rs).
                case sltiu:
                    // SEQZ rd,rs
                    if (immediate == 1)
                    {
                        if (rs1ValueUnsigned == 0)
                        {
                            Register.WriteSignedInt(rd, 1);
                        }
                        else
                        {
                            Register.WriteSignedInt(rd, 0);
                        }
                    }
                    else
                    {
                        uint immediateUnsinged = Convert.ToUInt32(immediate);
                        // Compare them
                        if (rs1ValueUnsigned < immediateUnsinged)
                        {
                            Register.WriteSignedInt(rd, 1);
                        }
                        else
                        {
                            Register.WriteSignedInt(rd, 0);
                        }
                    }
                    break;

                //
                // andi
                //
                case andi:
                    resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Add, rs1block, immediate, Architecture.Rv32I);
                    Register.WriteBlock(rd, resultBuffer);
                    break;

                //
                //  ori
                //
                case ori:
                    resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Or, rs1block, immediate, Architecture.Rv32I);
                    Register.WriteBlock(rd, resultBuffer);
                    break;

                //
                // xori
                //
                case xori:
                    if (immediate == 0xfff) // 12 Bit Immediate with -1
                    {
                        resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.BitwiseInversion, rs1block, immediate, Architecture.Rv32I);
                        Register.WriteBlock(rd, resultBuffer);
                    }
                    else
                    {
                        resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Xor, rs1block, immediate, Architecture.Rv32I);
                        Register.WriteBlock(rd, resultBuffer);
                    }

                    break;

                //
                // slli
                //
                // Shifts by a constant are encoded as a specialization of the I-type format. 
                // The operand to be shiftedis in rs1, and the shift amount is encoded in the lower 5 bits of the I-immediate field.
                case slli:
                    var leftShiftAmount = immediate & 0x1F; // the last 5 bytes are the shift increment;
                    resultUnsigned = rs1ValueUnsigned << leftShiftAmount;
                    Register.WriteUnsignedInt(rd, resultUnsigned);
                    break;

                //
                // srli and srai
                //
                // The right shift type is encoded in bit 30.SLLI is a logical left shift(zeros are shifted into the lower bits); 
                // Volume I: RISC - V Unprivileged ISA V20191213 19 SRLI is a logical right shift(zeros are shifted into the upper bits); 
                // and SRAI is an arithmetic right shift(the original sign bit is copied into the vacated upper bits).
                case srli_and_srai:

                    // https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators

                    var rightShiftAmount = immediate & 0x1F;
                    var rightShiftMode = (immediate & 0x0400);
                    if (rightShiftMode == 0x400)
                    {
                        resultSigned = rs1ValueSigned >> rightShiftAmount;
                        Register.WriteSignedInt(rd, resultSigned);
                    }
                    else
                    {
                        resultUnsigned = rs1ValueUnsigned >> rightShiftAmount;
                        Register.WriteUnsignedInt(rd, resultUnsigned);
                    }
                    break;

                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, funct3));
            }
        }

    }
}
