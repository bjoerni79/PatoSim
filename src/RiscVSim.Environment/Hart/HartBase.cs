using RiscVSim.Environment.Custom;
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

        protected ICsrRegister csrRegister;

        protected bool rvMode;

        // the decoder
        protected InstructionDecoder instructionDecoder;
        // the type decoder
        protected TypeDecoder typeDecoder;

        protected string[] registerNames32 = new string[]
        {
            "zero","ra","sp","gp","tp","t0","t1","t2",
            "s0","s1", "a0","a1","a2","a3","a4","a5",
            "a6","a7","s2","s3","s4","s5","s6","s7",
            "s8","s9","s10","s11","t3","t4","t5","t6",
            "pc"
        };

        public HartBase(Architecture architecture)
        {
            this.architecture = architecture;
            isInitialized = false;
        }

        public void Configure(HartConfiguration configuration)
        {
            this.configuration = configuration;
            this.rvMode = configuration.RvMode;
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

                if (rvMode)
                {
                    var isCustom = instructionCoding.First() == 0x00;
                    if (isCustom)
                    {
                        Logger.Info("Processing RV custom command.");
                        var customCoding = memory.GetWord(pc);

                        var isHalt = customCoding.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        if (isHalt)
                        {
                            // Stop the simulation!
                            break;
                        }


                        var payload = typeDecoder.DecodeCustom(instruction, customCoding);

                        var rvOpcode = new RvOpcode(memory, register);
                        var inc = rvOpcode.Execute(instruction, payload);

                        if (inc)
                        {
                            // Go to the next instruction...
                            register.NextInstruction(4);
                        }

                    }
                    else
                    {
                        // Handle instruction as usual..
                        HandleCompliantMode(pc, instructionCoding, instruction);
                    }


                }
                else
                {
                    HandleCompliantMode(pc, instructionCoding, instruction);
                }

                // Done. Next run.
                pc = register.ReadUnsignedInt(register.ProgramCounter);
                instructionCoding = memory.GetHalfWord(pc);
                Logger.Info("Instruction {ins:X} fetched", BitConverter.ToString(instructionCoding.ToArray()));
            }
        }

        private void HandleCompliantMode(uint pc, IEnumerable<byte> instructionCoding, Instruction instruction)
        {
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
            var insByte = instruction.First();
            var isValid = insByte != 0x00;

            if (rvMode)
            {
                // RV extension enabled
                Logger.Info("RV extension enabled and 00 detected. Continue...");
                isValid = true;
            }
            else
            {
                // RISC-V conform way
                if (!isValid)
                {
                    Logger.Error("Invalid instruction byte detected : {ins:X2}", insByte);
                }
            }

            return isValid;
        }

    }
}
