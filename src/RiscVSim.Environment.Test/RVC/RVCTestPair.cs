using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.RVC
{
    public class RvcTestPair
    {
        public RvcTestPair() : this(0,true)
        {
            
        }

        public RvcTestPair(int arch) : this (arch, true)
        {
        }

        public RvcTestPair (int arch, bool isValid)
        {
            TargetArchitecture = Architecture.Unknown;

            if (arch == 32)
            {
                TargetArchitecture = Architecture.Rv32I;
            }

            if (arch == 64)
            {
                TargetArchitecture = Architecture.Rv64I;
            }

            IsValid = isValid;
        }

        public bool IsValid { get; private set; }


        public Architecture TargetArchitecture { get; set; }

        public IEnumerable<byte> Coding { get; set; }

        public RvcPayload ExpectedPayload { get; set; }

        public InstructionPayload ExpectedPayload32 { get; set; }
    }
}
