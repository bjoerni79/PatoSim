using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVSim.Environment
{
    public interface ISystemNotifier
    {
        void NotifySystemCall(uint f12);

        void IncreaseNopCounter();

        void NotifyFatalTrap(string description);

        void NotfyStarted();

        void NotifyStopped();
    }
}
