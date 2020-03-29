using RiscVSim.Environment;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.OpCodes.RV64I
{
    public class OpCode64Id04 : OpCodeCommand
    {
        private ISystemNotifier environment;

        private const int addi = 0;
        private const int slli = 1; // Shift Logical left ..
        private const int slti = 2;
        private const int sltiu = 3;
        private const int xori = 4;
        private const int srli_and_srai = 5;
        private const int ori = 6;
        private const int andi = 7;

        public OpCode64Id04(IMemory memory, IRegister register, ISystemNotifier environment) : base(memory, register)
        {
            this.environment = environment;
            // base()..
        }

        public override int Opcode => 0x04;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            if (payload.Rd == 0)
            {
                // Test for a NOP operation
                var isNop = (payload.Rs1 == 0) && (payload.SignedImmediate == 0) && (payload.Funct3 == 0);
                if (isNop && environment != null)
                {
                    Logger.Info("Opcode04 : NOP operation detected");
                    environment.IncreaseNopCounter();
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
            long rs1ValueSigned = Register.ReadSignedLong(rs1);
            ulong rs1ValueUnsigned = Register.ReadUnsignedLong(rs1);
            IEnumerable<byte> rs1block = Register.ReadBlock(rs1);
            IEnumerable<byte> resultBuffer;

            long resultSigned;
            ulong resultUnsigned;


            var funct3 = payload.Funct3;
            Logger.Info("Opcode 04: rd={rd}, rs1={rs1}, funct3={funct3}", rd, rs1, funct3);
            switch (funct3)
            {
                // Addi
                case addi:
                    resultSigned = immediate + rs1ValueSigned;
                    Register.WriteSignedLong(rd, resultSigned);
                    break;

                // SLTI (set less than immediate)
                case slti:
                    if (rs1ValueSigned < immediate)
                    {
                        Register.WriteSignedLong(rd, 1); // true
                    }
                    else
                    {
                        Register.WriteSignedLong(rd, 0); // false
                    }
                    break;

                // SLTIU  (Set less than immediate unsigned
                case sltiu:
                    // SEQZ rd,rs
                    if (immediate == 1)
                    {
                        if (rs1ValueUnsigned == 0)
                        {
                            Register.WriteSignedLong(rd, 1);
                        }
                        else
                        {
                            Register.WriteSignedLong(rd, 0);
                        }
                    }
                    else
                    {
                        uint immediateUnsinged = Convert.ToUInt32(immediate);
                        // Compare them
                        if (rs1ValueUnsigned < immediateUnsinged)
                        {
                            Register.WriteSignedLong(rd, 1);
                        }
                        else
                        {
                            Register.WriteSignedLong(rd, 0);
                        }
                    }
                    break;

                // ANDI (logical and)
                case andi:
                    resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Add, rs1block, immediate, Architecture.Rv64I);
                    Register.WriteBlock(rd, resultBuffer);
                    break;

                // ORI (logical or)
                case ori:
                    resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Or, rs1block, immediate, Architecture.Rv64I);
                    Register.WriteBlock(rd, resultBuffer);
                    break;

                // xori (logical xor)
                case xori:
                    if (immediate == 0xfff) // 12 Bit Immediate with -1
                    {
                        resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.BitwiseInversion, rs1block, immediate, Architecture.Rv64I);
                        Register.WriteBlock(rd, resultBuffer);
                    }
                    else
                    {
                        resultBuffer = MathHelper.ExecuteLogicalOp(MathHelper.LogicalOp.Xor, rs1block, immediate, Architecture.Rv64I);
                        Register.WriteBlock(rd, resultBuffer);
                    }

                    break;

                // Shift Left Immediate
                case slli:
                    var leftShiftAmount = immediate & 0x3F; // the last 6 bytes are the shift increment;
                    resultUnsigned = rs1ValueUnsigned << leftShiftAmount;
                    Register.WriteUnsignedLong(rd, resultUnsigned);
                    break;

                case srli_and_srai:

                    // https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators

                    var rightShiftAmount = immediate & 0x3F;  // Get the last 6 Bit  (RV64I difference compared toRV32)
                    var rightShiftMode = (immediate & 0x0400);
                    if (rightShiftMode == 0x400)
                    {
                        resultSigned = rs1ValueSigned >> rightShiftAmount;
                        Register.WriteSignedLong(rd, resultSigned);
                    }
                    else
                    {
                        resultUnsigned = rs1ValueUnsigned >> rightShiftAmount;
                        Register.WriteUnsignedLong(rd, resultUnsigned);
                    }
                    break;

                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, funct3));
            }
        }
    }
}
