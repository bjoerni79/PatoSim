using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    /// <summary>
    /// Represents a RISC-V hart (CPU with Instruction fetch)
    /// </summary>
    public interface IHart
    {
        /// <summary>
        /// Configures a hart
        /// </summary>
        /// <param name="configuration">the configuration</param>
        void Configure(HartConfiguration configuration);

        /// <summary>
        /// Inits a hart
        /// </summary>
        /// <remarks>RV32I uses a 32 Bit (4 Byte) address space. For the reason of compabillity a RV64 ready data type is used </remarks>
        /// <param name="programCounter">the program counter start</param>
        void Init(ulong programCounter);

        /// <summary>
        /// Starts a hart
        /// </summary>
        void Start();

        /// <summary>
        /// Loads data to an address block. This can be instructions or data
        /// </summary>
        /// <param name="address">the address</param>
        /// <param name="data">the data block</param>
        void Load(ulong address, IEnumerable<byte> data);

        /// <summary>
        /// Returns an overview of the registers
        /// </summary>
        /// <returns>A string</returns>
        string GetRegisterStates();

        /// <summary>
        /// Returns an overview of the memory usage
        /// </summary>
        /// <returns>A string</returns>
        string GetMemoryState();
    }
}
