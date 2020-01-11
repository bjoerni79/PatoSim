using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class RegisterEntryTest
    {
        private RegisterEntry entry;
        private Architecture architecture;

        [SetUp]
        public void Setup()
        {
            architecture = Architecture.Rv32I;
            entry = new RegisterEntry(architecture);
        }

        //[Test]
        //public void Playground1()
        //{



        //}

        [Test]
        public void WriteIntegerTest()
        {
            var value1 = 5;
            var value2 = -5;

            entry.WriteInteger(value1);
            var block1 = entry.ReadBlock();
            var returnValue1 = entry.ReadInteger();
            Assert.AreEqual(value1, returnValue1);

            entry.WriteInteger(value2);
            var block2 = entry.ReadBlock();
            var returnValue2 = entry.ReadInteger();
            Assert.AreEqual(value2, returnValue2);
        }

        [Test]
        public void WriteUnsignedIntTest1()
        {
            uint value1 = 0x03fe;
            entry.WriteUnsignedInteger(value1);
            var registerBlock = entry.ReadBlock();
            Assert.AreEqual(entry.ReadUnsignedInteger(), value1);
            Assert.AreEqual(registerBlock, new byte[] { 0xfe, 0x03,0x00,0x00 });

            uint value2 = 0x0300fe;
            entry.WriteUnsignedInteger(value2);
            registerBlock = entry.ReadBlock();
            Assert.AreEqual(registerBlock, new byte[] {0xfe,0x00,0x03,0x00 });
            Assert.AreEqual(entry.ReadUnsignedInteger(), value2);

            uint value3 = 0x00ffffff;
            entry.WriteUnsignedInteger(value3);
            registerBlock = entry.ReadBlock();
            Assert.AreEqual(registerBlock, new byte[] { 0xff, 0xff, 0xff,0x00 });
            Assert.AreEqual(entry.ReadUnsignedInteger(), value3);
        }

        [Test]
        public void WriteUnsingedIntTest2()
        {
            uint startValue = 0xFFFFFFFF;

            for (uint substract = 0; substract < 0xFFFFFFFE; substract +=0xFFFF)
            {
                uint curValue = startValue - substract;
                entry.WriteUnsignedInteger(curValue);

                var valueFromEntry = entry.ReadUnsignedInteger();
                Assert.AreEqual(curValue, valueFromEntry, "CurValue = {0:X2}", curValue);
            }
        }

        [Test]
        public void WriteUnsignedIntTest3()
        {
            uint value3 = 0x00ffffff;
            entry.WriteUnsignedInteger(value3);
            var registerBlock = entry.ReadBlock();
            Assert.AreEqual(registerBlock, new byte[] { 0xff, 0xff, 0xff,0x00 });
            Assert.AreEqual(entry.ReadUnsignedInteger(), value3);
        }

        [Test]
        public void WriteUnsignedIntTest4()
        {
            uint value3 = 0xffffff00;
            entry.WriteUnsignedInteger(value3);
            var registerBlock = entry.ReadBlock();
            Assert.AreEqual(registerBlock, new byte[] { 0x00, 0xff, 0xff, 0xff });
            Assert.AreEqual(entry.ReadUnsignedInteger(), value3);
        }

        [Test]
        public void WriteBlockTest1()
        {
            var sampleBlock = new byte[] { 0x00,0x00,0x1F, 0xF1 };
            entry.WriteBlock(sampleBlock);

            var content = entry.ReadBlock();
            Assert.AreEqual(sampleBlock, content);
        }

    }
}
