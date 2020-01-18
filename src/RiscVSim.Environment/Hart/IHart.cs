using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    public interface IHart
    {
        void Configure(HartConfiguration configuration);

        void Init();

        void Start();
    }
}
