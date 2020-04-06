using RiscVSim.Environment;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using RiscVSim.Environment.Hart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Rv32I
{
    public class HartCore32 : HartBase
    {
        // the initial PC 
        private uint initialPc;
        // the CPU with the Opcode
        private ICpu32 cpu;
        // the "Return Address Stack" for the jumps
        private Stack<uint> ras;

        private Architecture architecture;

        private RvcComposer32 composer;

        public HartCore32(Architecture architecture) : base(architecture)
        {
            this.architecture = architecture;
            var is32 = (architecture == Architecture.Rv32I) || (architecture == Architecture.Rv32E);
            if (!is32)
            {
                Logger.Error("this hart core implementation only support RV32I and RV32E");
                throw new RiscVSimException("this hart core implementation only support RV32I and RV32E");
            }
        }

        public override void Load(ulong address, IEnumerable<byte> data)
        {
            if (!isInitialized)
            {
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }

            uint address32 = Convert.ToUInt32(address);

            Logger.Info("Address = {address:X} hex, Data = {data}", address32, BitConverter.ToString(data.ToArray()));

            // Store the data in the address
            memory.Write(address32, data);
        }

        protected override void BootCpu()
        {
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
            register.WriteUnsignedInt(register.ProgramCounter, initialPc);
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

        protected override void InitDetails(ulong programCounter)
        {
            // Set the initial program counter
            var programCounter32 = Convert.ToUInt32(programCounter);

            initialPc = programCounter32;


            // Set the CPU, register, memory and Return Address Stack (ras) and hint
            cpu = new Cpu32();
            register = new Register32(architecture);
            memory = Factory.CreateDynamicMemory(architecture);
            csrRegister = Factory.CreateCsrRegister();
            environment = HartEnvironmentFactory.Build(architecture,register, memory,csrRegister);

            composer = new RvcComposer32();

            ras = new Stack<uint>();

            if (configuration.RvMode)
            {
                register.WriteUnsignedInt(2, 0x10000);
            }

            register.WriteUnsignedInt(3, programCounter32);
        }
    }
}
