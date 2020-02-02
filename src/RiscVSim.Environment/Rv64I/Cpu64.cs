using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Rv64I
{
    internal class Cpu64 : ICpu64
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IMemory memory;
        private IRegister register;
        private OpCodeRegistry opCodeRegistry;
        private ICsrRegister csrRegister;
        private ISystemNotifier environment;
        private Stack<ulong> rasStack;

        internal Cpu64()
        {
            opCodeRegistry = new OpCodeRegistry();
        }

        public void AssignCrs(ICsrRegister csrRegister)
        {
            this.csrRegister = csrRegister;
        }

        public void AssignEEI(ISystemNotifier environment)
        {
            this.environment = environment;
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
                Logger.Error("CPU is not initialized");
                throw new RiscVSimException("CPU is not initialized: Please call init() first!");
            }

            var curOpCode = instruction.OpCode;

            // Execute the command now
            var opCodeCommand = opCodeRegistry.Get(curOpCode);

            if (opCodeCommand == null)
            {
                string opCodeNotSupportedErrorMessage = String.Format("Implementation for OpCode {0} cannot be found", curOpCode);
                Logger.Error(opCodeNotSupportedErrorMessage);
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
            opCodeRegistry.Add(0x04, new OpCode64Id04(memory, register, environment));
            opCodeRegistry.Add(0x06, new OpCode64Id06(memory, register));

            //// Add opcode=0C, 0D and 05
            opCodeRegistry.Add(0x0C, new OpCode64Id0C(memory, register));
            opCodeRegistry.Add(0x0D, new OpCode64Id0D(memory, register));
            opCodeRegistry.Add(0x05, new OpCode64Id05(memory, register));
            opCodeRegistry.Add(0x0E, new OpCode64Id0E(memory, register));

            //// Jump Opcodes:
            ////
            //// opcode 1B (JAL), opcode 19 (JALR), opcode = 18 (BNE...)
            opCodeRegistry.Add(0x1B, new OpCode64Id1B(memory, register, rasStack));
            opCodeRegistry.Add(0x19, new OpCode64Id19(memory, register, rasStack));
            opCodeRegistry.Add(0x18, new OpCode64Id18(memory, register, rasStack));

            ////
            //// Load and Store
            ////
            opCodeRegistry.Add(0x00, new OpCode64Id00(memory, register));
            opCodeRegistry.Add(0x08, new OpCode64Id08(memory, register));

            ////
            //// FENCE
            ////
            opCodeRegistry.Add(0x03, new OpCode64Id03(memory, register));
            opCodeRegistry.Add(0x0B, new OpCode64Id0B(memory, register));

            ////
            //// System
            ////
            opCodeRegistry.Add(0x1C, new OpCode64Id1C(memory, register,csrRegister, environment));
        }
    }
}
