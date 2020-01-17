using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    public class OpCode0C : OpCodeCommand
    {
        public OpCode0C (IMemory memory, IRegister register) : base(memory,register)
        {
            // memry and register are stored in the base class
        }

        /*
         *  add     rd rs1 rs2 31..25=0  14..12=0 6..2=0x0C 1..0=3  OK
            sub     rd rs1 rs2 31..25=32 14..12=0 6..2=0x0C 1..0=3  OK

            sll     rd rs1 rs2 31..25=0  14..12=1 6..2=0x0C 1..0=3

            slt     rd rs1 rs2 31..25=0  14..12=2 6..2=0x0C 1..0=3  OK
            sltu    rd rs1 rs2 31..25=0  14..12=3 6..2=0x0C 1..0=3  OK

            xor     rd rs1 rs2 31..25=0  14..12=4 6..2=0x0C 1..0=3
            srl     rd rs1 rs2 31..25=0  14..12=5 6..2=0x0C 1..0=3
            sra     rd rs1 rs2 31..25=32 14..12=5 6..2=0x0C 1..0=3
            or      rd rs1 rs2 31..25=0  14..12=6 6..2=0x0C 1..0=3
            and     rd rs1 rs2 31..25=0  14..12=7 6..2=0x0C 1..0=3
         */

        public override int Opcode => 0x0C;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            int funct3 = payload.Funct3;
            int funct7 = payload.Funct7;
            int rd = payload.Rd;

            int rs1 = payload.Rs1;
            int rs1SignedValue = Register.ReadSignedInt(rs1);
            uint rs1UnsignedValue = Register.ReadUnsignedInt(rs1);

            int rs2 = payload.Rs2;
            int rs2SignedValue = Register.ReadSignedInt(rs2);
            uint rs2UnsignedValue = Register.ReadUnsignedInt(rs2);


            RunOp(instruction, funct3, funct7, rd, rs1, rs1SignedValue, rs1UnsignedValue, rs2SignedValue, rs2UnsignedValue);
            return true;
        }

        private void RunOp(Instruction instruction, int funct3, int funct7, int rd, int rs1, int rs1SignedValue, uint rs1UnsignedValue, int rs2SignedValue, uint rs2UnsignedValue)
        {
            int signedResult;
            uint unsignedResult;

            switch (funct3)
            {
                // Add and Sub
                case 0:

                    int result = 0;
                    if (funct7 == 0x20)
                    {
                        // sub

                        result = rs2SignedValue - rs1SignedValue;
                    }
                    else if (funct7 == 0x00)
                    {
                        // add
                        result = rs1SignedValue + rs2SignedValue;
                    }
                    else
                    {
                        throw new RiscVSimException("Funct7 code is not as expected. ");
                    }

                    Register.WriteSignedInt(rd, result);
                    break;

                //
                //  slt
                //
                case 2:
                    if (rs1SignedValue < rs2SignedValue)
                    {
                        signedResult = 1;
                    }
                    else
                    {
                        signedResult = 0;
                    }
                    Register.WriteSignedInt(rd, signedResult);
                    break;

                //
                //  sltu
                //
                //  SLTU rd, x0, rs2 sets rd to 1 if rs2 is not equal to zero, otherwise sets rd to zero (assembler pseudoinstruction SNEZ rd, rs).
                //
                case 3:
                    if (rs1 == 0)
                    {
                        // If rs1 = register 0 and rs2 != 0 then TRUE
                        if (rs2UnsignedValue != 0)
                        {
                            unsignedResult = 1;
                        }
                        else
                        {
                            unsignedResult = 0;
                        }
                    }
                    else
                    {
                        // Default implementation
                        if (rs1UnsignedValue < rs2UnsignedValue)
                        {
                            unsignedResult = 1;
                        }
                        else
                        {
                            unsignedResult = 0;
                        }
                    }
                    Register.WriteUnsignedInt(rd, unsignedResult);
                    break;

                //
                // and = 7
                //
                case 7:
                    unsignedResult = rs1UnsignedValue & rs2UnsignedValue;
                    Register.WriteUnsignedInt(rd, unsignedResult);
                    break;

                //
                // or = 6
                //
                case 6:
                    unsignedResult = rs1UnsignedValue | rs2UnsignedValue;
                    Register.WriteUnsignedInt(rd, unsignedResult);
                    break;

                //
                // xor = 4
                //
                case 4:
                    unsignedResult = rs1UnsignedValue ^ rs2UnsignedValue;
                    Register.WriteUnsignedInt(rd, unsignedResult);
                    break;

                //
                //  sll = 1
                //
                case 1:
                    var leftShiftAmount = rs2SignedValue & 0x1F; // the last 5 bytes are the shift increment;
                    unsignedResult = rs1UnsignedValue << leftShiftAmount;
                    Register.WriteUnsignedInt(rd, unsignedResult);
                    break;

                //
                //  srl / sra = 5
                //
                case 5:
                    //
                    //  See also the immediate instruction for more details.
                    //
                    var rightShiftAmount = rs2SignedValue & 0x1F; // the last 5 bytes are the shift increment;
                    if (funct7 == 0x20)
                    {
                        // Arithmetic right shift
                        signedResult = rs1SignedValue >> rightShiftAmount;
                        Register.WriteSignedInt(rd, signedResult);
                    }
                    else
                    {
                        // Logic right shift
                        unsignedResult = rs1UnsignedValue >> rightShiftAmount;
                        Register.WriteUnsignedInt(rd, unsignedResult);
                    }
                    break;

                // Error
                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, funct3));
            }
        }
    }
}
