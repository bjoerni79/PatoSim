# RiscVSim
The goal of this project is simulating a RISC-V hart based on .NET Core 3.1 

## Architecture
The project uses .NET Core 3.1 and runs as a console application. Every operating system provided by .NET Core should be able running it. 
However, the code is written on Windows 10 with an Intel i5 CPU and this is the Little Endian / Big Endian decision for the .NET Core environment for for my test. 

### How To Build
Simply build it via the DotNet tool or installing the Visual Studio Community Edtion 2019.

### Testing
All opcode and instructions are tested via Unit Tests. 

## Implementation Status
lala

| Feature | Status | Comment |
| ------- | ------ | ------- |
| RV32I | In Progress | Version 2.1 |
| CSR | Planned | |
| M | Planned | |
| A | Plammed | |
