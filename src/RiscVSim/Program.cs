using RiscVSim.Environment;
using RiscVSim.Environment.Hart;
using RiscVSim.Input.OpCode;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RiscVSim
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Logger.Info("Simulator started");

            if (args.Length == 0 || args[0].Equals("/?"))
            {
                ShowHelp();
            }
            else
            {
                var config = ReadArgs(args);
                if (config == null)
                {
                    ShowHelp();
                }
                else
                {
                    // OK.. Start the RISC-V hart now!

                    try
                    {
                        var host = new Host();
                        host.Run(config);
                    }
                    catch (RiscVSimException rvEx)
                    {
                        Logger.Error(rvEx);
                        Console.WriteLine("Risc V Simulation error thrown!\n");
                        Console.Error.WriteLine(rvEx.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Console.Error.WriteLine("Unknown error thrown\n");
                        Console.Error.WriteLine(ex.ToString());
                    }
                }
            }

            Logger.Info("Simulator stopped");
        }

        private static HartConfiguration ReadArgs(string[] args)
        {
            string cpu = null;
            string memory = null;
            string debug = null;
            string file = null;

            foreach (var arg in args)
            {
                var toUpper = arg.ToUpper();

                // Check for CPU..
                if (toUpper.StartsWith("/CPU:"))
                {
                    cpu = arg;
                }

                // Check for Memory..
                if (toUpper.StartsWith("/MEMORY:"))
                {
                    memory = arg;
                }

                // Check for Debug..
                if (toUpper.StartsWith("/DEBUG:"))
                {
                    debug = arg;
                }

                if (!arg.StartsWith("/"))
                {
                    file = arg;
                }
            }

            // Set some defaults

            if (cpu == null)
            {
                cpu = "/CPU:RV64I";
            }

            if (memory == null)
            {
                memory = "/Memory:Dynamic";
            }

            if (debug == null)
            {
                debug = "/Debug:Off";
            }

            //
            // Build the configuraton now
            //
            var config = new HartConfiguration();

            var cpuMode = cpu.Split(new char[] { ':' });
            var memoryMode = memory.Split(new char[] { ':' });
            var debugMode = debug.Split(new char[] { ':' });

            ApplyCpu(config, cpuMode[1]);
            ApplyMemory(config, memoryMode[1]);
            ApplyDebug(config, debugMode[1]);

            config.Source = file;
            return config;
        }

        private static void ApplyCpu(HartConfiguration config, string mode)
        {
            var toUpper = mode.ToUpper();
            Architecture architecture = Architecture.Unknown;

            if (mode == "RV32I")
            {
                architecture = Architecture.Rv32I;
            }

            if (mode == "RV32E")
            {
                architecture = Architecture.Rv32E;
            }

            if (mode == "RV64I")
            {
                architecture = Architecture.Rv64I;
            }

            config.Architecture = architecture;
        }

        private static void ApplyMemory(HartConfiguration config, string mode)
        {
            var toUpper = mode.ToUpper();

            // keep the default.  others are in the queue...
        }

        private static void ApplyDebug (HartConfiguration config, string mode)
        {
            var toUpper = mode.ToUpper();

            if (mode == "ON")
            {
                config.Debug = DebugMode.Enabled;
            }
        }

        private static void ShowHelp()
        {
            StringBuilder sb = new StringBuilder("\n");
            sb.AppendLine("RiscVSim - A RISC-V simulator for .Net Core");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("");
            sb.AppendLine("RiscVSim [Option] [File]");
            sb.AppendLine("");
            sb.AppendLine("Option:\n");
            sb.AppendLine(" CPU: RV32I,RV32E,RV64I,");
            sb.AppendLine(" /Memory : Dynamic");
            sb.AppendLine(" /Debug: On,Off");
            sb.AppendLine("");
            sb.AppendLine("Default Values are");
            sb.AppendLine(" CPU = RV64I, Memory = Dynamic, Debug= Off");
            sb.AppendLine("");
            sb.AppendLine("Examples:\n");
            sb.AppendLine(" RiscVSim /CPU:RV64 /Memory:Dynamic /Debug:On myFile");
            sb.AppendLine(" Starts RV64I RISC-V hart with dynamic memory and debugging enabled");
            sb.AppendLine("");
            sb.AppendLine(" RiscVSim myfile");
            sb.AppendLine(" Start the simulator with the defaults");
            sb.AppendLine("");
            Console.WriteLine(sb.ToString());
        }
    }
}
