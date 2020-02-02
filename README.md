# patoSim
The goal of this project is simulating a RISC-V hart based on .NET Core 3.1.

## Architecture
The project uses .NET Core 3.1 and runs as a console application. Every operating system provided by .NET Core should be able running it.  However, the code is written and tested on Windows 10 with an Intel i5 CPU. Loging is supported by NLog and the ecosystem of the this logging tool like numeros viewers etc. can be used. Each hart runs in his own thread and, in future, complex scenarios could be set up like two harts exchanging content over memory etc.
I am new to the RISC-V world and any input of useful scenarios is appreciated.
The entire simulator is built with unit testing from the beginning (run dotnet test on src folder..) and the goal is to reach a stable OpCode implementation in a short time.

### How To Build
Simply clone the repository and build it via the DotNet tool or installing the Visual Studio Community Edtion 2019.

## Implementation Status
| Feature | Status | Comment |
| ------- | ------ | ------- |
| RV64I | Complete | Version 2.1 |
| RV32I | Complete | Version 2.1 |
| RV32E | Complete | Version 1.9 |
| CSR | Complete | Version 2.0 |
| RISC-V Assembly Language book support | *Under Test* | Supports the Opcode from the book written by Anthony J. Dos Reis |
| A | *Under Test* | The memory model needs a revise first |
| M | *Prepared* | Implementation done. More testing required |
| Compressed Instruction | *Next milestone* | |
| Debugging | Future milestone | According to RISC-V debug spec |
| Big Endian support | Future Milestone | The groundwork is done, but not enabled and requires testing |
| GCC Toolchain Support (using ELF) | Future milestone  ||
| LLVM Toolchain support (using ELF) | Future milestone ||

The simulator supports all opcodes as specified for RV32I and RV64I. It also supports the reduced RV32E mode as a subset of RV32I.
All opcodes are tested via Unit Tests on my Core I5 using Windows 10 and Dotnet core using little endian coding.



## How to Run
The simulator is a console base programm and supports the input forrmats of the book "RISC-V Assembly Language" and opcodes. Starting the console app without additional parameters shows the syntax of the arguments. Please find the OpCodeSamples in the RiscVSim project for sample files.
The long term goal of the project is using the GCC / LLVM toolchain for coding and using this simulator for executing different harts in different environments.

## Documentaion
Please find the documentation at the Wiki

## What's next?
The milestone 3 is in progress and the goal is to reach the next milestone. 

## Why?
Maybe this is the most important question. I developed with .NET for a long time and in December 2019 the Version 3.1 has been released. The thing was that I never had a really interesting topic to play with and, thanks to an article at osnews.com, I found this an interesting approach to try out. Here I am... Life is full of surprises.
