using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RiscVSim.Input.OpCode
{
    public class Parser
    {
        private bool debugEnabled;

        public Parser() : this(false)
        {

        }

        public Parser(bool debugEnabled)
        {
            this.debugEnabled = debugEnabled;
        }

        public Program Parse (string source)
        {
            var program = new Program();


            try
            {
                // Check if not null...
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                // Check if file exists..
                var fileExists = File.Exists(source);
                if (!fileExists)
                {
                    throw new FileNotFoundException("File " + source + " cannot be found!");
                }

                // Parse it now..
                using (var textStream = File.OpenText(source))
                {
                    while (!textStream.EndOfStream)
                    {
                        var line = textStream.ReadLine();
                        //Console.WriteLine(line);

                        if (line.StartsWith("!"))
                        {
                            // Read initial PC
                            ApplyPc(program, line);
                        }

                        if (line.StartsWith("#"))
                        {
                            // Read new address value
                            StartNewBlock(program, line);
                        }

                        if (line.StartsWith(";"))
                        {
                            // Read Data..
                            AddDataToBlock(program, line);
                        }

                        if (line.Equals("nop"))
                        {
                            AddNop(program);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new ParserException("Could not parse file", ex);
            }

            return program;
        }

        private void AddNop (Program program)
        {
            program.AddData(new byte[] { 0x13, 0x00, 0x00, 0x00 });
        }

        private void AddDataToBlock(Program program, string line)
        {
            var data = line.Remove(0, 1);
            //Console.WriteLine("Processing ..." + data);

            // Value is XY where X and Y as between 0 and F ..
            var hexString = Cleanup(data);
            foreach (var curChar in hexString)
            {
                if (!isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = ConvertHexStringToByteArray(hexString);
            program.AddData(bytes);
        }

        private void StartNewBlock(Program program, string line)
        {
            var data = line.Remove(0, 1);
            //Console.WriteLine("Processing ..." + data);

            var hexString = Cleanup(data);
            if (hexString.Length % 2 == 1)
            {
                hexString = "0" + hexString;
            }

            foreach (var curChar in hexString)
            {
                if (!isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = ConvertHexStringToByteArray(hexString);
            var address = ConvertToAddress(bytes);
            program.AddNewAddressMarker(address);
        }



        private void ApplyPc(Program program, string line)
        {
            var data = line.Remove(0, 1);
            //Console.WriteLine("Processing ..." + data);

            var hexString = Cleanup(data);
            if (hexString.Length % 2 == 1)
            {
                hexString = "0" + hexString;
            }

            foreach (var curChar in hexString)
            {
                if (!isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = ConvertHexStringToByteArray(hexString);
            var address = ConvertToAddress(bytes);
            program.AddStartAddress(address);
        }


        private bool isHex(char c)
        {
            return Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F';
        }

        private string Cleanup(string data)
        {
            var sb = new StringBuilder();
            foreach (var curChar in data.ToUpper())
            {
                if (curChar != ' ')
                {
                    sb.Append(curChar);
                }
            }

            return sb.ToString();
        }

        private byte[] ConvertHexStringToByteArray(string hexString)
        {
            // https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array

            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        private uint ConvertToAddress(IEnumerable<byte> bytes)
        {
            uint address = 0;
            foreach (var curByte in bytes)
            {
                address |= curByte;
                address <<= 8;
            }

            address >>= 8;
            return address;
        }

    }
}
