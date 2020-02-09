using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Represents the EEI (Execution Environment Interface) of the RVxxx implementations
    /// </summary>
    public interface IHartEnvironment : ISystemNotifier
    {
        void ApplyOutputParameter(DebugMode debugMode, bool verbose);

        string GetRegisterStates();

        string GetMemoryState();

        void RvConsoleAction(InstructionPayload payload);
        

        /// <summary>
        /// Gets the No-Op counter
        /// </summary>
        int NopCounter { get; }

        /// <summary>
        /// Called by the Fetch module before the execution of an opcode
        /// </summary>
        /// <param name="payload">the opcode bytes and already extracted values</param>
        void NotifyBeforeExec(InstructionPayload payload);

        State GetCurrentState();

        string GetStateDescription();
    }
}
