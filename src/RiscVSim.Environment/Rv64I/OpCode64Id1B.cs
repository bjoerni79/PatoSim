using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    public class OpCode64Id1B : OpCodeCommand
    {
        private Stack<ulong> ras;

        public OpCode64Id1B (IMemory memory, IRegister register, Stack<ulong> ras) : base(memory,register)
        {
            this.ras = ras;
        }

        public override int Opcode => 0x1B;

        public override bool Execute(Instruction instruction, InstructionPayload payload)
        {
            // JAL
            int rd = payload.Rd;

            Logger.Info("Opcode 1B : rd = {rd} , immediate = {imm}", rd, payload.SignedImmediate);

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
                var currentPc = Register.ReadUnsignedLong(pcIndex);

                // Write the next address the RAS (Return Address Stack) and the register
                var nextPc = currentPc + 4;
                ras.Push(nextPc);
                Register.WriteUnsignedLong(rd, nextPc);
            }

            Jump(payload);
            return false;
        }

        private void Jump(InstructionPayload payload)
        {
            var immediate = payload.SignedImmediate;
            var pcIndex = Register.ProgramCounter;

            var pc = Register.ReadUnsignedLong(pcIndex);
            //var newPc = pc + immediate;
            var newPc = MathHelper.Add(pc, immediate);
            Register.WriteUnsignedLong(pcIndex, newPc);
        }
    }
}
