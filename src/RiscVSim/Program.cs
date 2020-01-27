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

        private static string cpu = null;
        private static string memory = null;
        private static string debug = null;
        private static string file = null;
        private static string rv = null;
        private static string rvL = null;

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

                if (toUpper.StartsWith("/RVMODE:"))
                {
                    rv = arg;
                }

                if (toUpper.StartsWith("/RVMODEL:"))
                {
                    rvL = arg;
                }

                if (!arg.StartsWith("/"))
                {
                    file = arg;
                }
            }

            // Set some defaults
            LoadDefaults();


            //
            // Build the configuraton now
            //
            var config = new HartConfiguration();

            var cpuMode = cpu.Split(new char[] { ':' });
            var memoryMode = memory.Split(new char[] { ':' });
            var debugMode = debug.Split(new char[] { ':' });
            var rvMode = rv.Split(new char[] { ':' });
            var rvModeL = rvL.Split(new char[] { ':'});

            ApplyCpu(config, cpuMode[1]);
            ApplyMemory(config, memoryMode[1]);
            ApplyDebug(config, debugMode[1]);
            ApplyRvMode(config, rvMode[1]);
            ApplyRvModeL(config, rvModeL[1]);

            config.Source = file;
            return config;
        }

        private static void LoadDefaults()
        {
            var simConfig = new SimConfiguration();
            simConfig.Init();

            if (cpu == null)
            {
                cpu = "/CPU:" + simConfig.GetCpu();
            }

            if (memory == null)
            {
                memory = "/Memory:" + simConfig.GetMemory();
            }

            if (debug == null)
            {
                debug = "/Debug:" + simConfig.GetDebug();
            }

            if (rv == null)
            {
                rv = "/RvMode:" + simConfig.GetRvMode();
            }

            if (rvL == null)
            {
                rvL = "/RvModeL:" + simConfig.GetRvModeL();
            }
        }

        private static void ApplyRvModeL(HartConfiguration config, string mode)
        {
            int loadPoint = Int32.Parse(mode,System.Globalization.NumberStyles.HexNumber);
            config.RvLoadOffset = loadPoint;
        }

        private static void ApplyRvMode(HartConfiguration config, string mode)
        {
            var toUpper = mode.ToUpper();
            if (toUpper.Equals("ON"))
            {
                config.RvMode = true;
            }
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
            sb.AppendLine("Options:\n");
            sb.AppendLine(" CPU: RV32I,RV32E,RV64I,");
            sb.AppendLine(" /Memory : Dynamic");
            sb.AppendLine(" /Debug: On,Off");
            sb.AppendLine();
            sb.AppendLine("RV Options:\n");
            sb.AppendLine(" /RvMode: On,Off");
            sb.AppendLine(" /RvModeL: [Hex Value]");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("RvMode enables support for the assembler extensions and input formats as specified in the book RISC-V Assembly Langugage by Anthony J. Dos Reis");
            sb.AppendLine("The RvModeL simulates the -L switch of the original simualtion");
            sb.AppendLine();
            sb.AppendLine("Default Values are");
            sb.AppendLine(" CPU = RV64I, Memory = Dynamic, Debug= Off, RvMode=Off");
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
