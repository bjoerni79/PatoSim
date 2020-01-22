using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    /// <summary>
    /// Represents a simple container for the payload of the instruction.
    /// </summary>
    public sealed class InstructionPayload
    {
        internal InstructionPayload(Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException("instruction");
            }

            Type = instruction.Type;
            OpCode = instruction.OpCode;
            Rd = instruction.RegisterDestination;

        }

        public int Rd { get; private set; }

        public int OpCode { get; private set; }

        public InstructionType Type { get; private set; }



        public int Rs1 { get; internal set; }

        public int Rs2 { get; internal set; }



        public int Funct3 { get; internal set; }

        public int Funct7 { get; internal set; }

        public int SignedImmediate { get; internal set; }

        public uint UnsignedImmediate { get; internal set; }

        /// <summary>
        /// All bits are treated as values bits 
        /// </summary>
        public int SignedImmediateComplete { get; internal set; }
    }
}
