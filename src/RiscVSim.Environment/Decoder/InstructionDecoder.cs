using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{

    /// <summary>
    /// Decodes the opcode of the incoming instruction and ...
    /// 
    ///
    /// See chapter 1.5 , Base Instruction Length Encoding
    ///
    /// xxxxxxxx xxxxxxxx  |  xxxxxxxx xxxbbb11 = 32 Bit bbb != 111
    ///
    /// xxxxxxxx xxxxxxaa = 16 Bit aa != 11
    ///
    ///        B+2                    B
    ///
    /// </summary>
    public class InstructionDecoder
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        private byte OPCODE_FILTER = 0x7C;       // 0111 1100 = OPCODE!

        private byte OPCODE_FILTER_32 = 0x03;     // 0000 0011  => 0000 00aa where a != 11


        private Dictionary<int, InstructionType> opCodeDict;
        private EndianType endianType;

        public InstructionDecoder(EndianType endianType)
        {
            this.endianType = endianType;
            opCodeDict = new Dictionary<int, InstructionType>();

            //
            // R-Type
            //
            opCodeDict.Add(0x0C, InstructionType.R_Type);
            opCodeDict.Add(0x0E, InstructionType.R_Type);
            opCodeDict.Add(0x0B, InstructionType.R_Type); // A Extension

            //
            // I-Type  32Bit
            //
            opCodeDict.Add(0x04, InstructionType.I_Type);
            opCodeDict.Add(0x19, InstructionType.I_Type);  // JALR
            opCodeDict.Add(0x00, InstructionType.I_Type);
            opCodeDict.Add(0x03, InstructionType.I_Type);
            opCodeDict.Add(0x1C, InstructionType.I_Type); // System

            //
            // I-Type 64Bit
            //
            opCodeDict.Add(0x06, InstructionType.I_Type);

            //
            // U-Type
            //
            opCodeDict.Add(0x0D, InstructionType.U_Type);
            opCodeDict.Add(0x05, InstructionType.U_Type);

            //
            // J-Type
            //
            opCodeDict.Add(0x1B, InstructionType.J_Type); // JAL

            //
            // S-Type
            //
            opCodeDict.Add(0x08, InstructionType.S_Type); // Store

            //
            // B-Type
            //
            opCodeDict.Add(0x18, InstructionType.B_Type); // BNE..
        }


        public Instruction Decode (IEnumerable<byte> instructionCoding)
        {
            if (instructionCoding == null)
            {
                throw new ArgumentNullException("instructionCoding");
            }

            if (endianType == EndianType.Big)
            {
                throw new EncodingException("Big Endian is currently not supported!");
            }

            IEnumerable<byte> decodingBuffer = instructionCoding;
            //if (endianType == EndianType.Big)
            //{
            //    // Convert to Little Endian first!
            //    decodingBuffer = instructionCoding.Reverse();
            //}
            //else
            //{
            //    decodingBuffer = instructionCoding;
            //}

            int instLength = GetInstructionLength(decodingBuffer);

            if (instLength == 4)
            {
                // Code for 4 Bytes INST32 coding....
                var opCode = GetOpCode(decodingBuffer);
                var rd = GetDestinationRegister(decodingBuffer);

                InstructionType type = InstructionType.Unknown;
                if (opCodeDict.ContainsKey(opCode))
                {
                    type = opCodeDict[opCode];
                }

                var instruction = new Instruction(type, opCode, rd, instLength);
                return instruction;
            }
            else
            {
                // Code for 2 Bytes RVC

                var instruction = new Instruction(InstructionType.C_Unknown, 0, 0, 2);
                return instruction;
            }
        }

        private int GetInstructionLength(IEnumerable<byte> instruction)
        {
            byte opCodeByte;
            int length;

            opCodeByte = instruction.First();


            //
            //  If the bits 000b bbaa == 11, then we have b != 111 and aa=11 and a 32 bit instruction
            //
            var isInst32 = opCodeByte & OPCODE_FILTER_32;
            if (isInst32 == 0x03)
            {
                length = 4;
            }
            else
            {
                length = 2;
            }

            return length;
        }

        private int GetDestinationRegister(IEnumerable<byte> instruction)
        {
            int rd = instruction.ElementAt(1);
            byte opCodeByte = instruction.First();

            rd <<= 8;
            rd |= opCodeByte;
            rd >>= 7;
            rd &= 0x1F;

            return rd;
        }

        private int GetOpCode(IEnumerable<byte> instruction)
        {
            byte opCodeByte = instruction.First();
            var opCode = (opCodeByte & OPCODE_FILTER) >> 2;

            return opCode;
        }

    }
}
