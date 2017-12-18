using System;

namespace WpfUsbMonitor
{
    /// <summary>
    /// A class for containing USB event data
    /// </summary>
    public class UsbEventArgs : EventArgs
    {
        internal UsbEventArgs(UsbDeviceAction action)
        {
            this.Action = action;
        }

        internal UsbEventArgs(UsbDeviceAction action, Guid classGuid, UsbDeviceClass classId, string name)
        {
            this.Action = action;
            this.ClassGuid = classGuid;
            this.Class = classId;
            this.Name = name;
        }

        /// <summary>
        /// Device action
        /// </summary>
        public UsbDeviceAction Action { get; private set; }

        /// <summary>
        /// Device class GUID
        /// </summary>
        public Guid ClassGuid { get; private set; }

        /// <summary>
        /// Device class enum
        /// </summary>
        public UsbDeviceClass Class { get; private set; }

        /// <summary>
        /// Device PnP name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>Debug string</returns>
        public override string ToString()
        {
            return Action == UsbDeviceAction.Changed ? "#####" : $"{Action} {Class} {Name} {ClassGuid}";
        }
    }
}
