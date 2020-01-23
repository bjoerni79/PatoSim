# RiscVSim
The goal of this project is simulating a RISC-V hart based on .NET Core 3.1 

## Architecture
The project uses .NET Core 3.1 and runs as a console application. Every operating system provided by .NET Core should be able running it. 
However, the code is written and tested on Windows 10 with an Intel i5 CPU. 

### How To Build
Simply build it via the DotNet tool or installing the Visual Studio Community Edtion 2019.

## Implementation Status
| Feature | Status | Comment |
| ------- | ------ | ------- |
| RV64I | Complete | Version 2.1 |
| RV32I | Complete | Version 2.1 |
| RV32E | Under Test | Version 1.9 |
| CSR | In Progress | |
| Compressed Instruction | In Progress | |
| A | Planned | |
| GCC Toolchain Support | Planned ||

The simulator supports all opcodes as specified for RV32I and RV64I. It also supports the reduced RV32E mode, but it is stil under test. All OpCode are tested via Unit tests and there are currently 160+ Tests. Nevertheless, there are still bugs in it and I would not recommend using it for your daily work. All my tests were done with my Windows 10 + Intel I5 CPU machine with Little Endian coding. Big Endian coding is on the list (basically just a reverse on the byte order, but still needs testing efforts) and will come later.

## How to Run
The simulator is a console base programm and only supports Opode at the moment. Starting the console app without additional parameters shows the syntax of the arguments. Please find the OpCodeSamples in the RiscVSim project for sample files. 

## What's next?
The roadmap of the tool is to complete the simulator with the extensions and add support for other input formats. Right now this is just a (hopefully) stable prototype and someone else can get the idea how it shall look like.

## Why?
Maybe this is the most important question. I developed with .NET for a long time and in December 2019 the Version 3.1 has been released. The problem is that I never had a really interesting topic to play with and, thanks to an article at osnews.com, I found this an interesting approach to try out. Here I am... Life is full of surprises.
