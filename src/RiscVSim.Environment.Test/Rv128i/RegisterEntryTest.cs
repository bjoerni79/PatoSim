using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment.Test.Rv128i
{
    public class RegisterEntryTest
    {
        private RegisterEntry entry;

        [SetUp]
        public void Setup()
        {
            entry = new RegisterEntry(Architecture.Rv128I);
        }

        [Test]
        public void SimpleReadAndWriteWithNoSignedBitSet()
        {
            // In Little Endian
            var leBytes = new byte[] { 0xFF, 0x03 };

            BigInteger bigInt1 = new BigInteger(leBytes);
            entry.WriteBigInteger(bigInt1);

            BigInteger bigInt2 = entry.ReadBigInteger();
            var signedInt = entry.ReadInteger();
            var unsignedInt = entry.ReadUnsignedInteger();
            var signedLong = entry.ReadLong();
            var unsignedLong = entry.ReadUnsignedLong();
            var block = entry.ReadBlock();

            Assert.IsTrue(bigInt1.Equals(bigInt2));
            Assert.AreEqual(0x3FF, signedInt);
            Assert.AreEqual(0x3FF, unsignedInt);
            Assert.AreEqual(0x3FF, signedLong);
            Assert.AreEqual(0x3FF, unsignedLong);
        }

        [Test]
        public void SimpleReadAndWriteWithSignedBitSet()
        {
            // In Little Endian with Signed Bit = 1
            var leBytes = new byte[] { 0xFB };

            BigInteger bigInt1 = new BigInteger(leBytes);
            entry.WriteBigInteger(bigInt1);

            BigInteger bitInt2 = entry.ReadBigInteger();

            Assert.IsTrue(bigInt1.Equals(bitInt2));
        }

        [Test]
        public void SimpleReadAndWriteTestUint32Max()
        {
            // In Little Endian
            var leBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            // Read it without signed bit!
            var bigInt1 = new BigInteger(leBytes,true,false);
            entry.WriteBigInteger(bigInt1);

            var bigInt2 = entry.ReadBigInteger();
            Assert.IsTrue(bigInt1.Equals(bigInt2));

            var long1 = entry.ReadLong();
            var uint1 = entry.ReadUnsignedInteger();
            Assert.AreEqual(0xFFFFFFFF, long1);
            Assert.AreEqual(0xFFFFFFFF, uint1);
        }

        [Test]
        public void SimpleReadAndWriteTestUint64Max()
        {
            // In Little Endian
            var leBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            var bigInt1 = new BigInteger(leBytes, true, false);
            entry.WriteBigInteger(bigInt1);

            var bigInt2 = entry.ReadBigInteger();
            var ulong1 = entry.ReadUnsignedLong();

            Assert.IsTrue(bigInt1.Equals(bigInt2));
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, ulong1);
        }

        [Test]
        public void SimpleLargerThanUlongTest()
        {
            var leBytes = new byte[] 
            {   0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x01 };
            var bigInt1 = new BigInteger(leBytes);

            entry.WriteBigInteger(bigInt1);
            var bigInt2 = entry.ReadBigInteger();

            Assert.IsTrue(bigInt1.Equals(bigInt2));
        }

    }
}
