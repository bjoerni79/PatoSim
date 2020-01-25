using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RiscVSim.Input.Rv
{
    /// <summary>
    /// Implements the RV Parser as specified in the RISC-V assembly language by Anthony J. Dos Reis
    /// </summary>
    public class RvParser
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public RvParser()
        {

        }

        private delegate void ParseContent(string source, RvProgram program);

        public RvProgram Parse (string source)
        {
            Logger.Info("Parse file {file}", source);
            var program = new RvProgram();
            ParseContent concreteParse = null;

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

            try
            {
                var fi = new FileInfo(source);
                var extension = fi.Extension;

                if (extension.Equals(".hex"))
                {
                    concreteParse = ParseHex;
                }

                if (extension.Equals(".bin"))
                {
                    concreteParse = ParseBin;
                }

                // Something is not as expected.
                if (concreteParse == null)
                {
                    throw new ParserException("Could not read rv format. Please use a valid extension (.hex, .bin,...");
                }

                // Parse it now..
                using (var textStream = File.OpenText(source))
                {
                    // Read each line and convert it to Opcodes
                    while (!textStream.EndOfStream)
                    {
                        var line = textStream.ReadLine();
                        Logger.Debug("Reading {line}", line);

                        // Convert 
                        concreteParse(line,program);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Exception detected");
                throw;
            }

            return program;
        }

        private void ParseHex(string source, RvProgram program)
        {
            Logger.Info("Hex file coding detected");

            /*
             *  Sample:
             * 
             *  0141a283  # lw
                0181a303  # lw
                006282b3  # add
                04028000  # dout
                00000000  # halt
                00000002  # data (2)
                00000003  # data (3)
             */

            var data = source.Split(new char[] { '#' });

            var code = data[0];
            var comment = String.Empty;

            if (code.Length > 1)
            {
                comment = data[1];
            }

            code = code.Trim();
            comment = comment.Trim();

            // Write Code to the program
            //Console.WriteLine("Code = {0}, Comment = {1}", code, comment);
            program.AddOpCodeLine(code, comment);

            if (code.Length % 2 == 1)
            {
                throw new ParserException("Hexcoding must be a 2 byte pair");
            }

            // Read the bytes in big endian order and write in in little endian order
            var bytesInBigEndian = FormatHelper.ConvertHexStringToByteArray(code);
            var bytesInLittleEndian = bytesInBigEndian.Reverse();
            program.AddOpcode(bytesInLittleEndian);
        }

        private void ParseBin(string source, RvProgram program)
        {
            Logger.Info("Bin file coding detected");

            /*
             *  Sample:
             *  000000010100 00011 010 00101 0000011   # lw    
                000000011000 00011 010 00110 0000011   # lw
                0000000 00110 00101 000 00101 0110011  # add
                0000010 00000 00101 000 00000 0000000  # dout
                0000000 00000 00000 000 00000 0000000  # halt
                00000000000000000000000000000010       # data (2)
                00000000000000000000000000000011       # data (3)

             */

        }
    }
}
