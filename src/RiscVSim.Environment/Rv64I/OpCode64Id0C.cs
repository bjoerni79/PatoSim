using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id0C : OpCodeCommand
    {
        public OpCode64Id0C(IMemory memory, IRegister register) : base(memory,register)
        {

        }

        public override int Opcode => 0x0C;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var funct3 = payload.Funct3;
            var funct7 = payload.Funct7;
            var rd = payload.Rd;

            var rs1 = payload.Rs1;
            var rs1SignedValue = Register.ReadSignedLong(rs1);
            var rs1UnsignedValue = Register.ReadUnsignedLong(rs1);

            var rs2 = payload.Rs2;
            var rs2SignedValue = Register.ReadSignedInt(rs2);
            var rs2UnsignedValue = Register.ReadUnsignedInt(rs2);

            long signedResult;
            ulong unsignedResult;

            Logger.Info("Opcode 0C : rd = {rd}, rs1 = {rs1}, rs2 = {rs2}, funct3 = {f3}, funct7 = {f7}", rd, rs1, rs2, payload.Funct3, payload.Funct7);

            switch (funct3)
            {
                // Add and Sub
                case 0:

                    long result = 0;
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
                        Logger.Error("Funct7 is not as expected. F7 = {f7}", funct7);
                        throw new RiscVSimException("Funct7 code is not as expected. ");
                    }

                    Register.WriteSignedLong(rd, result);
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
                    Register.WriteSignedLong(rd, signedResult);
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
                    Register.WriteUnsignedLong(rd, unsignedResult);
                    break;

                //
                // and = 7
                //
                case 7:
                    unsignedResult = rs1UnsignedValue & rs2UnsignedValue;
                    Register.WriteUnsignedLong(rd, unsignedResult);
                    break;

                //
                // or = 6
                //
                case 6:
                    unsignedResult = rs1UnsignedValue | rs2UnsignedValue;
                    Register.WriteUnsignedLong(rd, unsignedResult);
                    break;

                //
                // xor = 4
                //
                case 4:
                    unsignedResult = rs1UnsignedValue ^ rs2UnsignedValue;
                    Register.WriteUnsignedLong(rd, unsignedResult);
                    break;

                //
                //  sll = 1
                //
                case 1:
                    var leftShiftAmount = rs2SignedValue & 0x1F; // the last 5 bytes are the shift increment;
                    unsignedResult = rs1UnsignedValue << leftShiftAmount;
                    Register.WriteUnsignedLong(rd, unsignedResult);
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
                        Register.WriteSignedLong(rd, signedResult);
                    }
                    else
                    {
                        // Logic right shift
                        unsignedResult = rs1UnsignedValue >> rightShiftAmount;
                        Register.WriteUnsignedLong(rd, unsignedResult);
                    }
                    break;


                // Error
                default:
                    throw new OpCodeNotSupportedException(String.Format("OpCode = {0}, Funct3 = {1}", instruction.OpCode, funct3));
            }

            return true;
        }
    }
}
