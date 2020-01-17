using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface IRegister
    {
        int ProgramCounter  { get;  }



        void WriteSignedInt(int index, int value);

        int ReadSignedInt(int index);



        uint ReadUnsignedInt(RegisterName name);

        uint ReadUnsignedInt(int index);




        void WriteUnsignedInt(RegisterName name, uint value);

        void WriteUnsignedInt(int index, uint value);


        long ReadSignedLong(RegisterName name);

        long ReadSignedLong(int index);


        void WriteSignedLong(RegisterName registerName, long value);

        void WriteSignedLong(int index, long value);


        ulong ReadUnsignedLong(RegisterName name);

        ulong ReadUnsignedLong(int name);

        void WriteUnsignedLong(RegisterName name, ulong value);

        void WriteUnsignedLong(int index, ulong value);




        void NextInstruction(int offset);

        IEnumerable<byte> ReadBlock(int index);

        void WriteBlock(int index, IEnumerable<byte> block);
    }
}
