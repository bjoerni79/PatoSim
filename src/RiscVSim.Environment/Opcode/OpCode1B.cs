using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    /// <summary>
    /// Implements the OpCode 1B, which implements the RV32i JAL (Jump and Link) instruction
    /// </summary>
    public class OpCode1B : OpCodeCommand
    {
        private Stack<uint> rasStack;

        public OpCode1B (IMemory memory, Register register, Stack<uint> rasStack) : base(memory, register)
        {
            this.rasStack = rasStack;
        }

        public override int Opcode => 0x1B;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            // JAL
            int rd = payload.Rd;

            //
            // First filter for x0,x1 or x5. All other registers are not mentioned in the spec!
            //
            var isValidRd = (rd == 0) || (rd == 1) || (rd == 5);
            if (!isValidRd)
            {
                new RiscVSimException("JAL uses an rd, which is not 0,1 or 5");
            }

            if (rd == 0)
            {
                // Plain unconditional jump (Pseudo Op J)
            }
            else
            {
                // rd is 1 or 5
                //
                // Write the instrucion address of the next instruction to the link (x1) or the alternative link (x5) register.
                //
                int pcIndex = Register.ProgramCounter;
                var currentPc = Register.ReadUnsignedInt(pcIndex);

                // Write the next address the RAS (Return Address Stack) and the register
                var nextPc = currentPc + 4;
                rasStack.Push(nextPc);
                Register.WriteUnsignedInt(rd, nextPc );
            }

            Jump(payload);
            return false;
        }

        private void Jump(InstructionPayload payload)
        {
            var immediate = payload.SignedImmediate;
            var pcIndex = Register.ProgramCounter;

            var pc = Register.ReadSignedInt(pcIndex);
            var newPc = pc + immediate;
            Register.WriteSignedInt(pcIndex, newPc);
        }
    }
}
