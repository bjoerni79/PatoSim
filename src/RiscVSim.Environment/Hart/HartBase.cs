using RiscVSim.Environment.Custom;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
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
        protected IHartEnvironment environment;

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
        // RVC Decoder
        protected RvcDecoder rvcDecoder;

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

        public  string GetMemoryState()
        {
            return environment.GetMemoryState();
        }

        public string GetRegisterStates()
        {
            return environment.GetRegisterStates();
        }


        public abstract void Load(ulong address, IEnumerable<byte> data);

        protected abstract void BootCpu();

        protected abstract void ExecuteOpcode(Instruction instruction, InstructionPayload payload);

        protected abstract void ExecuteRvcOpcode(RvcPayload payload);

        protected abstract void InitDetails(ulong programCounter);

        public void Init(ulong programCounter)
        {
            // Set the instruction decoder and type decoder
            instructionDecoder = new InstructionDecoder(EndianType.Little);
            typeDecoder = new TypeDecoder();
            rvcDecoder = new RvcDecoder(architecture);

            InitDetails(programCounter);
            environment.ApplyOutputParameter(configuration.Debug, configuration.VerboseMode);

            isInitialized = true;
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
                environment.NotfyStarted();
                BootCpu();

                // All set up and start the loop
                Fetch();
            }
            catch (System.Exception ex)
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
                    var isCustom = (instructionCoding.First() & 0x7F) == 0x00;
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

                        var rvOpcode = new RvOpcode(memory, register, environment);
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

                // Stop the instruction fetch if an error occured or the state is stopped
                var currentState = environment.GetCurrentState();
                if (currentState == State.FatalError || currentState == State.Stopped)
                {
                    break;
                }

                // Done. Next run.
                pc = register.ReadUnsignedInt(register.ProgramCounter);
                instructionCoding = memory.GetHalfWord(pc);
                Logger.Info("Instruction {ins:X} fetched", BitConverter.ToString(instructionCoding.ToArray()));
            }

            if (environment.GetCurrentState() == State.FatalError)
            {
                Console.Error.WriteLine("# FATAL ERROR occured : " + environment.GetStateDescription());
            }

            if (environment.GetCurrentState() == State.Stopped)
            {
                Console.Out.WriteLine("# Hart Stopped");
            }

            environment.NotifyStopped();
        }

        /// <summary>
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="instructionCoding"></param>
        /// <param name="instruction"></param>
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
                    OpCode = instruction.OpCode
                };
            }

            if (instruction.InstructionLength == 2)
            {
                // RVC, Compressed Opcode detected
                Logger.Info("RVC Compressed Opcode detected");

                var payload = rvcDecoder.Decode(instructionCoding);
                if (payload == null || payload.Type == InstructionType.RVC_Unknown)
                {
                    Logger.Error("Rvc: No Payload detected");
                    throw new RiscVSimException("Rvc : No Payload detected!");
                }

                ExecuteRvcOpcode(payload);
            }

            if (instruction.InstructionLength == 4)
            {
                // Read the complete 32 Bit instruction set for the decoding

                var inst32Coding = memory.GetWord(pc);
                var payload = typeDecoder.DecodeType(instruction, inst32Coding);
                Logger.Info("Instruction to excecute = {ins32}", BitConverter.ToString(inst32Coding.ToArray()));

                if (payload == null)
                {
                    Logger.Error("No Payload detected");
                    throw new RiscVSimException("No Payload detected!");
                }

                // Execute the command
                environment.NotifyBeforeExec(payload);
                ExecuteOpcode(instruction, payload);
            }
        }



        /// <summary>
        /// Left over from the Bootstrap Core... is there a better way of doing this?
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool ContinueIfValid(IEnumerable<byte> instruction)
        {
            if (rvMode)
            {
                return true;
            }

            // 00 00 is not a valid codign!
            bool isValid = !((instruction.First() == 0x00) && (instruction.ElementAt(1) == 0x00));

            //TODO:  Raise a fatal error in the environment!


            // RISC-V conform way
            if (!isValid)
            {
                environment.NotifyFatalTrap("Invalid coding detected (RVCMode missing?)  : " + BitConverter.ToString(instruction.ToArray()) );
            }

            return isValid;
        }

    }
}
