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

        private List<int> opCodeTypeRList;
        private List<int> opCodeTypeIList;
        private List<int> opCodeTypeUList;
        private List<int> opCodeTypeSList;


        public InstructionDecoder()
        {
            opCodeTypeRList = new List<int>();
            opCodeTypeIList = new List<int>();
            opCodeTypeUList = new List<int>();
            opCodeTypeSList = new List<int>();

            AddRType(opCodeTypeRList);
            AddIType(opCodeTypeIList);
            AddUType(opCodeTypeUList);
            AddSType(opCodeTypeSList);
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
            if (opCodeTypeRList.Contains(opCode))
            {
                type = InstructionType.R_Type;
            }

            if (opCodeTypeIList.Contains(opCode))
            {
                type = InstructionType.I_Type;
            }

            if (opCodeTypeUList.Contains(opCode))
            {
                type = InstructionType.U_Type;
            }

            if (opCodeTypeSList.Contains(opCode))
            {
                type = InstructionType.S_Type;
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

        private void AddRType(List<int> list)
        {
            list.Add(0x0C);

        }

        private void AddIType(List<int> list)
        {
            list.Add(0x04);
        }

        private void AddUType(List<int> list)
        {
            list.Add(0x0D);
            list.Add(0x05);
        }

        private void AddSType(List<int> list)
        {

        }
    }
}
