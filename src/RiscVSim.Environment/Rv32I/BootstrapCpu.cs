using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    internal class BootstrapCpu : ICpu
    {
        private IMemory memory;
        private IRegister register;
        private OpCodeRegistry opCodeRegistry;
        private Hint hint;
        private Stack<uint> rasStack;

        public BootstrapCpu()
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

        public void AssignRegister(IRegister register)
        {
            this.register = register;
        }

        public void AssignRasStack(Stack<uint> rasStack)
        {
            this.rasStack = rasStack;
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
            // Add opcode=04
            opCodeRegistry.Add(0x04, new OpCodeId04(memory, register, hint));

            // Add opcode=0C, 0D and 05
            opCodeRegistry.Add(0x0C, new OpCode0C(memory, register));
            opCodeRegistry.Add(0x0D, new OpCode0D(memory, register));
            opCodeRegistry.Add(0x05, new OpCode05(memory, register));

            // Jump Opcodes:
            //
            // opcode 1B (JAL), opcode 19 (JALR), opcode = 18 (BNE...)
            opCodeRegistry.Add(0x1B, new OpCode1B(memory, register, rasStack));
            opCodeRegistry.Add(0x19, new OpCode19(memory, register, rasStack));
            opCodeRegistry.Add(0x18, new OpCode18(memory, register, rasStack));

            //
            // Load and Store
            //
            opCodeRegistry.Add(0x00, new OpCode00(memory, register));
            opCodeRegistry.Add(0x08, new OpCode08(memory, register));

            //
            // FENCE
            //
            opCodeRegistry.Add(0x03, new OpCode03(memory, register));

            //
            // System
            //
            opCodeRegistry.Add(0x1C, new OpCode1C(memory, register));
        }
    }
}
