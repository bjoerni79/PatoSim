using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Rv64I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    internal class HartVersion1 : IHart
    {
        private HartConfiguration configuration;
        private bool isInitialized;
        private int registerLength = 32;

        #region RISC V hart components
        //
        //  Components for the hart
        //
        // the initial PC 
        private ulong initialPc;
        // the CPU with the Opcode
        private ICpu64 cpu;
        // the memory 
        private IMemory memory;
        // The register set
        private IRegister register;
        // the "Return Address Stack" for the jumps
        private Stack<ulong> ras;
        // the Hint
        private Hint hint;

        // the decoder
        private InstructionDecoder instructionDecoder;
        // the type decoder
        private TypeDecoder typeDecoder;

        #endregion

        internal HartVersion1()
        {
            isInitialized = false;
        }

        public void Configure(HartConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetMemoryState()
        {


            return "TODO";
        }

        public string GetRegisterStates()
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Register States");
            int blockCount = 0;
            for (int index = 0; index <= registerLength; index++)
            {
                var value = register.ReadUnsignedLong(index);

                if (value == 0)
                {
                    sb.AppendFormat("X{0:D2} = {1:X}, ", index, value);
                }
                else
                {
                    //TODO: Highlight this somehow...
                    sb.AppendFormat("X{0:D2} = {1:X}, ", index, value);
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

        public void Init(ulong programCounter)
        {
            // Set the initial program counter
            initialPc = programCounter;

            // Set the CPU, register, memory and Return Address Stack (ras) and hint
            cpu = new Cpu64();
            register = Factory.CreateRegisterRv64();
            memory = Factory.CreateDynamicMemory(Architecture.Rv64I);
            ras = new Stack<ulong>();
            hint = new Hint();

            // Set the instruction decoder and type decoder
            instructionDecoder = new InstructionDecoder(EndianType.Little);
            typeDecoder = new TypeDecoder();

            isInitialized = true;
        }

        public void Load(ulong address, IEnumerable<byte> data)
        {
            if (!isInitialized)
            {
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }

            // Store the data in the address
            memory.Write(address, data);
        }

        public void Start()
        {
            if (!isInitialized)
            {
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }

            // OK. Boot up the CPU first.
            cpu.AssignMemory(memory);
            cpu.AssignRegister(register);
            cpu.AssignHint(hint);
            cpu.AssignRasStack(ras);
            cpu.Init();

            //
            //  Set the program counter
            //
            register.WriteUnsignedLong(register.ProgramCounter, initialPc);

            // Done! 
            // Fetch the first instruction and run the loop. Get the first 2 Bytes from the Base Address aka PC

            var pc = register.ReadUnsignedInt(register.ProgramCounter);
            var instructionCoding = memory.GetHalfWord(pc);

            while (ContinueIfValid(instructionCoding))
            {
                // Loop for the commands
                var instruction = instructionDecoder.Decode(instructionCoding);

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

                if (instruction.InstructionLength == 4)
                {
                    // Read the complete 32 Bit instruction set for the decoding

                    var inst32Coding = memory.GetWord(pc);
                    var payload = typeDecoder.DecodeType(instruction, inst32Coding);

                    if (payload == null)
                    {
                        throw new RiscVSimException("No Payload available!");
                    }


                    // Execute the command
                    cpu.Execute(instruction, payload);
                }

                // Done. Next run.
                pc = register.ReadUnsignedInt(register.ProgramCounter);
                instructionCoding = memory.GetHalfWord(pc);
            }
        }

        /// <summary>
        /// Left over from the Bootstrap Core... is there a better way of doing this?
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool ContinueIfValid(IEnumerable<byte> instruction)
        {
            var isEmpty = instruction.All(b => b == 00);
            return !isEmpty;
        }
    }
}
