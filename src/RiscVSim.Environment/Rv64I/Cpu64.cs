using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    internal class Cpu64 : ICpu64
    {
        private IMemory memory;
        private IRegister register;
        private OpCodeRegistry opCodeRegistry;
        private Hint hint;
        private Stack<ulong> rasStack;

        internal Cpu64()
        {
            opCodeRegistry = new OpCodeRegistry();
        }

        public void AssignHint(Hint hint)
        {
            this.hint = hint;
        }

        public void AssignMemory(IMemory memory)
        {
            this.memory = memory;
        }

        public void AssignRasStack(Stack<ulong> rasStack)
        {
            this.rasStack = rasStack;
        }

        public void AssignRegister(IRegister register)
        {
            this.register = register;
        }

        public void Execute(Instruction instruction, InstructionPayload payload)
        {
            if (!opCodeRegistry.IsInitialized)
            {
                new RiscVSimException("CPU is not initialized: Please call init() first!");
            }

            var curOpCode = instruction.OpCode;

            // Execute the command now
            var opCodeCommand = opCodeRegistry.Get(curOpCode);

            if (opCodeCommand == null)
            {
                string opCodeNotSupportedErrorMessage = String.Format("Implementation for OpCode {0} cannot be found", curOpCode);
                throw new OpCodeNotSupportedException(opCodeNotSupportedErrorMessage);
            }

            var incPc = opCodeCommand.Execute(instruction, payload);
            if (incPc)
            {
                register.NextInstruction(instruction.InstructionLength);
            }
        }

        public void Init()
        {
            // Init the OpCodes here!

            // Add opcode=04
            opCodeRegistry.Add(0x04, new OpCode64Id04(memory, register, hint));
            opCodeRegistry.Add(0x06, new OpCode64Id06(memory, register));

            //// Add opcode=0C, 0D and 05
            opCodeRegistry.Add(0x0C, new OpCode64Id0C(memory, register));
            opCodeRegistry.Add(0x0D, new OpCode64Id0D(memory, register));
            opCodeRegistry.Add(0x05, new OpCode64Id05(memory, register));

            //// Jump Opcodes:
            ////
            //// opcode 1B (JAL), opcode 19 (JALR), opcode = 18 (BNE...)
            opCodeRegistry.Add(0x1B, new OpCode64Id1B(memory, register, rasStack));
            opCodeRegistry.Add(0x19, new OpCode64Id19(memory, register, rasStack));
            opCodeRegistry.Add(0x18, new OpCode64Id18(memory, register, rasStack));

            ////
            //// Load and Store
            ////
            //opCodeRegistry.Add(0x00, new OpCode00(memory, register));
            //opCodeRegistry.Add(0x08, new OpCode08(memory, register));

            ////
            //// FENCE
            ////
            //opCodeRegistry.Add(0x03, new OpCode03(memory, register));

            ////
            //// System
            ////
            //opCodeRegistry.Add(0x1C, new OpCode1C(memory, register));
        }
    }
}
