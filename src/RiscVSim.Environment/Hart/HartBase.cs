using RiscVSim.Environment.Decoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Environment.Hart
{
    public abstract class HartBase : IHart
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected HartConfiguration configuration;
        protected bool isInitialized;
        protected Architecture architecture;

        // the Hint
        protected Hint hint;

        // the memory 
        protected IMemory memory;
        // The register set
        protected IRegister register;

        // the decoder
        protected InstructionDecoder instructionDecoder;
        // the type decoder
        protected TypeDecoder typeDecoder;

        public HartBase(Architecture architecture)
        {
            this.architecture = architecture;
            isInitialized = false;
        }

        public void Configure(HartConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public abstract string GetMemoryState();

        public abstract string GetRegisterStates();

        public abstract void Load(ulong address, IEnumerable<byte> data);

        protected abstract void BootCpu();

        protected abstract void ExecuteOpcode(Instruction instruction, InstructionPayload payload);

        protected abstract void InitDetails(ulong programCounter);

        public void Init(ulong programCounter)
        {
            // Set the instruction decoder and type decoder
            instructionDecoder = new InstructionDecoder(EndianType.Little);
            typeDecoder = new TypeDecoder();

            isInitialized = true;
            InitDetails(programCounter);
        }

        public void Start()
        {
            if (!isInitialized)
            {
                Logger.Error("Hart is not initialized");
                throw new RiscVSimException("Please initialize the RISC-V hart first!");
            }

            try
            {
                BootCpu();

                // All set up and start the loop
                Fetch();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }


        }


        /// <summary>
        /// The generic fetch method for all hart implementations
        /// </summary>
        private void Fetch()
        {
            // Done! 
            // Fetch the first instruction and run the loop. Get the first 2 Bytes from the Base Address aka PC
            var pc = register.ReadUnsignedInt(register.ProgramCounter);
            var instructionCoding = memory.GetHalfWord(pc);

            Logger.Info("Instruction {ins:X} fetched", BitConverter.ToString(instructionCoding.ToArray()));

            while (ContinueIfValid(instructionCoding))
            {
                // Loop for the commands
                var instruction = instructionDecoder.Decode(instructionCoding);
                Logger.Info("Instruction Detected with Opcode {opcode:X2} and Type {type}", instruction.OpCode, instruction.Type);

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
                    Logger.Info("Instruction to excecute = {ins32}", BitConverter.ToString(inst32Coding.ToArray()));

                    if (payload == null)
                    {
                        Logger.Error("No Payload available");
                        throw new RiscVSimException("No Payload available!");
                    }


                    // Execute the command
                    ExecuteOpcode(instruction, payload);
                }

                // Done. Next run.
                pc = register.ReadUnsignedInt(register.ProgramCounter);
                instructionCoding = memory.GetHalfWord(pc);
                Logger.Info("Instruction {ins:X} fetched", BitConverter.ToString(instructionCoding.ToArray()));
            }
        }

        protected int GetRegisterCount()
        {
            if (architecture == Architecture.Rv32E)
            {
                return 16;
            }

            return 32;
        }

        protected int GetRegisterLength()
        {
            if (architecture == Architecture.Rv64I)
            {
                return 8;
            }

            return 4;
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
