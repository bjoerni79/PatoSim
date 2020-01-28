using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    /// <summary>
    /// Represents the EEI (Execution Environment Interface) of the RVxxx implementations
    /// </summary>
    public interface IHartEnvironment
    {
        void RvConsoleAction(InstructionPayload payload);

        /// <summary>
        /// Increases the No-Op counter
        /// </summary>
        void IncreaseNopCounter();

        /// <summary>
        /// Gets the No-Op counter
        /// </summary>
        int NopCounter { get; }
    }
}
