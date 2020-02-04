using RiscVSim.Environment.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment.Decoder
{
    public class RvcPayload
    {
        public RvcPayload()
        {

        }

        public void LoadCI(int op, int immm, int rd, int f3)
        {
            Op = op;
            Rd = rd;
            Funct3 = f3;
            Immediate = immm;
            Type = InstructionType.RVC_CI;
        }

        public void LoadCSS (int op, int rs2, int immediate, int f3)
        {
            Op = op;
            Rs2 = rs2;
            Immediate = immediate;
            Funct3 = f3;
            Type = InstructionType.RVC_CSS;
        }

        public void LoadCL(int op, int rdc, int imm, int rs1c, int f3)
        {
            Op = op;
            Rd = ConvertCompressRegister(rdc);
            Immediate = imm;
            Rs1 = ConvertCompressRegister(rs1c);
            Funct3 = f3;
            Type = InstructionType.RVC_CL;
        }

        public void LoadCS(int op, int rs2c, int imm, int rs1c, int f3)
        {
            Op = op;
            Rs2 = ConvertCompressRegister(rs2c);
            Immediate = imm;
            Rs1 = ConvertCompressRegister(rs1c);
            Funct3 = f3;
            Type = InstructionType.RVC_CS;
        }

        public void LoadCJ(int op, int imm, int f3)
        {
            Op = op;
            Immediate = imm;
            Funct3 = f3;
            Type = InstructionType.RVC_CJ;
        }

        public void LoadCR(int op, int rs1, int rs2, int funct4)
        {
            Op = op;
            Rs1 = rs1;
            Rs2 = rs2;
            Funct4 = funct4;
            Type = InstructionType.RVC_CR;
        }

        public void LoadCB(int op, int imm, int rs1c, int f3)
        {
            Op = op;
            Immediate = imm;
            Rs1 = ConvertCompressRegister(rs1c);
            Funct3 = f3;
            Type = InstructionType.RVC_CB;
        }

        public void LoadCIW(int op, int rdc, int imm, int f3)
        {
            Op = op;
            Rd = ConvertCompressRegister(rdc);
            Immediate = imm;
            Funct3 = f3;
            Type = InstructionType.RVC_CIW;
        }

        public InstructionType Type { get; private set; }

        public int Op { get; private set; }

        public int Rs1 { get; private set; }

        public int Rs2 { get; private set; }

        public int Rd { get; private set; }

        public int Funct3 { get; private set; }

        public int Funct4 { get; private set; }

        public int Immediate { get; private set; }


        private int ConvertCompressRegister(int compressed)
        {
            return compressed + 8;
        }
    }
}
