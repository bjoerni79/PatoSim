using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.DebugSupport
{
    public class DebugConsole
    {
        private const string command_rr = "RR";
        private const string command_rw = "RW";
        private const string command_rd = "RD";

        private const string command_mr = "MR";
        private const string command_mw = "MR";

        private const string command_c = "C";
        private const string command_n = "N";
        private const string command_q = "Q";

        private IHartEnvironment environment;

        public DebugConsole(IHartEnvironment environment)
        {
            this.environment = environment;
        }

        public void OpenDebugConsole(InstructionPayload payload)
        {
            Console.WriteLine(">> Debug Console. Type h for help");
            var continueDebug = true;

            while (continueDebug)
            {
                Console.WriteLine(">> Next " + payload.GetHumanReadbleContent() + "\n");

                Console.Write(">>");
                var input = Console.ReadLine();
                var toUpper = input.ToUpper();

                if (toUpper.Equals("H"))
                {
                    ShowDebugHelp();
                }

                if (toUpper.Equals(command_rd))
                {
                    var registerdump =  environment.GetRegisterStates();
                    Console.WriteLine(registerdump);
                }

                if (toUpper.Equals("C") || toUpper.Equals("N") || toUpper.Equals("Q"))
                {
                    continueDebug = false;
                }
            }
        }

        public void ShowDebugHelp()
        {
            Console.WriteLine("\nCommands:");
            //Console.WriteLine("rr [register]");
            //Console.WriteLine("rw [register]");
            Console.WriteLine("rd");
            //Console.WriteLine();
            //Console.WriteLine("mr [address] [offset]");
            //Console.WriteLine("mw [address] [content]");
            Console.WriteLine();
            Console.WriteLine("c");
            Console.WriteLine("n");
            Console.WriteLine("q");
        }
    }
}
