using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    /// <summary>
    /// Represents a simple container for the payload of the instruction.
    /// </summary>
    public sealed class InstructionPayload
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal InstructionPayload(Instruction instruction, IEnumerable<byte> coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            Type = instruction.Type;
            OpCode = instruction.OpCode;
            Rd = instruction.RegisterDestination;
            Coding = coding;
        }

        public IEnumerable<byte> Coding { get; private set; }

        public int Rd { get; private set; }

        public int OpCode { get; private set; }

        public InstructionType Type { get; private set; }



        public int Rs1 { get; internal set; }

        public int Rs2 { get; internal set; }



        public int Funct3 { get; internal set; }

        public int Funct7 { get; internal set; }

        public int SignedImmediate { get; internal set; }

        public uint UnsignedImmediate { get; internal set; }

        /// <summary>
        /// All bits are treated as values bits 
        /// </summary>
        public int SignedImmediateComplete { get; internal set; }

        public string GetHumanReadbleContent()
        {
            var common = new Common();
            var sb = new StringBuilder();
            sb.AppendFormat("Opcode = {0:X2}, ", OpCode);

            switch (Type)
            {
                case InstructionType.R_Type:
                    sb.AppendFormat("Type = R, Rd = {0}, f3 = {1:X}, Rs1 = {2}, Rs2 = {3}, f7 = {4:X}", common.DecocdeRegisterIndex(Rd), Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), Funct7);
                    break;

                case InstructionType.I_Type:
                    sb.AppendFormat("Type = I, Rd= {0}, f3 = {1:X}, Rs1 = {2}, Signed Imm = {3:X}, Unsigned Imm = {4:X}", common.DecocdeRegisterIndex(Rd), Funct3, common.DecocdeRegisterIndex(Rs1), SignedImmediate, UnsignedImmediate);
                    break;

                case InstructionType.S_Type:
                    sb.AppendFormat("Type = S, f3 = {0:X}, Rs1 = {1}, Rs2 = {2}, Signed Imm = {3:X}", Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), SignedImmediate);
                    break;

                case InstructionType.U_Type:
                    sb.AppendFormat("Type = U, Rd = {0},  Unsigned Imm = {1:X}", common.DecocdeRegisterIndex(Rd), SignedImmediate, UnsignedImmediate);
                    break;

                case InstructionType.B_Type:
                    sb.AppendFormat("Type = B, F3 = {0:X}, Rs1 = {1}, Rs2 = {2} , Signed Immediate = {3:X}", Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), SignedImmediate);
                    break;

                case InstructionType.J_Type:
                    sb.AppendFormat("Type = J, Rd = {0}, Immediate = {1:X}", common.DecocdeRegisterIndex(Rd), SignedImmediate);
                    break;

                default:
                    sb.AppendFormat("Type = Unknown, Opcode={0:X} Rd = {1}", OpCode, common.DecocdeRegisterIndex(Rd));
                    break;
            }

            Logger.Info(sb.ToString());
            return sb.ToString();
        }
    }
}
