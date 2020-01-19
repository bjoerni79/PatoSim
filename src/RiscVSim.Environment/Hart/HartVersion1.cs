using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartVersion1 : IHart
    {
        private HartConfiguration configuration;
        private bool isInitialized;

        //
        //  Components for the hart
        //
        // the initial PC 
        private ulong initialPc;
        // the CPU with the Opcode
        private ICpu64 cpu;
        // the memory 
        private IMemory memory;
        // The register set
        private IRegister register;
        // the "Return Address Stack" for the jumps
        private Stack<ulong> ras;

        internal HartVersion1()
        {
            isInitialized = false;
        }

        public void Configure(HartConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetMemoryState()
        {
            return "TODO";
        }

        public string GetRegisterStates()
        {
            return "TODO";
        }

        public void Init(ulong programCounter)
        {
            initialPc = programCounter;

            cpu = new Cpu64();
            register = Factory.CreateRegisterRv64();
            memory = Factory.CreateDynamicMemory(Architecture.Rv64I);
            ras = new Stack<ulong>();

            isInitialized = true;
        }

        public void Load(ulong address, IEnumerable<byte> data)
        {
            if (!isInitialized)
            {
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }


        }

        public void Start()
        {
            if (!isInitialized)
            {
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }


        }
    }
}
