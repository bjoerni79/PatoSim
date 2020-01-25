using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RiscVSim.Input.OpCode
{
    public class Parser
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
            Logger.Info("Parse file {file}", source);
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
                Logger.Error(ex, "Exception detected!");
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
            var hexString = FormatHelper.Cleanup(data);
            foreach (var curChar in hexString)
            {
                if (!FormatHelper.isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = FormatHelper.ConvertHexStringToByteArray(hexString);
            program.AddData(bytes);
        }

        private void StartNewBlock(Program program, string line)
        {
            var data = line.Remove(0, 1);
            //Console.WriteLine("Processing ..." + data);

            var hexString = FormatHelper.Cleanup(data);
            if (hexString.Length % 2 == 1)
            {
                hexString = "0" + hexString;
            }

            foreach (var curChar in hexString)
            {
                if (!FormatHelper.isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = FormatHelper.ConvertHexStringToByteArray(hexString);
            var address = FormatHelper.ConvertToAddress(bytes);
            program.AddNewAddressMarker(address);
        }



        private void ApplyPc(Program program, string line)
        {
            var data = line.Remove(0, 1);
            //Console.WriteLine("Processing ..." + data);

            var hexString = FormatHelper.Cleanup(data);
            if (hexString.Length % 2 == 1)
            {
                hexString = "0" + hexString;
            }

            foreach (var curChar in hexString)
            {
                if (!FormatHelper.isHex(curChar))
                {
                    throw new ParserException("Invalid content detected. Please only provide hex values");
                }
            }

            var bytes = FormatHelper.ConvertHexStringToByteArray(hexString);
            var address = FormatHelper.ConvertToAddress(bytes);
            program.AddStartAddress(address);
        }



    }
}
