using RiscVSim.Environment;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.OpCodes.Rv32I
{
    /// <summary>
    /// Implements the RV32I conditional branches BNE...
    /// </summary>
    public class OpCode32Id18 : OpCodeCommand
    {
        private Stack<uint> ras;

        public OpCode32Id18 (IMemory memory, IRegister register, Stack<uint> ras) : base (memory,register)
        {
            // base (...)
            this.ras = ras;
        }

        public override int Opcode => 0x18;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rs1 = payload.Rs1;
            var rs2 = payload.Rs2;
            var f3 = payload.Funct3;

            var rs1Signed = Register.ReadSignedInt(rs1);
            var rs2Signed = Register.ReadSignedInt(rs2);
            var rs1Unsigned = Register.ReadUnsignedInt(rs1);
            var rs2Unsigned = Register.ReadUnsignedInt(rs2);

            bool doJump = false;

            Logger.Info("Opcode 18 : rs1 = {rs1}, rs2 = {rs2} funct3 = {f3}", rs1, rs2, f3);

            switch (f3)
            {
                // beq
                case 0:
                    if (rs1Signed == rs2Signed)
                    {
                        doJump = true;
                    }
                    break;

                // bne
                case 1:
                    if (rs1Signed != rs2Signed)
                    {
                        doJump = true;
                    }
                    break;

                // blt
                case 4:
                    if (rs1Signed < rs2Signed)
                    {
                        doJump = true;
                    }
                    break;

                // bge
                case 5:
                    if (rs1Signed > rs2Signed)
                    {
                        doJump = true;
                    }
                    break;

                // bltu
                case 6:
                    if (rs1Unsigned < rs2Unsigned)
                    {
                        doJump = true;
                    }
                    break;

                // bgeu
                case 7:
                    if (rs1Unsigned > rs2Unsigned)
                    {
                        doJump = true;
                    }
                    break;

                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, f3));

            }

            if (doJump)
            {
                var pcIndex = Register.ProgramCounter;
                var pc = Register.ReadUnsignedInt(pcIndex);

                var newPc = MathHelper.Add(pc, payload.SignedImmediate);
                var rasPc = pc + 4;

                // Write it to X1 and the RAS
                Register.WriteUnsignedInt(1, rasPc);
                ras.Push(rasPc);

                Register.WriteUnsignedInt(pcIndex, newPc);
            }

            return !doJump;
        }
    }
}
