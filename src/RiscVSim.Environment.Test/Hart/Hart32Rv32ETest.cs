using NUnit.Framework;
using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Hart
{
    public class Hart32Rv32ETest
    {
        private IHart hart;
        private TestEnvironment te;

        [SetUp]
        public void Setup()
        {
            var configuration = new HartConfiguration();
            configuration.Architecture = Architecture.Rv32E;

            hart = HartFactory.CreateHart(configuration);
            hart.Configure(configuration);

            te = new TestEnvironment();
        }

        [Test]
        public void RunTest()
        {
            hart.Init(0x100);
            te.LoadTest1(hart);
            hart.Start();

            // The test is passed if we are able to retrieve a dump of the register.
            // The opcode are tests individually
            var data = hart.GetRegisterStates();
            Assert.IsNotNull(data);
        }
    }
}
