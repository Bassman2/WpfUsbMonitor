using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfUsbMonitor
{
    /// <summary>
    /// USB Monitor class to notify if the USB content changes
    /// </summary>
    public class UsbMonitor : DeviceChangeManager
    {
        private const int WM_DEVICECHANGE = 0x0219;

        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        private const int DBT_DEVTYP_OEM = 0x00000000;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        private const int DBT_DEVTYP_PORT = 0x00000003;
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        private const int DBT_DEVTYP_HANDLE = 0x00000006;

        private const int DBT_DEVNODES_CHANGED = 0x0007;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEQUERYREMOVE = 0x8001;
        private const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        private const int DBT_DEVICEREMOVEPENDING = 0x8003;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVICETYPESPECIFIC = 0x8005;
        private const int DBT_CUSTOMEVENT = 0x8006;

        private IntPtr windowHandle;
        //private IntPtr deviceEventHandle;

        /// <summary>
        /// Event for USB update
        /// </summary>
        public event EventHandler<UsbEventArgs> UsbChanged;

        public event EventHandler<UsbEventOemArgs> UsbOem;

        public event EventHandler<UsbEventVolumeArgs> UsbVolume;

        public event EventHandler<UsbEventPortArgs> UsbPort;

        public event EventHandler<UsbEventDeviceInterfaceArgs> UsbDeviceInterface;

        public event EventHandler<UsbEventHandleArgs> UsbHandle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Main window of the application.</param>
        /// <param name="start">Enable USB notification on startup or not.</param>
        public UsbMonitor(Window window, bool start = true)
        {
            windowHandle = new WindowInteropHelper(window).EnsureHandle();
            HwndSource.FromHwnd(windowHandle)?.AddHook(HwndHandler);
            if (start) Start();
        }

        /// <summary>
        /// Enable USB notification.
        /// </summary>
        public void Start()
        {
            Register(this.windowHandle);
            //int size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));
            //var deviceInterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();
            //deviceInterface.dbcc_size = size;
            //deviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            //IntPtr buffer = Marshal.AllocHGlobal(size);
            //Marshal.StructureToPtr(deviceInterface, buffer, true);
            //this.deviceEventHandle = NativeMethods.RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_WINDOW_HANDLE | DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            //if (deviceEventHandle == IntPtr.Zero)
            //{
            //    int error = Marshal.GetLastWin32Error();
            //}
            //Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// Disable USB notification.
        /// </summary>
        public void Stop()
        {
            Unregister();
            //if (deviceEventHandle != IntPtr.Zero)
            //{
            //    NativeMethods.UnregisterDeviceNotification(deviceEventHandle);
            //}
            //deviceEventHandle = IntPtr.Zero;
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                UsbDeviceChangeEvent deviceChangeEvent = (UsbDeviceChangeEvent)wparam.ToInt32();
                switch (deviceChangeEvent)
                {
                case UsbDeviceChangeEvent.Arrival:
                case UsbDeviceChangeEvent.QueryRemove:
                case UsbDeviceChangeEvent.QueryRemoveFailed:
                case UsbDeviceChangeEvent.RemovePending:
                case UsbDeviceChangeEvent.RemoveComplete:
                    int deviceType = Marshal.ReadInt32(lparam, 4);
                    switch (deviceType)
                    {
                    case DBT_DEVTYP_OEM:
                        var oemArgs = OnDeviceOem(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbOem?.Invoke(this, oemArgs);
                        break;
                    case DBT_DEVTYP_VOLUME:
                        var volumeArgs = OnDeviceVolume(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbVolume?.Invoke(this, volumeArgs);
                        break;
                    case DBT_DEVTYP_PORT:
                        var portArgs = OnDevicePort(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbPort?.Invoke(this, portArgs);
                        break;
                    case DBT_DEVTYP_DEVICEINTERFACE:
                        var interfaceArgs = OnDeviceInterface(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbDeviceInterface?.Invoke(this, interfaceArgs);
                        break;
                    case DBT_DEVTYP_HANDLE:
                        var handleArgs = OnDeviceHandle(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbHandle?.Invoke(this, handleArgs);
                        break;
                    default:
                        break;
                    }
                    break;
                case UsbDeviceChangeEvent.Changed:
                    // fire event
                    this.UsbChanged?.Invoke(this, new UsbEventArgs(deviceChangeEvent));
                    break;
                }

                //switch (wparam.ToInt32())
                //{
                //    case DBT_DEVICEARRIVAL:
                //        if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                //        {
                //            //this.UsbUpdate?.Invoke(this, CreateUsbEventArgs(UsbDeviceAction.Arrival, lparam));
                //        }
                //        break;
                //    case DBT_DEVICEQUERYREMOVE:
                //        if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                //        {
                //            //this.UsbUpdate?.Invoke(this, CreateUsbEventArgs(UsbDeviceAction.QueryRemove, lparam));
                //        }
                //        break;
                //    case DBT_DEVICEREMOVECOMPLETE:
                //        if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                //        {
                //            //this.UsbUpdate?.Invoke(this, CreateUsbEventArgs(UsbDeviceAction.RemoveComplete, lparam));
                //        }
                //        break;
                //    case DBT_DEVNODES_CHANGED:
                //        //this.UsbChanged?.Invoke(this, new EventArgs());
                //        break;
                //}
            }
            handled = false;
            return IntPtr.Zero;
        }

        //private UsbEventArgs CreateUsbEventArgs(UsbDeviceAction action, IntPtr lparam)
        //{

        //    int size = Marshal.ReadInt32(lparam, 0);
        //    //var deviceInterface = Marshal.PtrToStructure<NativeMethods.DEV_BROADCAST_DEVICEINTERFACE>(lparam);
        //    var deviceInterface = (NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lparam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));
        //    //var deviceType = (UsbDeviceType)deviceInterface.dbcc_devicetype;
        //    var classGuid = new Guid(deviceInterface.dbcc_classguid);
        //    var classId = GuidToDeviceClass(classGuid);
        //    var name = new string(deviceInterface.dbcc_name.TakeWhile(c => c != 0).ToArray());
        //    return new UsbEventArgs(action, classGuid, classId, name);
        //}

        private UsbDeviceClass GuidToDeviceClass(Guid guid)
        {
            UsbDeviceClass en = Enum.GetValues(typeof(UsbDeviceClass)).Cast<UsbDeviceClass>().Where(e =>
            {
                return e.GetType().GetField(e.ToString()).GetCustomAttribute<GuidAttribute>().Guid == guid;
            }).FirstOrDefault();
            return en;
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct DEV_BROADCAST_DEVICEINTERFACE
            {
                public Int32 dbcc_size;
                public Int32 dbcc_devicetype;
                public Int32 dbcc_reserved;
                [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
                public byte[] dbcc_classguid;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
                public char[] dbcc_name;
            }

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        }
    }
}
