using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public sealed class TypeDecoder
    {

        public TypeDecoder()
        {
        }

        public InstructionPayload DecodeCustom (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            InstructionPayload payload;
            var isRv = (inst32Coding.First() & 0x7F) == 0x00;
            if (isRv)
            {
                payload = DecodeTypeR(instruction, inst32Coding);
            }
            else
            {
                throw new EncodingException("Unknown Type Encoding detected!");
            }

            return payload;
        }

        public InstructionPayload DecodeType (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            InstructionPayload payload;
            switch (instruction.Type)
            {
                case InstructionType.R_Type:
                    payload = DecodeTypeR(instruction, inst32Coding);
                    break;
                case InstructionType.I_Type:
                    payload = DecodeTypeI(instruction, inst32Coding);
                    break;
                case InstructionType.U_Type:
                    payload = DecodeTypeU(instruction, inst32Coding);
                    break;
                case InstructionType.J_Type:
                    payload = DecodeTypeJ(instruction, inst32Coding);
                    break;
                case InstructionType.B_Type:
                    payload = DecodeTypeB(instruction, inst32Coding);
                    break;
                case InstructionType.S_Type:
                    payload = DecodeTypeS(instruction, inst32Coding);
                    break;

                default:
                    throw new EncodingException("Unknown Type Encoding detected!");
            }

            return payload;
        }

        private int GetRd(IEnumerable<byte> ins32Coding)
        {
            int buffer;

            //  b2 b1
            buffer = ins32Coding.ElementAt(1);
            buffer <<= 8;
            buffer |= ins32Coding.First();

            // ignore opcode (5) and inst32 id (2)
            buffer >>= 7;
            var rd = buffer & 0x1F;
            return rd;
        }

        private InstructionPayload DecodeTypeJ (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            int workingBuffer;

            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);

            // fill the buffer with b4 b3 b2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;

            //
            // Always a multiplier of 2  => first bit = 0.
            //
            // Imm 1...10 : 21...30
            // Imm     11 : 20
            // Imm 12..19 : 12 ... 19
            // Imm     20 : Signed Bit
            //

            // Right shift and remove the rd bytes
            workingBuffer >>= 4;

            var rd = GetRd(inst32Coding);

            // Ready. Now extract the pieces of the immediate

            // Immm[19...12]
            int immediate = workingBuffer & 0xFF;
            immediate <<= 12;

            // Imm[11]
            workingBuffer >>= 8;
            int b11 = workingBuffer & 0x01;
            b11 <<= 11;
            immediate |= b11;

            // Imm[10...1]
            workingBuffer >>= 1;
            int b10to1 = workingBuffer & 0x3FF;
            b10to1 <<= 1;
            immediate |= b10to1;

            // Imm[20]
            workingBuffer >>= 10;
            int b20 = workingBuffer & 0x01;
            b20 <<= 20;
            immediate |= b20;

            //// shift by left for guaranteeing that we have 2 byte multiplier
            //immediate <<= 1;



            payload.Rd = rd;
            payload.SignedImmediate = MathHelper.GetSignedInteger(immediate, instruction.Type);
            return payload;
        }

        private InstructionPayload DecodeTypeS(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            int workingBuffer;

            // b4   (bit 0...7)
            // b3   (bit 8...15)
            // b2   (bit 16...23)
            // b1   (bit 24...31)
            //
            // Opcode   = 0  ... 6
            // rd       = 7  ... 11
            // funct3   = 12 ... 14
            // rs1      = 15 ... 19
            // rs2      = 20 ... 24
            // funct7   = 25 ... 31

            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);

            // decode funct3
            workingBuffer = b2;
            workingBuffer >>= 4;
            int funct3 = Convert.ToInt32(workingBuffer & 0x7);

            // decode rs1
            workingBuffer = b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;
            //  working Buffer = b3b2;
            workingBuffer >>= 7;
            workingBuffer &= 0x1F;
            int rs1 = Convert.ToInt32(workingBuffer);

            // decode rs2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer >>= 4;
            workingBuffer &= 0x1F;
            int rs2 = Convert.ToInt32(workingBuffer);

            // signed immediate
            workingBuffer = b4;
            workingBuffer >>= 1;
            workingBuffer &= 0x7F;
            int upperPart = workingBuffer;

            var immediate = upperPart;
            immediate <<= 5;
            immediate |= GetRd(inst32Coding);

            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.SignedImmediate = MathHelper.GetSignedInteger(immediate, instruction.Type);
            return payload;
        }

        private InstructionPayload DecodeTypeR(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            uint workingBuffer;

            // b4   (bit 0...7)
            // b3   (bit 8...15)
            // b2   (bit 16...23)
            // b1   (bit 24...31)
            //
            // Opcode   = 0  ... 6
            // rd       = 7  ... 11
            // funct3   = 12 ... 14
            // rs1      = 15 ... 19
            // rs2      = 20 ... 24
            // funct7   = 25 ... 31

            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);

            // decode funct3
            workingBuffer = b2;
            workingBuffer >>= 4;
            int funct3 = Convert.ToInt32(workingBuffer & 0x7);

            // decode rs1
            workingBuffer = b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;
            //  working Buffer = b3b2;
            workingBuffer >>= 7;
            workingBuffer &= 0x1F;
            int rs1 = Convert.ToInt32(workingBuffer);

            // decode rs2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer >>= 4;
            workingBuffer &= 0x1F;
            int rs2 = Convert.ToInt32(workingBuffer);

            // funct 7
            workingBuffer = b4;
            workingBuffer >>= 1;
            workingBuffer &= 0x7F;
            int funct7 = Convert.ToInt32(workingBuffer);

            var rd = GetRd(inst32Coding);

            payload.Rd = rd;
            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.Funct7 = funct7;
            return payload;
        }

        private InstructionPayload DecodeTypeB(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            int workingBuffer;

            // b4   (bit 0...7)
            // b3   (bit 8...15)
            // b2   (bit 16...23)
            // b1   (bit 24...31)
            //
            // Opcode   = 0  ... 6
            // rd       = 7  ... 11
            // funct3   = 12 ... 14
            // rs1      = 15 ... 19
            // rs2      = 20 ... 24
            // funct7   = 25 ... 31

            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);
            var b1 = bytes.ElementAt(0);

            // decode funct3
            workingBuffer = b2;
            workingBuffer >>= 4;
            int funct3 = Convert.ToInt32(workingBuffer & 0x7);

            // decode rs1
            workingBuffer = b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;
            //  working Buffer = b3b2;
            workingBuffer >>= 7;
            workingBuffer &= 0x1F;
            int rs1 = Convert.ToInt32(workingBuffer);

            // decode rs2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer >>= 4;
            workingBuffer &= 0x1F;
            int rs2 = Convert.ToInt32(workingBuffer);

            // compute the 12 bit signed integer
            int immediate;


            //
            // Read Imm[4...1]
            //
            workingBuffer = b2;
            workingBuffer <<= 8;
            workingBuffer |= b1;
            workingBuffer >>= 8; // Right shift the opcode and imm[11]
            immediate = workingBuffer & 0x0F;

            //
            // Read Imm [10...5]
            //
            workingBuffer = b4;
            workingBuffer >>= 1;
            workingBuffer &= 0x7F;
            workingBuffer <<= 4;
            immediate |= workingBuffer;

            //
            // Read Imm [11]
            //
            workingBuffer = b1;
            workingBuffer >>= 7;
            workingBuffer <<= 10;
            immediate |= workingBuffer;

            //
            //  Read Imm[12]
            //
            workingBuffer = b4;
            workingBuffer >>= 7;
            workingBuffer <<= 11;
            immediate |= workingBuffer;

            // Finally a left shift to the immediate for making sure it is based on a multiple of 2.
            // All in all a 12 Bit Signed Int
            immediate <<= 1;

            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.SignedImmediate = MathHelper.GetSignedInteger(immediate, InstructionType.B_Type);
            return payload;
        }

        private InstructionPayload DecodeTypeI (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            int workingBuffer;

            // b4   (bit 0...7)
            // b3   (bit 8...15)
            // b2   (bit 16...23)
            // b1   (bit 24...31)
            //
            // Opcode   = 0  ... 6
            // rd       = 7  ... 11
            // funct3   = 12 ... 14
            // rs1      = 15 ... 19
            // Imm.     = 20 ... 31


            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);

            // decode funct3
            workingBuffer = b2;
            workingBuffer >>= 4;
            int funct3 = Convert.ToInt32(workingBuffer & 0x7);

            // decode rs1
            workingBuffer = b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;
            //  working Buffer = b3b2;
            workingBuffer >>= 7;
            workingBuffer &= 0x1F;
            int rs1 = Convert.ToInt32(workingBuffer);

            // decode immediate
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer >>= 4;

            int immediate = MathHelper.GetSignedInteger(workingBuffer, instruction.Type);

            var rd = GetRd(inst32Coding);

            payload.Rd = rd;
            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.SignedImmediate = immediate;
            payload.UnsignedImmediate = Convert.ToUInt32(workingBuffer);
            payload.SignedImmediateComplete = Convert.ToInt32(workingBuffer);
            return payload;
        }

        private InstructionPayload DecodeTypeU(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction, inst32Coding);
            var bytes = inst32Coding;
            uint workingBuffer;

            // b4   (bit 0...7)
            // b3   (bit 8...15)
            // b2   (bit 16...23)
            // b1   (bit 24...31)
            //
            // Opcode   = 0  ... 6
            // rd       = 7  ... 11
            // funct3   = 12 ... 14
            // rs1      = 15 ... 19
            // Imm.     = 20 ... 31


            var b4 = bytes.ElementAt(3);
            var b3 = bytes.ElementAt(2);
            var b2 = bytes.ElementAt(1);

            // fill the buffer with b4 b3 b2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;

            // shift to the right and that's it. Signed bit ??
            workingBuffer >>= 4;
            uint immediate = workingBuffer;


            var rd = GetRd(inst32Coding);

            payload.Rd = rd;
            payload.UnsignedImmediate = immediate;
            return payload;
        }
    }
}
