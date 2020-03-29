using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Represents an RISC V CPU with access to memory and register.
    /// </summary>
    public interface ICpu
    {
        /// <summary>
        /// Executes an instruction
        /// </summary>
        /// <param name="instruction">the instruction coding</param>
        /// <param name="payload">the payload</param>
        void Execute(Instruction instruction, InstructionPayload payload);

        void ExecuteRvc(RvcPayload payload);

        /// <summary>
        /// Assigns the memory
        /// </summary>
        /// <param name="memory">the memory instance</param>
        void AssignMemory(IMemory memory);
        /// <summary>
        /// Assigns the register
        /// </summary>
        /// <param name="register">the register instance</param>
        void AssignRegister(IRegister register);

        void AssignEEI(ISystemNotifier environment);

        /// <summary>
        /// Initializes the CPU 
        /// </summary>

        void AssignCrs(ICsrRegister csrRegister);


        void Init();
    }
}
