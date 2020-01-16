using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test
{
    public class PlaygroundTest
    {
        [Test]
        public void SignedIntTest1()
        {
            int a = 5;
            int b = -5;
            int c = 0x7FFFFFFF;
            int d = c * -1;
            int e = d * -1;
            int f = 1;
            int g = f * -1;

            var bytesA = BitConverter.GetBytes(a);
            var bytesB = BitConverter.GetBytes(b);
            var bytesC = BitConverter.GetBytes(c);
            var bytesD = BitConverter.GetBytes(d);
            var bytesE = BitConverter.GetBytes(e);
            var bytesF = BitConverter.GetBytes(f);
            var bytesG = BitConverter.GetBytes(g); 

            Assert.AreEqual(bytesA, new byte[] { 0x05, 0x00, 0x00, 0x00 });
            Assert.AreEqual(bytesB, new byte[] { 0xFB, 0xFF, 0xFF, 0xFF }); // -5 in complement based on 2

            Assert.AreEqual(bytesC, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F });
            Assert.AreEqual(bytesD, new byte[] { 0x01, 0x00, 0x00, 0x80 });
            Assert.AreEqual(bytesE, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F });
            Assert.AreEqual(bytesF, new byte[] { 0x01, 0x00, 0x00, 0x00 });
            Assert.AreEqual(bytesG, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
        }

        #region Little Endian Coding Tests

        [Test]
        public void DecodingLittleEndianTest1()
        {
            // Little Endian Coding :  Lowest First...
            var byteBlock1 = new byte[] { 0x00, 0x00, 0x00, 0x01 };
            var byteBlock2 = new byte[] { 0x00, 0x00, 0x01, 0x00 };
            var byteBlock3 = new byte[] { 0x00, 0x01, 0x00, 0x00 };
            var byteBlock4 = new byte[] { 0x01, 0x00, 0x00, 0x00 };

            var isLittleEndian = BitConverter.IsLittleEndian;
            var int1 = BitConverter.ToInt32(byteBlock1, 0x0);
            var int2 = BitConverter.ToInt32(byteBlock2, 0);
            var int3 = BitConverter.ToInt32(byteBlock3, 0);
            var int4 = BitConverter.ToInt32(byteBlock4, 0);

            // isLittleEndian = true (on my Windows 10 PC, Dot Net Core 3.1, Intel i5)
            Assert.IsTrue(isLittleEndian);
            Assert.AreEqual(int1, 0x01000000);
            Assert.AreEqual(int2, 0x00010000);
            Assert.AreEqual(int3, 0x00000100);
            Assert.AreEqual(int4, 0x00000001);
        }

        [Test]
        public void DecodingLittleEndianTest2()
        {
            // Some more excercises...

            var byteBlock1 = new byte[] { 0xFF, 0xFF, 0xFF, 0x00 };
            var byteBlock2 = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            var byteBlock3 = new byte[] { 0x80, 0x01, 0x00, 0x02 };

            var isLittleEndian = BitConverter.IsLittleEndian;
            var int1 = BitConverter.ToInt32(byteBlock1, 0x0);
            var int2 = BitConverter.ToInt32(byteBlock2, 0);
            int int3 = BitConverter.ToInt32(byteBlock3, 0);

            // isLittleEndian = true (on my Windows 10 PC, Dot Net Core 3.1, Intel i5)

            // In the coding it is the lowest first, but in the int value it is vice versa and highest first!
            Assert.IsTrue(isLittleEndian);
            Assert.AreEqual(int1, 0x00FFFFFF);
            Assert.AreEqual(int2, 0x01020304);
            Assert.AreEqual(int3, 0x02000180);

        }

        [Test]
        public void DecodingIntToLittleEndianTest1()
        {
            var int1 = 0x3;
            var int2 = 0x03B3;
            int int3 = 0x0000FF;
            int int4 = 0x00FF0000;

            var byteBlock1 = BitConverter.GetBytes(int1);
            var byteBlock2 = BitConverter.GetBytes(int2);
            var byteBlock3 = BitConverter.GetBytes(int3);
            var byteBlock4 = BitConverter.GetBytes(int4);

            Assert.IsTrue(BitConverter.IsLittleEndian);
            Assert.AreEqual(byteBlock1, new byte[] { 0x03, 0x00, 0x00, 0x00 });
            Assert.AreEqual(byteBlock2, new byte[] { 0xB3, 0x03, 0x00, 0x00 });
            Assert.AreEqual(byteBlock3, new byte[] { 0xFF, 0x00, 0x00, 0x00 });
            Assert.AreEqual(byteBlock4, new byte[] { 0x00, 0x00, 0xFF, 0x00 });
        }

        [Test]
        public void DecodingLittleEndianToLSBTest1()
        {
            var bytes = new byte[] { 0xB3, 0x01,0x00,0x00 };
            Assert.IsTrue(BitConverter.IsLittleEndian);

            var b1 = bytes.First();
            var b2 = bytes.ElementAt(1);
            var bitScheme = b1 & 0x03;
            Assert.AreEqual(bitScheme, 0x03);

            var opCode = b1 & 0x07C;
            opCode >>= 2;
            Assert.AreEqual(opCode, 0x0C);

        }

        [Test]
        public void SimpleDecodingTest1()
        {
            var i = 0x42;
            var bytes = BitConverter.GetBytes(i);

            Assert.IsTrue(BitConverter.IsLittleEndian);
            Assert.AreEqual(bytes, new byte[] { 0x42, 0x00, 0x00, 0x00 });

            var b1 = bytes.First();
            Assert.AreEqual(b1, 0x42);

            var test1 = b1 & 0x40;
            var test2 = b1 & 0x02;
            Assert.AreEqual(test1, 0x40);
            Assert.AreEqual(test2, 0x02);
        }

        #endregion
    }


}
