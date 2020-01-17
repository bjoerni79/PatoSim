using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IRegister
    {
        int ProgramCounter  { get;  }

        Architecture Architecture { get; }

        void WriteSignedInt(int index, int value);

        int ReadSignedInt(int index);

        uint ReadUnsignedInt(RegisterName name);

        uint ReadUnsignedInt(int index);

        void WriteUnsignedInt(RegisterName name, uint value);

        void WriteUnsignedInt(int index, uint value);

        void NextInstruction(int offset);

        IEnumerable<byte> ReadBlock(int index);

        void WriteBlock(int index, IEnumerable<byte> block);
    }
}
