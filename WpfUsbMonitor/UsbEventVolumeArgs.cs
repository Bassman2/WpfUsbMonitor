using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUsbMonitor
{
    public class UsbEventVolumeArgs : UsbEventArgs
    {
        public UsbEventVolumeArgs(UsbDeviceAction action, uint unitmask, ushort flags, char drive) : base(action)
        {
            this.UnitMask = unitmask;
            this.Flags = flags;
            this.Drive = drive;
        }

        public uint UnitMask { get; private set; }

        public ushort Flags { get; private set; }

        public char Drive { get; private set; }

        public override string ToString()
        {
            return $"{Action} Volume: UnitMask={UnitMask} Flags={Flags} Drive={Drive}:";
        }
    }
}
