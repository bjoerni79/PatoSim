using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Input.LowLevel
{
    public class Program
    {
        private Dictionary<uint,IEnumerable<byte>> addressBlockDict;

        private uint currendAddressBlock;

        public Program()
        {
            addressBlockDict = new Dictionary<uint, IEnumerable<byte>>();
        }

        public uint InitialProgramCounter { get; private set; }

       
        public void AddData (IEnumerable<byte> content)
        {
            Console.WriteLine("Added data to Address " + currendAddressBlock);
            if (addressBlockDict.ContainsKey(currendAddressBlock))
            {
                var value = addressBlockDict[currendAddressBlock];
                addressBlockDict[currendAddressBlock] = value.Concat(content);
            }
            else
            {
                addressBlockDict.Add(currendAddressBlock, content);
            }
        }

        public void AddNewAddressMarker(uint address)
        {
            Console.WriteLine("Address " + address + " block detected");
            currendAddressBlock = address;
        }

        public void AddStartAddress (uint address)
        {
            Console.WriteLine("Start " + address + "  detected");
            InitialProgramCounter = address;
        }

        public string GetHumanReadableContent()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Initial Program Counter = {0:X}", InitialProgramCounter);
            sb.AppendLine();
            foreach (var address in addressBlockDict.Keys)
            {
                sb.AppendFormat("Address {0:X}\n", address);
                sb.AppendLine();
                foreach (var curByte in addressBlockDict[address])
                {
                    sb.AppendFormat("{0:X2} ", curByte);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
