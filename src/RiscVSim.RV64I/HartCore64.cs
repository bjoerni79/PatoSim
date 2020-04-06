using RiscVSim.Environment;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.RV64I
{
    public class HartCore64 : HartBase
    {
        // the initial PC 
        private ulong initialPc;
        // the CPU with the Opcode
        private ICpu64 cpu;
        // the "Return Address Stack" for the jumps
        private Stack<ulong> ras;

        private RvcComposer64 composer;

        public HartCore64() : base(Architecture.Rv64I)
        {

        }


        protected override void InitDetails(ulong programCounter)
        {
            Logger.Info("Init hart");
            // Set the initial program counter
            initialPc = programCounter;

            // Set the CPU, register, memory and Return Address Stack (ras) and hint
            cpu = new Cpu64();
            register = new Register64();
            csrRegister = Factory.CreateCsrRegister();
            memory = Factory.CreateDynamicMemory(Architecture.Rv64I);
            environment = HartEnvironmentFactory.Build(Architecture.Rv64I,register, memory,csrRegister);

            composer = new RvcComposer64();

            ras = new Stack<ulong>();
            

            if (configuration.RvMode)
            {
                register.WriteUnsignedInt(2, 0x10000);
            }

            register.WriteUnsignedLong(3, programCounter);
        }

        public override void Load(ulong address, IEnumerable<byte> data)
        {
            if (!isInitialized)
            {
                Logger.Error("Please initialize the RISC-V hart first!");
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }

            Logger.Info("Load data to memory. Address = {address:X} hex, Data = {data}", address, BitConverter.ToString(data.ToArray()));

            // Store the data in the address
            memory.Write(address, data);
        }

        protected override void BootCpu()
        {
            Logger.Info("Boot CPU");
            // OK. Boot up the CPU first.
            cpu.AssignMemory(memory);
            cpu.AssignRegister(register);
            cpu.AssignEEI(environment);
            cpu.AssignRasStack(ras);
            cpu.AssignCrs(csrRegister);
            cpu.Init();

            //
            //  Set the program counter
            //
            Logger.Info("Set program counter to {pc:X}", initialPc);
            register.WriteUnsignedLong(register.ProgramCounter, initialPc);
        }

        protected override void ExecuteOpcode(Instruction instruction, InstructionPayload payload)
        {
            cpu.Execute(instruction, payload);
        }

        protected override void ExecuteRvcOpcode(RvcPayload payload)
        {
            Console.WriteLine("Rvc: OpCode = {0:X}, F3 = {1:X}, Type = {2}", payload.Op, payload.Funct3, payload.Type);

            var instruction = composer.ComposeInstruction(payload);
            var instructionPayload = composer.Compose(instruction, payload);

            ExecuteOpcode(instruction, instructionPayload);
        }
    }
}
