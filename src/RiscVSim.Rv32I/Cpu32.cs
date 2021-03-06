﻿using RiscVSim.Environment;
using RiscVSim.Environment.Decoder;
using RiscVSim.Environment.Exception;
using RiscVSim.OpCodes.Rv32I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiscVSim.Rv32I
{
    public class Cpu32 : ICpu32
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IMemory memory;
        private IRegister register;
        private OpCodeRegistry opCodeRegistry;
        private ISystemNotifier environment;
        private ICsrRegister csrRegister;

        private RvcComposer32 composer;

        private Stack<uint> rasStack;

        public Cpu32()
        {
            opCodeRegistry = new OpCodeRegistry();
            composer = new RvcComposer32();
        }

        public void AssignEEI(ISystemNotifier environment)
        {
            this.environment = environment;
        }

        public void AssignMemory(IMemory memory)
        {
            this.memory = memory;
        }

        public void AssignRegister(IRegister register)
        {
            this.register = register;
        }

        public void AssignRasStack(Stack<uint> rasStack)
        {
            this.rasStack = rasStack;
        }

        public void ExecuteRvc(RvcPayload payload)
        {

            var instruction = composer.ComposeInstruction(payload);
            var instructionPayload = composer.Compose(instruction, payload);
            Execute(instruction, instructionPayload);
        }

        public void Execute(Instruction instruction, InstructionPayload payload)
        {
            if (!opCodeRegistry.IsInitialized)
            {
                Logger.Error("CPU is not initialized");
                new RiscVSimException("CPU is not initialized: Please call init() first!");
            }

            var curOpCode = instruction.OpCode;

            // Execute the command now
            var opCodeCommand = opCodeRegistry.Get(curOpCode);

            if (opCodeCommand == null)
            {
                string opCodeNotSupportedErrorMessage = String.Format("Implementation for OpCode {0} cannot be found", curOpCode);
                Logger.Error(opCodeNotSupportedErrorMessage);
                throw new OpCodeNotSupportedException(opCodeNotSupportedErrorMessage);
            }

            var incPc = opCodeCommand.Execute(instruction, payload);
            if (incPc)
            {
                register.NextInstruction(instruction.InstructionLength);
            }
            
           
        }

        public void Init()
        {
            // Add opcode=04
            opCodeRegistry.Add(0x04, new OpCode32Id04(memory, register, environment));

            // Add opcode=0C, 0D and 05
            opCodeRegistry.Add(0x0C, new OpCode32Id0C(memory, register));
            opCodeRegistry.Add(0x0D, new OpCode32Id0D(memory, register));
            opCodeRegistry.Add(0x05, new OpCode32Id05(memory, register));

            // Jump Opcodes:
            //
            // opcode 1B (JAL), opcode 19 (JALR), opcode = 18 (BNE...)
            opCodeRegistry.Add(0x1B, new OpCode32Id1B(memory, register, rasStack));
            opCodeRegistry.Add(0x19, new OpCode32Id19(memory, register, rasStack));
            opCodeRegistry.Add(0x18, new OpCode32Id18(memory, register, rasStack));

            //
            // Load and Store
            //
            opCodeRegistry.Add(0x00, new OpCode32Id00(memory, register));
            opCodeRegistry.Add(0x08, new OpCode32Id08(memory, register));

            //
            // FENCE, A Ext.
            //
            opCodeRegistry.Add(0x03, new OpCode32Id03(memory, register));
            opCodeRegistry.Add(0x0B, new OpCode32Id0B(memory, register));

            //
            // System
            //
            opCodeRegistry.Add(0x1C, new OpCode32Id1C(memory, register,csrRegister, environment));
        }

        public void AssignCrs(ICsrRegister csrRegister)
        {
            this.csrRegister = csrRegister;
        }
    }
}
