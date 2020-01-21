using RiscVSim.Environment;
using RiscVSim.Environment.Hart;
using RiscVSim.Input.OpCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RiscVSim
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

            bool fileExists = File.Exists(config.Source);
            if (fileExists)
            {
                Console.WriteLine("# File : {0}", config.Source);
            }
            else
            {
                Console.WriteLine("## STOP : File does not exists");
            }

            //
            // Read the low level file
            //
            // TODO: For now it is only this stupid little low level format (..but hey,..it works and could be useful for debugging). In future GCC-ELF for RISC-V is the target!
            var lowLwevelParser = new Parser();
            var myProgram = lowLwevelParser.Parse(config.Source);
            Console.WriteLine("\n## Program details:\n");
            Console.WriteLine(myProgram.GetHumanReadableContent());

            //
            // Init the RISC V hart and start the simulation
            //
            var hart = HartFactory.CreateHart(config);
            hart.Init(myProgram.InitialProgramCounter);

            // Load each modules to the memory
            foreach (var subRoutineMarker in myProgram.GetSubRoutineMarker())
            {
                var data = myProgram.GetSubRoutine(subRoutineMarker);
                hart.Load(subRoutineMarker, data);
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

            //var memoryState = hart.GetMemoryState();
            //Console.WriteLine(memoryState);
        }
    }
}
