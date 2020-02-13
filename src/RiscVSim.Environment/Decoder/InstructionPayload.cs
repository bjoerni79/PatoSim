using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    /// <summary>
    /// Represents a simple container for the payload of the instruction.
    /// </summary>
    public sealed class InstructionPayload
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public InstructionPayload(Instruction instruction, IEnumerable<byte> coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            Type = instruction.Type;
            OpCode = instruction.OpCode;
            Coding = coding;
        }

        public IEnumerable<byte> Coding { get; private set; }

        public int Rd { get;  set; }

        public int OpCode { get;  set; }

        public InstructionType Type { get;  set; }



        public int Rs1 { get;  set; }

        public int Rs2 { get;  set; }



        public int Funct3 { get;  set; }

        public int Funct7 { get;  set; }

        public int SignedImmediate { get;  set; }

        public uint UnsignedImmediate { get;  set; }

        /// <summary>
        /// All bits are treated as values bits 
        /// </summary>
        public int SignedImmediateComplete { get;  set; }

        public string GetHumanReadbleContent()
        {
            var common = new Common();
            var sb = new StringBuilder();
            sb.AppendFormat("{0:X8}, Opcode = {1:X2}, ",BitConverter.ToString(Coding.ToArray()),  OpCode);

            switch (Type)
            {
                case InstructionType.R_Type:
                    sb.AppendFormat("Type = R\tRd = {0}\tf3 = {1:X}\tRs1 = {2}\tRs2 = {3}\tf7 = {4:X}", common.DecocdeRegisterIndex(Rd), Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), Funct7);
                    break;

                case InstructionType.I_Type:
                    sb.AppendFormat("Type = I\tRd = {0}\tf3 = {1:X}\tRs1 = {2}\tSigned Imm = {3:X}\tUnsigned Imm = {4:X}", common.DecocdeRegisterIndex(Rd), Funct3, common.DecocdeRegisterIndex(Rs1), SignedImmediate, UnsignedImmediate);
                    break;

                case InstructionType.S_Type:
                    sb.AppendFormat("Type = S\tf3 = {0:X}\tRs1 = {1}\tRs2 = {2}\tSigned Imm = {3:X}", Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), SignedImmediate);
                    break;

                case InstructionType.U_Type:
                    sb.AppendFormat("Type = U\tRd = {0}\tUnsigned Imm = {1:X}", common.DecocdeRegisterIndex(Rd), SignedImmediate, UnsignedImmediate);
                    break;

                case InstructionType.B_Type:
                    sb.AppendFormat("Type = B\tF3 = {0:X}\tRs1 = {1}\tRs2 = {2}\tSigned Immediate = {3:X}", Funct3, common.DecocdeRegisterIndex(Rs1), common.DecocdeRegisterIndex(Rs2), SignedImmediate);
                    break;

                case InstructionType.J_Type:
                    sb.AppendFormat("Type = J\tRd = {0}\tImmediate = {1:X}", common.DecocdeRegisterIndex(Rd), SignedImmediate);
                    break;

                default:
                    sb.AppendFormat("Type = Unknown, Opcode={0:X}\tRd = {1}", OpCode, common.DecocdeRegisterIndex(Rd));
                    break;
            }

            Logger.Info(sb.ToString());
            return sb.ToString();
        }
    }
}
