using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class RvcComposer64 : IRvcComposer
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Rvc32Parser parser32;
        private Rvc64Parser parser64;

        private const int System = 0x1C;
        private const int Load = 0x00;
        private const int Immediate = 0x04;
        private const int Immediate32 = 0x06;
        private const int Store = 0x08;
        private const int CondBrach = 0x018;
        private const int JumpAndLink = 0x1B;
        private const int JumpAndLinkRegister = 0x19;
        private const int Register = 0x0C;
        private const int Lui = 0x0D;

        private const int ADDI4SPN = 0;
        private const int CLWSP = 2;
        private const int CLW = 2;
        private const int CLD = 3;
        private const int CSWSP = 6;
        private const int CSW = 6;
        private const int CSD = 7;
        private const int CSLLI = 0;
        private const int ADDIW = 1;
        private const int CJ = 5;
        private const int BEQZ = 6;
        private const int BNEZ = 7;

        public RvcComposer64()
        {
            parser32 = new Rvc32Parser();
            parser64 = new Rvc64Parser();
        }

        public Instruction ComposeInstruction(RvcPayload payload)
        {
            Logger.Debug("Compose Instruction for payload : Op = {op:X}, F3 = {f3:X}", payload.Op, payload.Funct3);
            int? opCode = null;

            // Q00
            // 000 C.ADDI4SPN OpCode 04
            // 001 C.FLD Not supported
            // 010 C.LW OpCode 00
            // 011 C.LD 
            // 101 C.FSD Not Supported
            // 110 C.SW OpCode 08
            // 111 C.SD
            if (payload.Op == 0x00)
            {
                switch (payload.Funct3)
                {
                    // [32] C.ADDI4SPN
                    case ADDI4SPN:
                        opCode = Immediate;
                        break;

                    // [32] C.LW
                    case CLW:
                        opCode = Load;
                        break;

                    // [64] C.LD
                    case CLD:
                        opCode = Load;
                        break;

                    // [32] C.SW
                    case CSW:
                        opCode = Store;
                        break;

                    // [64] C.SD
                    case CSD:
                        opCode = Store;
                        break;

                    default:
                        string message = string.Format("RVC Opcode {0:X} and F3 {1:X} is not supported", payload.Op, payload.Funct3);
                        throw new OpCodeNotSupportedException(message);
                }
            }

            // Q01
            // 000 C.NOP / C.ADDI OpCode 04
            // 001 C.ADDIW
            // 010 C.LI
            // 011 C.ADDI16SP (RD=2)
            // 011 C.LUI (RD != 2)
            // 100 x 00 C.SRLI
            // 100 x 01 C.SRAI
            // 100 x 10 C.ANDI
            // 100 x 11 C.SUB, C.XOR, C.OR, C.AND
            // 101 C.J
            // 110 C.BEQZ
            // 111 C.BNEZ
            if (payload.Op == 01)
            {
                switch (payload.Funct3)
                {
                    // C.NOP / C.ADDI
                    case 0:
                        opCode = Immediate;
                        break;

                    // C.ADDIW
                    case ADDIW:
                        opCode = Immediate32;
                        break;

                    // C.LI
                    case 2:
                        opCode = Immediate;
                        break;

                    // C.LUI
                    // C.ADDI16SP
                    case 3:
                        if (payload.Rd != 0 && payload.Rd != 2)
                        {
                            opCode = Lui;
                        }

                        if (payload.Rd == 2)
                        {
                            opCode = Immediate;
                        }

                        // C.ADDI16SP....

                        break;

                    // C.SRLI, C.SRAI, ...
                    // 100 x 00 C.SRLI
                    // 100 x 01 C.SRAI
                    // 100 x 10 C.ANDI
                    // 100 x 11 C.SUB, C.XOR, C.OR, C.AND
                    case 4:
                        if (payload.Funct2 == 0x03)
                        {
                            opCode = Register;
                        }
                        else
                        {
                            opCode = Immediate;
                        }
                        break;

                    // C.J
                    case CJ:
                        opCode = JumpAndLink;
                        break;

                    // C.BEQZ
                    case BEQZ:
                        opCode = CondBrach;
                        break;

                    // C.BNEZ
                    case BNEZ:
                        opCode = CondBrach;
                        break;

                    default:
                        string message = string.Format("RVC Opcode {0:X} and F3 {1:X} is not supported", payload.Op, payload.Funct3);
                        throw new OpCodeNotSupportedException(message);
                }
            }

            // If opCode is still null, we encountered an error somehow..
            if (!opCode.HasValue)
            {
                throw new RiscVSimException("Invalid RVC Opcode detected");
            }


            var instruction = new Instruction(payload.Type, opCode.Value, 2);
            return instruction;
        }

        public InstructionPayload Compose(Instruction ins, RvcPayload payload)
        {
            InstructionPayload instructionPayload = null;

            if (ins.OpCode == Store)
            {
                instructionPayload = ComposeStore(ins, payload);
            }

            if (ins.OpCode == Load)
            {
                instructionPayload = ComposeLoad(ins, payload);
            }

            if (ins.OpCode == Immediate)
            {
                instructionPayload = ComposeImmediate(ins, payload);
            }

            if (ins.OpCode == Immediate32)
            {
                instructionPayload = ComposeImmediate32(ins, payload);
            }

            if (ins.OpCode == Register)
            {
                instructionPayload = ComposeRegister(ins, payload);
            }

            return instructionPayload;
        }

        private InstructionPayload ComposeImmediate32 (Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 1 && payload.Funct3 == ADDIW)
            {
                parser64.ParseAddiW(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeStore(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 0 && payload.Funct3 == CSW)
            {
                parser32.ParseSw(payload, p);
            }

            if (payload.Op == 0 && payload.Funct3 == CSD)
            {
                parser64.ParseSd(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeLoad(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 0 && payload.Funct3 == CLW)
            {
                parser32.ParseLw(payload, p);
            }

            if (payload.Op == 0 && payload.Funct3 == CLD)
            {
                parser64.ParseLd(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeRegister(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 1 && payload.Funct3 == 4 && payload.Funct2 == 3)
            {
                if (payload.Funct4 == 8)
                {
                    // F4 = 8
                    parser32.ParseCaGeneric(payload, p);
                }

                if (payload.Funct4 == 9)
                {
                    // F4 = 9 => C.SUBW / C.ADDW
                    parser64.ParseAddWSubW(payload, p);
                }
            }

            if (payload.Op == 2 && payload.Funct3 == 4)
            {
                parser32.ParseAddAndMv(payload, p);
            }


            return p;
        }

        private InstructionPayload ComposeImmediate(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 0 && payload.Funct3 == ADDI4SPN)
            {
                parser32.ParseAddi4Spn(payload, p);
            }

            if (payload.Op == 1 && payload.Funct3 == 0)
            {
                parser32.ParseAddi(payload, p);
            }

            if (payload.Op == 1 && payload.Funct3 == 3)
            {
                parser32.ParseAddi16Sp(payload, p);
            }

            if (payload.Op == 1 && payload.Funct3 == 4)
            {
                if (payload.Funct2 == 0)
                {
                    parser32.ParseSrli(payload, p);
                }

                if (payload.Funct2 == 1)
                {
                    parser32.ParseSrai(payload, p);
                }

                if (payload.Funct3 == 4 && payload.Funct2 == 2)
                {
                    parser32.ParseAndi(payload, p);
                }
            }

            return p;
        }
    }
}
