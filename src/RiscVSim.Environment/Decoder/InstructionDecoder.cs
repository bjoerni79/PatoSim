using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Decoder
{

    /// <summary>
    /// Decodes the opcode of the incoming instruction and ...
    /// </summary>
    public class InstructionDecoder
    {
        private byte INSTRUCION__PREFIX_OK = 0x03;  // 0000 0011 = OK
        private byte INSTRUCTON_VALID = 0x1F;       // 0001 1111 = False!
        private byte OPCODE_FILTER = 0x7C;          // 0111 1100 = OPCODE!

        private Dictionary<int, InstructionType> opCodeDict;

        public InstructionDecoder()
        {
            opCodeDict = new Dictionary<int, InstructionType>();

            //
            //  R-Type
            //
            opCodeDict.Add(0x0C, InstructionType.R_Type);

            //
            //  I-Type
            //
            opCodeDict.Add(0x04, InstructionType.I_Type);

            //
            //  U-Type
            //
            opCodeDict.Add(0x0D, InstructionType.U_Type);
            opCodeDict.Add(0x05, InstructionType.U_Type);

            //
            //  J-Type
            //

            //
            //  S-Type
            //
        }


        public Instruction Decode (IEnumerable<byte> instructionCoding)
        {
            if (instructionCoding.Count() != 4)
            {
                throw new RiscVSimException("Invalid Instruction: An instruction must have 4 bytes");
            }

            //TODO: Add Instruction Check (See patterns in the declaration!)
            var opCode = GetOpCode(instructionCoding);
            var rd = GetDestinationRegister(instructionCoding);

            InstructionType type = InstructionType.Unknown;
            if (opCodeDict.ContainsKey(opCode))
            {
                type = opCodeDict[opCode];
            }

            var instruction = new Instruction(type, opCode, rd,instructionCoding);
            return instruction;
        }


        private int GetDestinationRegister(IEnumerable<byte> instruction)
        {
            int rd = instruction.ElementAt(2);
            byte opCodeByte = instruction.Last();

            rd <<= 8;
            rd |= opCodeByte;
            rd >>= 7;
            rd &= 0x1F;

            return rd;
        }

        private int GetOpCode(IEnumerable<byte> instruction)
        {
            byte opCodeByte = instruction.Last();
            var opCode = (opCodeByte & OPCODE_FILTER) >> 2;

            return opCode;
        }

    }
}
