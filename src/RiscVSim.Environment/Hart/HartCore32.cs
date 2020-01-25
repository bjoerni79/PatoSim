using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartCore32 : HartBase
    {
        // the initial PC 
        private uint initialPc;
        // the CPU with the Opcode
        private ICpu32 cpu;
        // the "Return Address Stack" for the jumps
        private Stack<uint> ras;

        internal HartCore32(Architecture architecture) : base(architecture)
        {
            var is32 = (architecture == Architecture.Rv32I) || (architecture == Architecture.Rv32E);
            if (!is32)
            {
                Logger.Error("this hart core implementation only support RV32I and RV32E");
                throw new RiscVSimException("this hart core implementation only support RV32I and RV32E");
            }
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
                var value = register.ReadUnsignedInt(index);

                if (value == 0)
                {
                    sb.AppendFormat(" {0} = {1:X4}\t", registerNames32[index], value);
                }
                else
                {
                    //TODO: Highlight this somehow...
                    sb.AppendFormat("!{0} = {1:X4}\t", registerNames32[index], value);
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
            cpu.AssignHint(hint);
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

        protected override void InitDetails(ulong programCounter)
        {
            // Set the initial program counter
            var programCounter32 = Convert.ToUInt32(programCounter);

            initialPc = programCounter32;

            // Set the CPU, register, memory and Return Address Stack (ras) and hint
            cpu = new Cpu32();
            register = Factory.CreateRegisterRv32(architecture);
            memory = Factory.CreateDynamicMemory(architecture);
            csrRegister = Factory.CreateCsrRegister();
            ras = new Stack<uint>();
            hint = new Hint();
        }
    }
}
