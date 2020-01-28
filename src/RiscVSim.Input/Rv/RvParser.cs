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

                bool isText = extension.Equals(".hex") || extension.Equals(".bin");

                if (isText)
                {
                    ParseText(source, program, extension);
                }
                else
                {
                    ParseBinary(source, program, extension);
                }

            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Exception detected");
                throw;
            }

            return program;
        }

        private void ParseBinary (string source, RvProgram program, string extension)
        {
            if (!extension.Equals(".e"))
            {
                throw new ParserException("Could not read rv format. Please use a valid extension (.hex, .bin,.e) ");
            }

            /*
             *  R - Header Start ...
             *  A 41h .. xx xx xx xx
             *  S 53h .. xx xx xx xx Starting point
             *  C - Header End...
             * 
             * block 1
             * block 2
             * ...
             * block n
             * 
             * 
             */

            using (var binaryStream = File.OpenRead(source))
            {
                // Read the header first
                byte curByte = Convert.ToByte(binaryStream.ReadByte());
                var headerBytes = new List<byte>();
                if (curByte != 'R')
                {
                    throw new ParserException("Header (R..C) expected!");
                }

                while (curByte != 'C') 
                {
                    //TODO: Read A address (if written)

                    //TODO: Read S address (if written)

                    headerBytes.Add(curByte);
                    curByte = Convert.ToByte(binaryStream.ReadByte());
                }
                headerBytes.Add(curByte);

                var headerLine = BitConverter.ToString(headerBytes.ToArray(), 0);
                program.AddHeader(headerLine);
                Logger.Debug("Header : {header", headerLine);

                // Read Inst32 Byte blocks. Not that efficient, but good for debugging.
                var buffer = new byte[4];
                while (binaryStream.Read(buffer,0,buffer.Length) > 0)
                {
                    var codeLine = BitConverter.ToString(buffer, 0);
                    Logger.Debug("Block {code} detected", codeLine);
                    program.AddOpCodeLine(codeLine, String.Empty);

                    // Add the opcode
                    program.AddOpcode(buffer);
                }
            }


        }

        private void ParseText(string source, RvProgram program, string extension)
        {
            ParseContent concreteParse = null;

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
                throw new ParserException("Could not read rv format. Please use a valid extension (.hex, .bin,.e) ");
            }

            // *.hex and *.bin are using the text format

            // Parse it now..
            using (var textStream = File.OpenText(source))
            {
                // Read each line and convert it to Opcodes
                while (!textStream.EndOfStream)
                {
                    var line = textStream.ReadLine();
                    Logger.Debug("Reading {line}", line);

                    // Convert 
                    concreteParse(line, program);
                }
            }

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

            string code, comment;
            Filter(source, out code, out comment);

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

        private void Filter(string source, out string code, out string comment)
        {
            var data = source.Split(new char[] { '#' });

            code = data[0];
            comment = String.Empty;
            if (code.Length > 1)
            {
                comment = data[1];
            }

            code = code.Trim();
            comment = comment.Trim();
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


                0000 0001 0100 0001 1010 0010 1000 0011   # lw    
                0000 0001 1000 0001 1010 0011 0000 0011   # lw
                00000000011000101000001010110011  # add
                00000100000000101000000000000000  # dout
                00000000000000000000000000000000  # halt
                00000000000000000000000000000010       # data (2)
                00000000000000000000000000000011       # data (3)
             */

            string code, comment;
            Filter(source, out code, out comment);

            program.AddOpCodeLine(code, comment);

            // Construct the coding now. 
            uint inst32Coding = 0;

            // We start with the LSB ...
            var reverseBits = code.Reverse();
            int power = 0;
            foreach (var bit in reverseBits)
            {
                // Ignore the spaces..
                if (bit != ' ')
                {
                    if (bit=='1')
                    {
                        inst32Coding += (uint)(Math.Pow(2, power));
                    }

                    // next position
                    power++;
                }

            }

            // Write the opcode

            var inst32Bytes = BitConverter.GetBytes(inst32Coding);
            program.AddOpcode(inst32Bytes);
        }
    }
}
