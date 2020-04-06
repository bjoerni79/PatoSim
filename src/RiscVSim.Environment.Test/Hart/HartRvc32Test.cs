using NUnit.Framework;
using RiscVSim.Environment.Hart;
using RiscVSim.Rv32I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Hart
{
    public class HartRvc32Test
    {
        private IHart hart;
        private TestEnvironment te;

        [SetUp]
        public void Setup()
        {
            var configuration = new HartConfiguration();
            configuration.Architecture = Architecture.Rv32I;
            configuration.RvMode = false;

            hart = new HartCore32(Architecture.Rv32I);
            hart.Configure(configuration);

            te = new TestEnvironment();
        }

        [Test]
        public void RunGenericTest()
        {
            hart.Init(0x100);
            te.LoadRvcGenericTest(hart);
            hart.Start();

            // The test is passed if we are able to retrieve a dump of the register.
            // The opcode are tests individually
            var data = hart.GetRegisterStates();
            Assert.IsNotNull(data);
        }
    }
}
