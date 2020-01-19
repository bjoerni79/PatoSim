using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Bootstrap
{
    public class BootstrapCore64
    {
        public IMemory Memory { get; set; }

        public IRegister Register { get; set; }

        public Hint Hint { get; set; }

        public uint BaseAddres { get; set; }

        public List<Instruction> InstructionsProcessed { get; set; }

        public List<InstructionPayload> InstructionPayloads { get; set; }

        public EndianType EndianCoding { get; set; }

        public Stack<ulong> RasStack { get; set; }

        public BootstrapCore64(Architecture architecture)
        {
            Memory = Factory.CreateDynamicMemory(architecture);
            Register = Factory.CreateRegisterRv64();
            RasStack = new Stack<ulong>();
            Hint = new Hint();
            BaseAddres = 0x100;
            EndianCoding = EndianType.Little;

            InstructionsProcessed = new List<Instruction>();
            InstructionPayloads = new List<InstructionPayload>();
        }

        public BootstrapCore64() : this(Architecture.Rv64I)
        {
        }

        public void Load(uint address, IEnumerable<byte> data)
        {
            Memory.Write(address, data);
        }

        public void Run(IEnumerable<byte> program)
        {
            // Init the core
            Memory.Write(BaseAddres, program);
            Register.WriteUnsignedInt(Register.ProgramCounter, BaseAddres);
            var decoder = new InstructionDecoder(EndianCoding);
            var typeDecoder = new TypeDecoder();

            // Create a simple bootstrap CPU
            var cpu = new Cpu64();
            cpu.AssignMemory(Memory);
            cpu.AssignRegister(Register);
            cpu.AssignHint(Hint);
            cpu.AssignRasStack(RasStack);
            cpu.Init();

            // Fetch the first instruction and run the loop
            var pc = Register.ReadUnsignedInt(Register.ProgramCounter);

            // Get the first 2 Bytes from the Base Address aka PC
            var instructionCoding = Memory.GetHalfWord(pc);
            while (ContinueIfValid(instructionCoding))
            {
                // Loop for the commands
                var instruction = decoder.Decode(instructionCoding);
                InstructionsProcessed.Add(instruction);

                // If the decoder cannot decode the parameter pattern, throw an exception
                if (instruction.Type == InstructionType.Unknown)
                {
                    string unknownOpCodeErrorMessage = String.Format("Error:  OpCode = {0}", instruction.OpCode);
                    throw new OpCodeNotSupportedException(unknownOpCodeErrorMessage)
                    {
                        Coding = instructionCoding,
                        Type = instruction.Type,
                        OpCode = instruction.OpCode,
                        RegisterDestination = instruction.RegisterDestination
                    };
                }

                // Reload the other bytes if required!

                if (instruction.InstructionLength == 4)
                {
                    // Read the complete 32 Bit instruction set for the decoding

                    var inst32Coding = Memory.GetWord(pc);
                    var payload = typeDecoder.DecodeType(instruction, inst32Coding);

                    if (payload == null)
                    {
                        throw new RiscVSimException("No Payload available!");
                    }

                    // For UnitTesting:  Add the result to the list
                    InstructionPayloads.Add(payload);


                    // Execute the command
                    cpu.Execute(instruction, payload);
                }


                // Done. Next run.
                pc = Register.ReadUnsignedInt(Register.ProgramCounter);
                instructionCoding = Memory.GetHalfWord(pc);
            }
        }

        private bool ContinueIfValid(IEnumerable<byte> instruction)
        {
            var isEmpty = instruction.All(b => b == 00);
            return !isEmpty;
        }
    }
}
