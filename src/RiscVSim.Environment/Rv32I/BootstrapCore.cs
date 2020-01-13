using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Opcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Rv32I
{
    /// <summary>
    /// A simple core for testing all components
    /// </summary>
    public class BootstrapCore
    {
        public IMemory Memory { get; set; }

        public Register Register { get; set; }

        public Hint Hint { get; set; }

        public uint BaseAddres { get; set; }

        public List<Instruction> InstructionsProcessed { get; set; }

        public List<InstructionPayload> InstructionPayloads { get; set; }

        public EndianType EndianCoding { get; set; }

        public BootstrapCore()
        {
            Memory = Factory.CreateDynamicMemory(Architecture.Rv32I);
            Register = Factory.CreateRv32IRegister();
            Hint = new Hint();
            BaseAddres = 0x100;
            EndianCoding = EndianType.Little;

            InstructionsProcessed = new List<Instruction>();
            InstructionPayloads = new List<InstructionPayload>();
        }

        public void Run(IEnumerable<byte> program)
        {
            // Init the core
            Memory.Write(BaseAddres, program);
            Register.WriteUnsignedInt(Register.ProgramCounter, BaseAddres);
            var decoder = new InstructionDecoder(EndianCoding);
            var typeDecoder = new TypeDecoder();

            // Create a simple bootstrap CPU
            var cpu = new BootstrapCpu();
            cpu.AssignMemory(Memory);
            cpu.AssignRegister(Register);
            cpu.AssignHint(Hint);
            cpu.Init();

            // Fetch the first instruction and run the loop
            var pc = Register.ReadUnsignedInt(Register.ProgramCounter);

            // Get the first 2 Bytes from the Base Address aka PC
            var instructionCoding = Memory.GetWord(pc);
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

                    var inst32Coding = Memory.GetDoubleWord(pc);
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
                Register.NextInstruction(instruction.InstructionLength);
                pc = Register.ReadUnsignedInt(Register.ProgramCounter);
                instructionCoding = Memory.GetWord(pc);
            }
        }

        private bool ContinueIfValid(IEnumerable<byte> instruction)
        {
            var isEmpty = instruction.All(b => b == 00);
            return !isEmpty;
        }

    }
}
