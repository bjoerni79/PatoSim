using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class RvcDecoder
    {
        private Architecture architecture;

        public RvcDecoder(Architecture architecture)
        {
            this.architecture = architecture;
        }

        public RvcPayload Decode (IEnumerable<byte> rvcCoding)
        {
            RvcPayload payload = null;

            return payload;
        }

        // ...........................
    }
}
