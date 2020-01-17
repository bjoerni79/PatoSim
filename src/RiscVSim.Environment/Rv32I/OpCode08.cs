using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class OpCode08 : OpCodeCommand32
    {
        public OpCode08 (IMemory memory, IRegister register) : base (memory,register)
        {
            // base ()
        }

        public override int Opcode => 0x08;

        /*
         *      sb     imm12hi rs1 rs2 imm12lo 14..12=0 6..2=0x08 1..0=3
                sh     imm12hi rs1 rs2 imm12lo 14..12=1 6..2=0x08 1..0=3
                sw     imm12hi rs1 rs2 imm12lo 14..12=2 6..2=0x08 1..0=3
                sd     imm12hi rs1 rs2 imm12lo 14..12=3 6..2=0x08 1..0=3
         * 
         */

        private const int sb = 0;
        private const int sh = 1;
        private const int sw = 2;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rs2 = payload.Rs2;
            var rs1 = payload.Rs1;
            
            var rs1Value = Register.ReadUnsignedInt(rs1);

            // memory address = s1 + immediate
            var rs2Block = Register.ReadBlock(rs2);
            //var memoryAddress = Convert.ToUInt32(rs1Value + payload.SignedImmediate);
            var memoryAddress = MathHelper.Add(rs1Value, payload.SignedImmediate);

            var list = new List<byte>();


            switch (payload.Funct3)
            {
                case sb:
                    // sb copies only the lowest 8 Bit
                    list.Add(rs2Block.First());
                    break;

                case sh:
                    // sh copies only the lowest 16 Bit
                    list.Add(rs2Block.First());
                    list.Add(rs2Block.ElementAt(1));
                    break;

                case sw:
                    // sw copies only the lowest 32 Bit
                    list.AddRange(rs2Block);
                    break;

                // Error
                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, payload.Funct3));
            }


            Memory.Write(memoryAddress, list);

            return true;
        }
    }
}
