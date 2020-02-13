using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class RvcComposer : IRvcComposer
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Rvc32Parser parser;

        private const int System = 0x1C;
        private const int Load = 0x00;
        private const int Immediate = 0x04;
        private const int Store = 0x08;
        private const int CondBrach = 0x018;
        private const int JumpAndLink = 0x1B;
        private const int JumpAndLinkRegister = 0x19;
        private const int Register = 0x0C;


        private const int CLWSP = 2;
        private const int CSWSP = 6;
        private const int CSLLI = 0;

        public RvcComposer()
        {
            parser = new Rvc32Parser();
        }

        public Instruction ComposeInstruction(RvcPayload payload)
        {
            Logger.Debug("Compose Instruction for payload : Op = {op:X}, F3 = {f3:X}", payload.Op, payload.Funct3);
            int? opCode = null;

            // Q00
            // 000 C.ADDI4SPN OpCode 04
            // 001 C.FLD Not supported
            // 010 C.LW OpCode 00
            // 011 C.FLW Not Supported
            // 101 C.FSD Not Supported
            // 110 C.SW OpCode 08
            // 111 C.FSW Not Supported
            if (payload.Op == 0x00)
            {
                switch (payload.Funct3)
                {
                    // C.ADDI4SPN
                    case 0:
                        opCode = Immediate;
                        break;

                    // C.LW
                    case 2:
                        opCode = Load;
                        break;

                    // C.SW
                    case 6:
                        opCode = Store;
                        break;

                    default:
                        string message = string.Format("RVC Opcode {0:X} and F3 {1:X} is not supported",payload.Op,payload.Funct3);
                        throw new OpCodeNotSupportedException(message);
                }
            }


            // Q01
            // 000 C.NOP / C.ADDI OpCode 04
            // 001 C.JAL
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

                    // C.JAL
                    case 1:
                        opCode = JumpAndLink;
                        break;

                    // C.LI
                    case 2:
                        opCode = Immediate;
                        break;

                    // C.LUI
                    // C.ADDI16SP
                    case 3:
                        opCode = Immediate;
                        break;

                    // C.SRLI, C.SRAI, ...
                    case 4:
                        opCode = Immediate;
                        break;

                    // C.J
                    case 5:
                        opCode = JumpAndLink;
                        break;

                    // C.BEQZ
                    case 6:
                        opCode = CondBrach;
                        break;

                    // C.BNEZ
                    case 7:
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

            if (payload.Op == 2 )
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

                    default:
                        string message = string.Format("RVC Opcode {0:X} and F3 {1:X} is not supported", payload.Op, payload.Funct3);
                        throw new OpCodeNotSupportedException(message);
                }
            }

            if (!opCode.HasValue)
            {
                throw new RiscVSimException("Invalid RVC Opcode detected");
            }

            
            var instruction = new Instruction(payload.Type, opCode.Value, 2);
            return instruction;
        }

        public InstructionPayload ComposePayload(Instruction ins, RvcPayload payload)
        {
            InstructionPayload instructionPayload = null;

            // Use the Rvc32 Factory for decoding

            if (ins.OpCode == Store)
            {
                instructionPayload = ComposePayloadStore(ins, payload);
            }

            // First code with the goal getting some ideas..  
            if (ins.OpCode == Load)
            {
                instructionPayload = ComposePayloadLoad(ins, payload);
            }
            
            if (ins.OpCode == JumpAndLinkRegister)
            {
                instructionPayload = ComposePayloadJalr(ins, payload);
            }

            if (ins.OpCode == Immediate)
            {
                instructionPayload = ComposePayloadImmediate(ins, payload);
            }

            return instructionPayload;
        }

        private InstructionPayload ComposePayloadImmediate(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Funct3 == CSLLI)
            {
                parser.ParseSlli(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposePayloadJalr(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Funct3 == 4)
            {
                // C.JR    (f4 = 8)
                // C.JALR  (f4 = 9)

                parser.ParseJrAndJalr(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposePayloadStore(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            if (payload.Funct3 == CSWSP)
            {
                parser.ParseSwSp(payload, p);
            }

            return p;
        }

        private InstructionPayload ComposePayloadLoad(Instruction ins, RvcPayload payload)
        {
            // Set the opcode, type and coding
            InstructionPayload p = new InstructionPayload(ins, payload.Coding);

            // Now do the rest according to the F3 type
            if (payload.Funct3 == CLWSP)
            {
                parser.ParseLwSp(payload, p);
            }

            return p;
        }
    }
}
