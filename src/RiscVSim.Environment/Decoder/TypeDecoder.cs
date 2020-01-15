using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public sealed class TypeDecoder
    {

        internal TypeDecoder()
        {
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

                default:
                    throw new EncodingException("Unknown Type Encoding detected!");
            }

            return payload;
        }

        private InstructionPayload DecodeTypeJ (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
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

            // Ready. Now extract the pieces of the immediate

            var block3 = workingBuffer & 0xFF; // Imm[19:12]
            workingBuffer >>= 8;
            block3 <<= 9;

            var block2 = workingBuffer & 0x01; // Imm[11];
            workingBuffer >>= 1;
            block2 <<= 8;

            var block1 = workingBuffer & 0x3FF; // Imm[1:20]
            workingBuffer >>= 10;

            var block4 = workingBuffer & 0x01; // Imm[20] / Signed Bit

            // Step 1:  Block1
            int immediate = block1;

            // Step 2:  Add the bit Imm[11] at the correct position
            immediate |= block2;

            // Step 3:
            immediate |= block3;

            // Step 4:
            if (block4 == 0x01)
            {
                immediate *= -1;
            }

            // shift by left for guaranteeing that we have 2 byte multiplier
            immediate <<= 1;


            payload.SignedImmediate = immediate;
            return payload;
        }

        private InstructionPayload DecodeTypeR(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
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

            var payload = new InstructionPayload(instruction);
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
            int funct3 = workingBuffer & 0x7;

            // decode rs1
            workingBuffer = b3;
            workingBuffer <<= 8;
            workingBuffer |= b2;
            //  working Buffer = b3b2;
            workingBuffer >>= 7;
            workingBuffer &= 0x1F;
            int rs1 = workingBuffer;

            // decode rs2
            workingBuffer = b4;
            workingBuffer <<= 8;
            workingBuffer |= b3;
            workingBuffer >>= 4;
            workingBuffer &= 0x1F;
            int rs2 = workingBuffer;

            // compute the 12 bit signed integer
            int immediate;

            // buffer = b4
            workingBuffer = b4;

            var block4 = (workingBuffer & 0x80) >> 7; // Imm[12]
            var block2 = (workingBuffer & 0x7E) >> 1; // Imm[10:5]

            // buffer = b2b1
            workingBuffer = b2;
            workingBuffer <<= 8;
            workingBuffer |= b1;

            // remove the op codes
            workingBuffer >>= 7;
            var block3 = workingBuffer & 0x01;
            workingBuffer >>= 1;
            var block1 = workingBuffer & 0x0F;

            immediate = block1;

            //TODO: Add Block2
            block2 <<= 4;
            immediate |= block2;

            //TODO: Add Block3
            block3 <<= 10;
            immediate |= block3;

            //TODO: Add Block4
            if (block4 == 0x01)
            {
                immediate *= -1;
            }


            // Finally a left shift to the immediate for making sure it is based on a multiple of 2.
            // All in all a 12 Bit Signed Int
            immediate <<= 1;

            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.Rs2 = rs2;
            payload.SignedImmediate = immediate;
            return payload;
        }

        private InstructionPayload DecodeTypeI (Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
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
            int immediate = Convert.ToInt32(workingBuffer);

            payload.Funct3 = funct3;
            payload.Rs1 = rs1;
            payload.SignedImmediate = immediate;
            return payload;
        }

        private InstructionPayload DecodeTypeU(Instruction instruction, IEnumerable<byte> inst32Coding)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
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
            payload.UnsignedImmediate = immediate;
            return payload;
        }
    }
}
