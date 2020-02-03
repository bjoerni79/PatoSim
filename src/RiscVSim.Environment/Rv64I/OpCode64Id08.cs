using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id08 : OpCodeCommand
    {
        private const int sb = 0;
        private const int sh = 1;
        private const int sw = 2;
        private const int sd = 3;

        public OpCode64Id08 (IMemory memory, IRegister register) : base(memory,register)
        {

        }

        public override int Opcode => 0x08;

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

            Logger.Info("Opcode 08 : rs1 = {rs1}, rs2 = {rs2}, immediate = {imm}", rs1, rs2, payload.SignedImmediate);

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
                    list.Add(rs2Block.First());
                    list.Add(rs2Block.ElementAt(1));
                    list.Add(rs2Block.ElementAt(2));
                    list.Add(rs2Block.ElementAt(3));
                    break;

                case sd:
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
