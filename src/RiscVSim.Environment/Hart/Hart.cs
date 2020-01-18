using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class Hart : IHart
    {
        private HartConfiguration configuration;

        internal Hart()
        {

        }

        public void Configure(HartConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init()
        {

        }

        public void Start()
        {

        }
    }
}
