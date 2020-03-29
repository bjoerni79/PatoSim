using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{

    /// <summary>
    /// The RV32I opcode base class 
    /// </summary>
    public abstract class OpCodeCommand
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IMemory memory;
        private readonly IRegister register;

        /// <summary>
        /// Creates a new OpCodeCommand
        /// </summary>
        /// <param name="memory">the memory</param>
        /// <param name="register">the register</param>
        public OpCodeCommand(IMemory memory, IRegister register)
        {
            this.memory = memory;
            this.register = register;
        }

        public Architecture Architecture => Architecture.Rv32I;

        /// <summary>
        /// Returns the Opcode assigned for this set of commands
        /// </summary>
        public abstract int Opcode { get; }

        /// <summary>
        /// Executes a instruction
        /// </summary>
        /// <param name="instruction">the instruction</param>
        /// <param name="payload">the payload</param>
        public abstract bool Execute(Instruction instruction, InstructionPayload payload);

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
        protected IRegister Register
        {
            get
            {
                return register;
            }
        }
    }
}
