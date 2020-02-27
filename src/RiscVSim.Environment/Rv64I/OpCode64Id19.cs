using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id19 : OpCodeCommand
    {


        public OpCode64Id19 (IMemory memory, IRegister register) : base(memory,register)
        {

        }

        public override int Opcode => 0x19;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;

            // Get the conditions for the Push and Pop operations
            var rdLink = (rd == 1) || (rd == 5);
            var rs1Link = (rs1 == 1) || (rs1 == 5);

            Logger.Info("Opcode 19 : rd = {rd}, rs1 = {rs1}, Immediate = {imm}", rd, rs1, payload.SignedImmediate);

            // A simple jump
            if (!rdLink && !rs1Link)
            {
                SimpleJump(instruction,payload);
            }
            else
            {
                ulong address = 0;
                if (rdLink && rs1Link)
                {
                    if (rd == rs1)
                    {
                        // Push operation
                        address = CalculateAddress(payload);

                    }
                    else
                    {
                        // pop then push
                        address = CalculateAddress(payload);
                        Register.WriteUnsignedLong(rd, address);
                    }
                }
                else
                {
                    // pop
                    if (!rdLink && rs1Link)
                    {
                        address = CalculateAddress(payload);
                    }

                    // push
                    if (rdLink && !rs1Link)
                    {
                        // Assumption:  rs1 is not a link register (x1 or x5) and therfore we just do the push operation and jump
                        // TODO: Review this!
                        address = CalculateAddress(payload);

                        var currentAddress = Register.ReadUnsignedLong(Register.ProgramCounter);
                        ulong instructionLength = Convert.ToUInt64(instruction.InstructionLength);
                        var returnAddress = currentAddress + instructionLength;
                        Register.WriteUnsignedLong(payload.Rd, returnAddress);
                    }
                }

                // Jump
                if (address != 0)
                {
                    Register.WriteUnsignedLong(Register.ProgramCounter, address);
                }
                else
                {
                    throw new EncodingException("Unknown code path detected in RVI64 JALR");
                }


            }

            return false;
        }

        private void SimpleJump(Instruction instruction, InstructionPayload payload)
        {
            // The indirect jump instruction JALR (jump and link register) uses the I-type encoding. The target
            // address is obtained by adding the sign - extended 12 - bit I - immediate to the register rs1, then setting
            // the least - significant bit of the result to zero. The address of the instruction following the jump
            // (pc+4) is written to register rd. Register x0 can be used as the destination if the result is not
            // required.
            var pcIndex = Register.ProgramCounter;
            var address = CalculateAddress(payload);

            if (payload.Rd != 0)
            {
                var currentAddress = Register.ReadUnsignedLong(pcIndex);
                var instructionLength = Convert.ToUInt64(instruction.InstructionLength);
                Register.WriteUnsignedLong(payload.Rd, currentAddress + instructionLength);
            }

            // Jump!
            Register.WriteUnsignedLong(Register.ProgramCounter, address);
        }

        private ulong CalculateAddress(InstructionPayload payload)
        {
            ulong address;
            var immediate = payload.SignedImmediate;
            int rd = payload.Rd;
            var rs1 = payload.Rs1;
            var rs1Value = Register.ReadUnsignedLong(rs1);

            // Make the address a multiplier by 2 !
            //address = rs1Value + immediate;
            address = MathHelper.Add(rs1Value, immediate);
            var rest = address % 2;
            if (rest == 1)
            {
                address -= 1;
            }

            return address;
        }
    }
}
