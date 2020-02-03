using NUnit.Framework;
using RiscVSim.Environment.Exception;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Rv32i
{
    public class RegisterTest1
    {
        private IRegister register;

        [SetUp]
        public void Setup()
        {
            register = Factory.CreateRegisterRv32(Architecture.Rv32I);
        }

        //TODO: Test SignedInt!

        [Test]
        public void ReadTest1()
        {
            var enums = new RegisterName[]
            {
                RegisterName.x0,
                RegisterName.x1,
                RegisterName.x2,
                RegisterName.x3,
                RegisterName.x4,
                RegisterName.x5,
                RegisterName.x6,
                RegisterName.x7,
                RegisterName.x8,
                RegisterName.x9,
                RegisterName.x10,
                RegisterName.x11,
                RegisterName.x12,
                RegisterName.x13,
                RegisterName.x14,
                RegisterName.x15,
                RegisterName.x16,
                RegisterName.x17,
                RegisterName.x18,
                RegisterName.x19,
                RegisterName.x20,
                RegisterName.x21,
                RegisterName.x22,
                RegisterName.x23,
                RegisterName.x24,
                RegisterName.x25,
                RegisterName.x26,
                RegisterName.x27,
                RegisterName.x28,
                RegisterName.x29,
                RegisterName.x30,
                RegisterName.x31,
                RegisterName.x32,
            };

            // The initial value for all register is zero!
            foreach (var curValue in enums)
            {
                var initValue = register.ReadUnsignedInt(curValue);
                Assert.AreEqual(initValue, 0);
            }
        }

        [Test]
        public void ModifyX0Test()
        {
            Assert.Catch(typeof(RiscVSimException), ModifyX0Test_Delegate);
        }

        private void ModifyX0Test_Delegate()
        {
            uint value = 1;

            register.WriteUnsignedInt(RegisterName.x0, value);
        }

        [Test]
        public void ModifyOtherThanX0Test()
        {
            var enums = new RegisterName[]
{
                RegisterName.x1,
                RegisterName.x2,
                RegisterName.x3,
                RegisterName.x4,
                RegisterName.x5,
                RegisterName.x6,
                RegisterName.x7,
                RegisterName.x8,
                RegisterName.x9,
                RegisterName.x10,
                RegisterName.x11,
                RegisterName.x12,
                RegisterName.x13,
                RegisterName.x14,
                RegisterName.x15,
                RegisterName.x16,
                RegisterName.x17,
                RegisterName.x18,
                RegisterName.x19,
                RegisterName.x20,
                RegisterName.x21,
                RegisterName.x22,
                RegisterName.x23,
                RegisterName.x24,
                RegisterName.x25,
                RegisterName.x26,
                RegisterName.x27,
                RegisterName.x28,
                RegisterName.x29,
                RegisterName.x30,
                RegisterName.x31,
                RegisterName.x32,
            };

            // Write the values starting with 10
            uint value = 10;
            int expectedValue = 10;

            foreach (var curValue in enums)
            {
                register.WriteUnsignedInt(curValue, value);
                value++;
            }

            // Now again read all of them again and verify if they are all different!

            foreach (var curValue in enums)
            {
                var content = register.ReadUnsignedInt(curValue);
                Assert.AreEqual(content, expectedValue);                

                expectedValue++;
            }

        }

        [Test]
        public void ProgramCounterTest()
        {
            uint programCounterValue = 100;

            register.WriteUnsignedInt(RegisterName.x32, programCounterValue);

            uint programCounterValue1 = register.ReadUnsignedInt(register.ProgramCounter); ;
            Assert.AreEqual(programCounterValue, programCounterValue1);

        }
    }
}
