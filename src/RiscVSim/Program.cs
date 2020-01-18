using RiscVSim.Environment;
using RiscVSim.Environment.Hart;
using System;
using System.IO;
using System.Text;

namespace RiscVSim
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        Run(config);
                    }
                    catch (RiscVSimException rvEx)
                    {
                        Console.WriteLine("Risc V Simulation error thrown!\n");
                        Console.Error.WriteLine(rvEx.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Unknown error thrown\n");
                        Console.Error.WriteLine(ex.ToString());
                    }
                }
            }

        }

        private static void Run (HartConfiguration config)
        {
            // Let#s go!"
            Console.WriteLine("## Configuration");
            Console.WriteLine("#Start : {0}", DateTime.Now.ToLongDateString());
            Console.WriteLine("#CPU : {0}", config.Architecture);
            Console.WriteLine("#Memory : {0}", config.Memory);
            Console.WriteLine("#Debug : {0}", config.Debug);

            bool fileExists = File.Exists(config.Source);
            if (fileExists)
            {
                Console.WriteLine("#File : {0}", config.Source);
            }
            else
            {
                Console.WriteLine("## STOP : File does not exists");
            }
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

            bool allSet = (cpu != null) && (memory != null) && (debug != null) && (file != null);
            if (allSet)
            {
                var config = new HartConfiguration();

                var cpuMode = cpu.Split(new char[] { ':' });
                var memoryMode = memory.Split(new char[] { ':' });
                var debugMode = debug.Split(new char[] { ':' });

                //TODO....

                config.Source = file;
                return config;
                
            }

            return null;
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
            sb.AppendLine(" CPU: RV32,RV64");
            sb.AppendLine(" /Memory : Dynamic,Fixed");
            sb.AppendLine(" /Debug: On,Off");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Examples:\n");
            sb.AppendLine(" RiscVSim /CPU:RV64 /Memory:Dynamic /Debug:On myFile.S");
            sb.AppendLine(" Starts RV64I RISC-V hart with dynamic memory and debugging enabled");
            sb.AppendLine("");
            Console.WriteLine(sb.ToString());
        }
    }
}
