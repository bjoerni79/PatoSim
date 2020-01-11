using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Opcode
{

    /// <summary>
    /// Represents an OpCode and implements the execution part of it. CPUs / Cores can create instances of it and use it for the instruction processing
    /// </summary>
    public abstract class OpCodeCommand
    {
        private readonly IMemory memory;
        private readonly Register register;

        /// <summary>
        /// Creates a new OpCodeCommand
        /// </summary>
        /// <param name="memory">the memory</param>
        /// <param name="register">the register</param>
        public OpCodeCommand(IMemory memory, Register register)
        {
            this.memory = memory;
            this.register = register;
        }

        /// <summary>
        /// Returns the Opcode assigned for this set of commands
        /// </summary>
        public abstract int Opcode { get; }

        /// <summary>
        /// Executes a instruction
        /// </summary>
        /// <param name="instruction">the instruction</param>
        /// <param name="payload">the payload</param>
        public abstract void Execute(Instruction instruction, InstructionPayload payload);

        /// <summary>
        /// Gets the memory instance for the concrecte implementations
        /// </summary>
        protected IMemory Memory
        {
            get
            {
                return memory;
            }
        }

        /// <summary>
        /// Gets the registers for the concrecte implementation
        /// </summary>
        protected Register Register
        {
            get
            {
                return register;
            }
        }
    }
}
