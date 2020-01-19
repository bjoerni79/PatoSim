using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    public interface IHart
    {
        void Configure(HartConfiguration configuration);

        void Init(ulong programCounter);

        void Start();

        void Load(ulong address, IEnumerable<byte> data);

        string GetRegisterStates();

        string GetMemoryState();
    }
}
