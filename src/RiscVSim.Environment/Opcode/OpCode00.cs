using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiscVSim.Environment.Opcode
{
    /// <summary>
    /// Implements the Load Opcode 00
    /// </summary>
    public class OpCode00 : OpCodeCommand
    {
        public OpCode00(IMemory memory, Register register) : base(memory,register)
        {
            // base...
        }

        /*
         *  lb      rd rs1       imm12 14..12=0 6..2=0x00 1..0=3
            lh      rd rs1       imm12 14..12=1 6..2=0x00 1..0=3
            lw      rd rs1       imm12 14..12=2 6..2=0x00 1..0=3
            ld      rd rs1       imm12 14..12=3 6..2=0x00 1..0=3
            lbu     rd rs1       imm12 14..12=4 6..2=0x00 1..0=3
            lhu     rd rs1       imm12 14..12=5 6..2=0x00 1..0=3
            lwu     rd rs1       imm12 14..12=6 6..2=0x00 1..0=3
         * 
         * 
         */

        private const int lb = 0;
        private const int lh = 1;
        private const int lw = 2;
        private const int ld = 3;
        private const int lbu = 4;
        private const int lhu = 5;
        private const int lwu = 6;


        public override int Opcode => 0x00;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var signedImmediate = payload.SignedImmediate;
            var rs1Value = Register.ReadSignedInt(rs1);

            IEnumerable<byte> buffer;
            byte[] result;

            var memoryAddress = Convert.ToUInt32(rs1Value + signedImmediate);
            
            switch (payload.Funct3)
            {
                case lh:
                    // LH loads a 16-bit value from memory, then sign-extends to 32 - bits before storing in rd.
                    buffer = Memory.GetHalfWord(memoryAddress);
                    result = Prepare(buffer.ToArray(), true);
                    break;

                case lhu:
                    // LHU loads a 16-bit value from memory but then zero extends to 32 - bits before storing in rd.
                    buffer = Memory.GetHalfWord(memoryAddress);
                    result = Prepare(buffer.ToArray(), false);
                    break;

                case lb:
                    buffer = Memory.GetByte(memoryAddress);
                    result = Prepare(buffer.ToArray(), true);
                    break;

                case lbu:
                    buffer = Memory.GetByte(memoryAddress);
                    result = Prepare(buffer.ToArray(), false);
                    break;

                case lw:
                    buffer = Memory.GetWord(memoryAddress);
                    result = buffer.ToArray();
                    break;

                case lwu:
                    buffer = Memory.GetWord(memoryAddress);
                    result = buffer.ToArray();
                    break;

                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, payload.Funct3));
            }

            Register.WriteBlock(rd, result);

            return true;
        }

        private byte[] Prepare(byte[] buffer, bool isSigned)
        {
            var bufferLength = buffer.Length;
            // Take care of the signed bit. Save the information and remove it
            bool signBitDetected = false;
            if (isSigned)
            {
                var last = buffer.Last();
                if ((last & 0x80) == 0x80)
                {
                    // Save this info and remove the signed bit.
                    buffer[buffer.Length - 1] &= 0x7F; 
                    signBitDetected = true;
                }
            }

            // Fill up to 4 bytes
            var diff = 4 - bufferLength;
            List<byte> preparedBuffer = new List<byte>();
            preparedBuffer.AddRange(buffer);

            for (int zeroIndex=0; zeroIndex < diff; zeroIndex++)
            {
                preparedBuffer.Add(0x00);
            }

            // Add the signed bit at the last position
            if (isSigned && signBitDetected)
            {
                preparedBuffer[3] |= 0x80;
            }

            return preparedBuffer.ToArray();
        }
    }
}
