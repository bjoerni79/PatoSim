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

        public InstructionPayload DecodeTypeR(Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
            var bytes = instruction.Coding;
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

            var b4 = bytes.First();
            var b3 = bytes.ElementAt(1);
            var b2 = bytes.ElementAt(2);

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

        public InstructionPayload DecodeTypeI (Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
            var bytes = instruction.Coding;
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


            var b4 = bytes.First();
            var b3 = bytes.ElementAt(1);
            var b2 = bytes.ElementAt(2);

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

        public InstructionPayload DecodeTypeU(Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            var payload = new InstructionPayload(instruction);
            var bytes = instruction.Coding;
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


            var b4 = bytes.First();
            var b3 = bytes.ElementAt(1);
            var b2 = bytes.ElementAt(2);

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
