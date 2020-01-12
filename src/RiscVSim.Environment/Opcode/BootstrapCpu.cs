using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Opcode
{
    internal class BootstrapCpu : ICpu
    {
        private IMemory memory;
        private Register register;
        private OpCodeRegistry opCodeRegistry;
        private Hint hint;

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

        public void AssignRegister(Register register)
        {
            this.register = register;
        }

        public void Execute(Instruction instruction, InstructionPayload payload)
        {
            if (opCodeRegistry.IsInitialized)
            {
                new RiscVSimException("CPU is not initialized: Please call init() first!");
            }

            var curOpCode = instruction.OpCode;
            var rd = instruction.RegisterDestination;

            if (rd == 0)
            {
                if (hint != null)
                {
                    // If this is a NOP then increase counter else TODO
                    bool isNop = (payload.SignedImmediate == 0) && (payload.Rs1 == 0) && (payload.OpCode == 0x4) && (payload.Funct3 == 0);
                    if (isNop)
                    {
                        hint.IncreaseNopCounter();
                    }
                }
                else
                {
                    // TODO!
                }
            }
            else
            {
                // Execute the command now
                var opCodeCommand = opCodeRegistry.Get(curOpCode);

                if (opCodeCommand == null)
                {
                    string opCodeNotSupportedErrorMessage = String.Format("Implementation for OpCode {0} cannot be found", curOpCode);
                    throw new OpCodeNotSupportedException(opCodeNotSupportedErrorMessage);
                }

                opCodeCommand.Execute(instruction, payload);
            }

        }

        public void Init()
        {
            // Add opcode=04
            opCodeRegistry.Add(0x04, new OpCodeId04(memory, register));

            // Add opcode=0C, 0D and 05
            opCodeRegistry.Add(0x0C, new OpCode0C(memory, register));
            opCodeRegistry.Add(0x0D, new OpCode0D(memory, register));
            opCodeRegistry.Add(0x05, new OpCode05(memory, register));

            // Add abc

            // Add def

        }
    }
}
