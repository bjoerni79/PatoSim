using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id06 : OpCodeCommand
    {
        public OpCode64Id06(IMemory memory, IRegister register) : base(memory,register)
        {
            // base
        }

        public override int Opcode => 0x06;

        /*
         *  addiw   rd rs1 imm12            14..12=0 6..2=0x06 1..0=3
            slliw   rd rs1 31..25=0  shamtw 14..12=1 6..2=0x06 1..0=3
            srliw   rd rs1 31..25=0  shamtw 14..12=5 6..2=0x06 1..0=3
            sraiw   rd rs1 31..25=32 shamtw 14..12=5 6..2=0x06 1..0=3
         * 
         */

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;
            var immediate = payload.SignedImmediate;
            int f3 = payload.Funct3;

            // The *W commands have the 32 Bit boundaries. Do the computing within these boundaries and then extend to 64 Bit.
            var rs1ValueSigned = Register.ReadSignedInt(rs1);
            var rs1ValueUnsigned = Register.ReadUnsignedInt(rs1);
            int signedIntResult;
            uint unsignedIntResult;
            IEnumerable<byte> result;


            switch (f3)
            {
                // addiw
                case 0:
                    signedIntResult = rs1ValueSigned + immediate;
                    result = GetSignedLongBytes(signedIntResult);
                    break;

                // slliw
                case 1:
                    var leftShiftAmount = immediate & 0x1F; // the last 5 bytes are the shift increment;
                    unsignedIntResult = rs1ValueUnsigned << leftShiftAmount;
                    result = GetSignedLongBytes(unsignedIntResult);
                    break;

                // srliw and sraiw
                case 5:
                    // https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators

                    var rightShiftAmount = immediate & 0x1F;
                    var rightShiftMode = (immediate & 0x0400);
                    if (rightShiftMode == 0x400)
                    {
                        signedIntResult = rs1ValueSigned >> rightShiftAmount;
                        result = GetSignedLongBytes(signedIntResult);
                    }
                    else
                    {
                        unsignedIntResult = rs1ValueUnsigned >> rightShiftAmount;
                        result = GetSignedLongBytes(unsignedIntResult);
                    }
                    break;

                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, f3));
            }

            if (result != null)
            {
                Register.WriteBlock(rd, result);
            }

            return true;
        }

        private IEnumerable<byte> GetSignedLongBytes(int value)
        {
            IEnumerable<byte> result;
            var toLong = Convert.ToInt64(value);

            result = BitConverter.GetBytes(toLong);
            return result;
        }

        private IEnumerable<byte> GetSignedLongBytes(uint value)
        {
            IEnumerable<byte> result;
            var toLong = Convert.ToInt64(value);

            result = BitConverter.GetBytes(toLong);
            return result;
        }
    }
}
