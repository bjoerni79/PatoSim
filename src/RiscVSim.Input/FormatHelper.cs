using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RiscVSim.Input
{
    public static class FormatHelper
    {
        public static bool isHex(char c)
        {
            return Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F';
        }

        public static string Cleanup(string data)
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

        public static byte[] ConvertHexStringToByteArray(string hexString)
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

        public static uint ConvertToAddress(IEnumerable<byte> bytes)
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
