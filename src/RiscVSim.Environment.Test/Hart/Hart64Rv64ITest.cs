using NUnit.Framework;
using RiscVSim.Environment.Hart;
using RiscVSim.RV64I;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Test.Hart
{
    public class Hart64Rv64ITest
    {
        private IHart hart;
        private TestEnvironment te;

        [SetUp]
        public void Setup()
        {
            var configuration = new HartConfiguration();
            configuration.Architecture = Architecture.Rv64I;
            configuration.RvMode = true;

            hart = new HartCore64();
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
