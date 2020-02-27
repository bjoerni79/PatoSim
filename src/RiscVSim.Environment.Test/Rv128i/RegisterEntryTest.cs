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
        public void SimpleReadAndWriteTest1()
        {
            // In Little Endian
            var leBytes = new byte[] { 0xFF, 0x03 };

            BigInteger bigInt1 = new BigInteger(leBytes);
            entry.WriteBigInteger(bigInt1);

            BigInteger bigInt2 = entry.ReadBigInteger();

            Assert.IsTrue(bigInt1.Equals(bigInt2));
        }
    }
}
