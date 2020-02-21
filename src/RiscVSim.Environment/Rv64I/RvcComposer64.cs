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
        private const int Register32 = 0x0E;
        private const int Lui = 0x0D;

        private const int ADDI4SPN = 0;
        private const int CLWSP = 2;
        private const int CLW = 2;
        private const int CLDSP = 3;
        private const int CLD = 3;
        private const int CSWSP = 6;
        private const int CSW = 6;
        private const int CSD = 7;
        private const int CSDSP = 7;
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
                    case CLDSP:
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
                            // Register or Register32 ?
                            if ((payload.Funct6 & 0x04) == 0x04)
                            {
                                opCode = Register32;
                            }
                            else
                            {
                                opCode = Register;
                            }
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

            // Q02
            // 000 C.SLLI
            // 001 C.FLDSP
            // 010 C.LWSP
            // 011 C.FLWSP
            // 100 0 C.JR / C.MV
            // 100 1 C.EBREAK / C.JALR / C.ADD
            // 101 C.FSDSP
            // 110 C.SWSP
            // 111 C.FSWSP
            if (payload.Op == 2)
            {
                switch (payload.Funct3)
                {
                    // C.SLLI
                    case CSLLI:
                        opCode = Immediate;
                        break;

                    // C.LWSP
                    case CLWSP:
                        opCode = Load;
                        break;

                    case CLDSP:
                        opCode = Load;
                        break;

                    // C.JR (Jalr)
                    // C.MV (ADD rd,x0,rs2)
                    // C.EBREAK 
                    // C.JALR (Jalr)
                    // C.ADD
                    case 4:
                        var isJr = (payload.Funct4 == 0x08) && (payload.Rs2 == 0);
                        var isMv = (payload.Funct4 == 0x08) && (payload.Rd != 0) && (payload.Rs2 != 0);
                        var isEBreak = (payload.Funct4 == 0x09) && (payload.Rd == 0) && (payload.Rs2 == 0);
                        var isJalr = (payload.Funct4 == 0x09) && (payload.Rs1 != 0) && (payload.Rs2 == 0);
                        var isAdd = (payload.Funct4 == 0x09) && (payload.Rs1 != 0) && (payload.Rs2 != 0);

                        if (isJr)
                        {
                            opCode = JumpAndLinkRegister;
                        }

                        if (isMv)
                        {
                            opCode = Register;
                        }

                        if (isEBreak)
                        {
                            opCode = System;
                        }

                        if (isJalr)
                        {
                            opCode = JumpAndLinkRegister;
                        }

                        if (isAdd)
                        {
                            opCode = Register;
                        }


                        if (!opCode.HasValue)
                        {
                            throw new RvcFormatException("Invalid coding detected for Q02, F3 100");
                        }
                        break;

                    // C.SWSP
                    case CSWSP:
                        opCode = Store;
                        break;

                    case CSDSP:
                        opCode = Store;
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

            if (ins.OpCode == System)
            {
                instructionPayload = ComposeSystem(ins, payload);
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

            if (ins.OpCode == Register32)
            {
                instructionPayload = ComposeRegister32(ins, payload);
            }

            if (ins.OpCode == JumpAndLink)
            {
                instructionPayload = ComposeJal(ins, payload);
            }

            if (ins.OpCode == JumpAndLinkRegister)
            {
                instructionPayload = ComposeJalr(ins, payload);
            }

            if (ins.OpCode == CondBrach)
            {
                instructionPayload = ComposeBranch(ins, payload);
            }

            if (ins.OpCode == Lui)
            {
                instructionPayload = ComposeLui(ins, payload);
            }

            return instructionPayload;
        }

        private InstructionPayload ComposeSystem(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 2 && payload.Funct3 == 4)
            {
                parser32.ParseEbreak(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeLui(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 1 && payload.Funct3 == 3)
            {
                // C.LUI
                parser32.ParseLui(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeBranch(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Funct3 == BEQZ)
            {
                parser32.ParseBeqzAndBnez(payload, p, true);
            }

            if (payload.Funct3 == BNEZ)
            {
                parser32.ParseBeqzAndBnez(payload, p, false);
            }

            return p;
        }

        private InstructionPayload ComposeJalr(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Funct3 == 4)
            {
                // C.JR    (f4 = 8)
                // C.JALR  (f4 = 9)

                parser32.ParseJrAndJalr(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeJal(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            // C.J
            if (payload.Op == 1 && payload.Funct3 == CJ)
            {
                parser32.ParseJ(payload, p);
            }

            return p;
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

            if (payload.Op == 2 && payload.Funct3 == CSWSP)
            {
                parser32.ParseSwSp(payload, p);
            }

            if (payload.Op == 0 && payload.Funct3 == CSD)
            {
                parser64.ParseSd(payload, p);
            }

            if (payload.Op == 2 && payload.Funct3 == CSDSP)
            {
                parser64.ParseSdSp(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposeLoad(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 2 && payload.Funct3 == CLWSP)
            {
                parser32.ParseLwSp(payload, p);
            }

            if (payload.Op == 2 && payload.Funct3 == CLDSP)
            {
                parser64.ParseLdSp(payload, p);
            }

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

        private InstructionPayload ComposeRegister32(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 1 && payload.Funct3 == 4 && payload.Funct2 == 3)
            {
                var mode = payload.Funct6 & 0x07;

                if (mode == 0x07)
                {
                    // F4 = 9 => C.SUBW / C.ADDW
                    parser64.ParseAddWSubW(payload, p);
                }
            }

            return p;
        }

        private InstructionPayload ComposeRegister(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Op == 1 && payload.Funct3 == 4 && payload.Funct2 == 3)
            {
                var mode = payload.Funct6 & 0x07;

                if (mode == 0x03)
                {
                    // F4 = 8
                    parser32.ParseCaGeneric(payload, p);
                }

                if (mode == 0x07)
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

            if (payload.Op == 1 && payload.Funct3 == 2)
            {
                // C.LI
                parser32.ParseLi(payload, p);
            }


            if (payload.Op == 2 && payload.Funct3 == CSLLI)
            {
                parser32.ParseSlli(payload, p);
            }



            return p;
        }
    }
}
