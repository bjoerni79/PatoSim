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

        public BootstrapCore()
        {
            Memory = Factory.CreateDynamicMemory(Architecture.Rv32I);
            Register = Factory.CreateRv32IRegister();
            Hint = new Hint();
            BaseAddres = 0x100;

            InstructionsProcessed = new List<Instruction>();
            InstructionPayloads = new List<InstructionPayload>();
        }

        public void Run(IEnumerable<byte> program)
        {
            // Init the core
            Memory.Write(BaseAddres, program);
            Register.WriteUnsignedInt(Register.ProgramCounter, BaseAddres);
            var decoder = new InstructionDecoder();
            var typeDecoder = new TypeDecoder();

            // Create a simple bootstrap CPU
            var cpu = new BootstrapCpu();
            cpu.AssignMemory(Memory);
            cpu.AssignRegister(Register);
            cpu.AssignHint(Hint);
            cpu.Init();

            // Fetch the first instruction and run the loop
            var pc = Register.ReadUnsignedInt(Register.ProgramCounter);
            var instructionCoding = Memory.FetchInstruction(pc);
            while (ContinueIfValid(instructionCoding))
            {
                // Loop for the commands
                var instruction = decoder.Decode(instructionCoding);
                InstructionsProcessed.Add(instruction);

                // If the decoder cannot decode the parameter pattern, throw an exception
                if (instruction.Type == InstructionType.Unknown)
                {
                    string unknownOpCodeErrorMessage = String.Format("Error:  OpCode = {0}",instruction.OpCode);
                    throw new OpCodeNotSupportedException(unknownOpCodeErrorMessage)
                        { Coding = instruction.Coding, 
                          Type = instruction.Type, 
                          OpCode = instruction.OpCode, 
                          RegisterDestination = instruction.RegisterDestination
                    };
                }

                // Ready to go!
                InstructionPayload payload = null;
                if (instruction.Type == InstructionType.R_Type)
                {
                    payload = typeDecoder.DecodeTypeR(instruction);
                }

                if (instruction.Type == InstructionType.I_Type)
                {
                    payload = typeDecoder.DecodeTypeI(instruction);
                }
                
                //TODO: Add S-Type

                if (instruction.Type == InstructionType.U_Type)
                {
                    payload = typeDecoder.DecodeTypeU(instruction);
                }

                if (payload == null)
                {
                    throw new RiscVSimException ("No Payload available!");
                }

                // For UnitTesting:  Add the result to the list
                InstructionPayloads.Add(payload);


                // Execute the command
                cpu.Execute(instruction, payload);


                // Done. Next run.
                Register.NextInstruction();
                pc = Register.ReadUnsignedInt(Register.ProgramCounter);
                instructionCoding = Memory.FetchInstruction(pc);
            }
        }

        private bool ContinueIfValid(IEnumerable<byte> instruction)
        {
            var isEmpty = instruction.All(b => b == 00);
            return !isEmpty;
        }

    }
}
