using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RiscVSim.Environment.Extensions
{
    public class Divider
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Architecture architecture;
        private IRegister register;
        private int boundary;
        private int defaultBufferSize;

        public Divider(Architecture architecture, IRegister register)
        {
            this.architecture = architecture;
            this.register = register;

            this.architecture = architecture;
            this.register = register;

            if (architecture == Architecture.Rv64I)
            {
                boundary = 64;
                defaultBufferSize = 8;
            }
            else
            {
                boundary = 32;
                defaultBufferSize = 4;
            }
        }


        public void Divw(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = BigInteger.Divide(rs1, rs2);

            ExtensionHelper.WriteToRegister(rd, result, 4, register);

            Logger.Debug("Div: {rs1} * {rs2} = {result}", rs1.ToString(), rs2.ToString(), result.ToString());
        }

        public void Remw(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = BigInteger.Remainder(rs1, rs2);

            ExtensionHelper.WriteToRegister(rd, result, 4, register);

            Logger.Debug("Rem: {rs1} * {rs2} = {result}", rs1.ToString(), rs2.ToString(), result.ToString());
        }

        public void Div (int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = BigInteger.Divide(rs1, rs2);

            ExtensionHelper.WriteToRegister(rd, result, defaultBufferSize, register);

            Logger.Debug("Div: {rs1} * {rs2} = {result}", rs1.ToString(), rs2.ToString(), result.ToString());
        }

        public void Rem(int rd, IEnumerable<byte> rs1Coding, IEnumerable<byte> rs2Coding)
        {
            var rs1 = new BigInteger(rs1Coding.ToArray());
            var rs2 = new BigInteger(rs2Coding.ToArray());
            var result = BigInteger.Remainder(rs1, rs2);

            ExtensionHelper.WriteToRegister(rd, result, defaultBufferSize, register);

            Logger.Debug("Rem: {rs1} * {rs2} = {result}", rs1.ToString(), rs2.ToString(), result.ToString());
        }
    }
}
