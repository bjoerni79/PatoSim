using RiscVSim.Environment;
using RiscVSim.Environment.Exception;
using RiscVSim.Environment.Hart;
using RiscVSim.Input.OpCode;
using RiscVSim.Input.Rv;
using RiscVSim.Rv32I;
using RiscVSim.RV64I;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RiscVSim.ConsoleApp
{
    public class Host
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run(HartConfiguration config)
        {
            Logger.Info("CPU = {cpu}", config.Architecture);
            Logger.Info("Memory = {memory}", config.Memory);

            // Let#s go!"
            Console.WriteLine("## Configuration");
            Console.WriteLine("# Start : {0}", DateTime.Now.ToUniversalTime());
            Console.WriteLine("# CPU : {0}", config.Architecture);
            Console.WriteLine("# Memory : {0}", config.Memory);
            Console.WriteLine("# Debug : {0}", config.Debug);
            Console.WriteLine("# RvMode = {0}", config.RvMode);

            bool fileExists = File.Exists(config.Source);
            if (fileExists)
            {
                Console.WriteLine("# File : {0}", config.Source);
            }
            else
            {
                Console.WriteLine("## STOP : File does not exists");
            }

            var hart = Build(config);
            hart.Configure(config);

            if (config.RvMode)
            {
                // Read the RV files
                ReadRv(config, hart);
            }
            else
            {
                // If the input format is not the "rv format" use the default opcode one
                ReadOpcode(config, hart);
            }

            //
            //  !Vamos!  Alonsy! Let's go! Auf gehts! ..... Start the simulation.
            //

            var task1 = Task.Run(() => hart.Start());
            Task.WaitAll(new Task[] { task1 });

            // Wait for the end of the task or any debug stop

            Console.WriteLine("## Simulation stopped : {0}", DateTime.Now.ToUniversalTime());

            //
            // Show the states of the register and memory (?)
            //
            var registerState = hart.GetRegisterStates();
            Console.WriteLine(registerState);

            var memoryState = hart.GetMemoryState();
            Console.WriteLine(memoryState);
        }

        private IHart Build(HartConfiguration config)
        {
            IHart hart = null;

            var architecture = config.Architecture;
            if (architecture == Architecture.Rv32E || architecture == Architecture.Rv32I)
            {
                hart = new HartCore32(architecture);
            }

            if (architecture == Architecture.Rv64I)
            {
                hart = new HartCore64();
            }

            if (hart == null)
            {
                throw new RiscVSimException("Unsupported architecture detected!");
            }

            return hart;
        }

        private void ReadRv(HartConfiguration config, IHart hart)
        {
            var rvParser = new RvParser();
            var program = rvParser.Parse(config.Source);

            Console.WriteLine("\n## Program details:\n");

            // The program counter starts at 0
            ulong programCounter = Convert.ToUInt64(config.RvLoadOffset);
            hart.Init(programCounter);

            var opcodes = program.GetOpcodes();
            Console.WriteLine(program.GetOpcodeLines());
            hart.Load(programCounter, opcodes);
        }

        private void ReadOpcode(HartConfiguration config, IHart hart)
        {
            var lowLwevelParser = new Parser();
            var myProgram = lowLwevelParser.Parse(config.Source);
            Console.WriteLine("\n## Program details:\n");
            Console.WriteLine(myProgram.GetHumanReadableContent());

            //
            // Init the RISC V hart and start the simulation
            //

            hart.Init(myProgram.InitialProgramCounter);

            // Load each modules to the memory
            foreach (var subRoutineMarker in myProgram.GetSubRoutineMarker())
            {
                var data = myProgram.GetSubRoutine(subRoutineMarker);
                hart.Load(subRoutineMarker, data);
            }

        }

    }
}
