using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartCore64 : HartBase
    {
        // the initial PC 
        private ulong initialPc;
        // the CPU with the Opcode
        private ICpu64 cpu;
        // the "Return Address Stack" for the jumps
        private Stack<ulong> ras;

        internal HartCore64() : base(Architecture.Rv64I)
        {

        }


        public override string GetMemoryState()
        {


            return "TODO";
        }

        public override string GetRegisterStates()
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Register States");
            int blockCount = 0;
            int registerLength = GetRegisterCount();
            for (int index = 0; index <= registerLength; index++)
            {
                var value = register.ReadUnsignedLong(index);

                if (value == 0)
                {
                    sb.AppendFormat(" {0:S5} = {1:X8}\t", registerNames32[index], value);
                }
                else
                {
                    //TODO: Highlight this somehow...
                    sb.AppendFormat("!{0:S5} = {1:X8}\t", registerNames32[index], value);
                }
                

                // Write 4 registers in a row.
                if (blockCount == 3)
                {
                    sb.AppendLine();
                    blockCount = 0;
                }
                else
                {
                    blockCount++;
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }

        protected override void InitDetails(ulong programCounter)
        {
            Logger.Info("Init hart");
            // Set the initial program counter
            initialPc = programCounter;

            // Set the CPU, register, memory and Return Address Stack (ras) and hint
            cpu = new Cpu64();
            register = Factory.CreateRegisterRv64();
            memory = Factory.CreateDynamicMemory(Architecture.Rv64I);
            ras = new Stack<ulong>();
            csrRegister = Factory.CreateCsrRegister();
            hint = new Hint();

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
            cpu.AssignHint(hint);
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

    }
}
