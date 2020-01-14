using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    /// <summary>
    /// Implements the JALR (Jump and Link Register) opcode
    /// </summary>
    public class OpCode19 : OpCodeCommand
    {
        private Stack<uint> rasStack;

        public OpCode19 (IMemory memory, Register register, Stack<uint> rasStack) : base(memory, register)
        {
            this.rasStack = rasStack;
        }

        public override int Opcode => 0x19;

        public override void Execute(Instruction instruction, InstructionPayload payload)
        {
            var rd = payload.Rd;
            var rs1 = payload.Rs1;

            // Get the conditions for the Push and Pop operations
            var rdLink = (rd == 1) || (rd == 5);
            var rs1Link = (rs1 == 1) || (rs1 == 5);
            uint address;

            // A simple jump
            if (!rdLink && !rs1Link)
            {
                SimpleJump(payload);
            }

            if (rdLink && rs1Link)
            {
                if (rd == rs1)
                {
                    // push
                }
                else
                {
                    // pop
                }
            }
            else
            {
                // pop
                if (!rdLink && rs1Link)
                {
                    address = rasStack.Pop();
                    Register.WriteUnsignedInt(Register.ProgramCounter, address);
                }

                // push
                if (rdLink && !rs1Link)
                {
                    var pushAddress = CalculateAddress(payload);
                    Register.WriteSignedInt(Register.ProgramCounter, pushAddress);
                }
            }
        }

        private void SimpleJump(InstructionPayload payload)
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
                var currentAddress = Register.ReadUnsignedInt(pcIndex);
                Register.WriteUnsignedInt(payload.Rd, currentAddress + 4);
            }

            // Jump!
            Register.WriteSignedInt(Register.ProgramCounter, address);
        }

        private int CalculateAddress(InstructionPayload payload)
        {
            int address;
            var immediate = payload.SignedImmediate;
            int rd = payload.Rd;
            var rs1 = payload.Rs1;
            var rs1Value = Register.ReadSignedInt(rs1);

            // Make the address a multiplier by 2 !
            address = rs1Value + immediate;
            var rest = address % 2;
            if (rest == 1)
            {
                address -= 1;
            }

            return address;
        }
    }
}
