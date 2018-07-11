using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfUsbMonitor
{
    public class DeviceChangeManager
    {
        //private const int WM_DEVICECHANGE = 0x0219;

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

        private IntPtr deviceEventHandle;

        public DeviceChangeManager()
        { }

        public void Register(IntPtr windowHandle)
        {
            int size = Marshal.SizeOf(typeof(DevBroadcastDeviceInterface));

            var deviceInterface = new DevBroadcastDeviceInterface();
            deviceInterface.Size = (uint)size;
            deviceInterface.DeviceType = DBT_DEVTYP_DEVICEINTERFACE;
            deviceInterface.Reserved = 0;
            //deviceInterface.ClassGuid = new Guid().ToByteArray();

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);

            this.deviceEventHandle = NativeMethods.RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_WINDOW_HANDLE | DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            if (this.deviceEventHandle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
            }
            Marshal.FreeHGlobal(buffer);
        }

        public void Unregister()
        {
            if (this.deviceEventHandle != IntPtr.Zero)
            {
                NativeMethods.UnregisterDeviceNotification(deviceEventHandle);
            }
            this.deviceEventHandle = IntPtr.Zero;
        }

        public UsbEventArgs OnDeviceChange(IntPtr wparam, IntPtr lparam)
        {
            int wParam = wparam.ToInt32();
            switch (wParam)
            {
            case DBT_DEVICEARRIVAL:
                return OnDevice(UsbDeviceAction.Arrival, lparam);
            case DBT_DEVICEQUERYREMOVE:
                return OnDevice(UsbDeviceAction.QueryRemove, lparam);
            case DBT_DEVICEREMOVECOMPLETE:
                return OnDevice(UsbDeviceAction.RemoveComplete, lparam);
            default:
                break;
            }
            return null;
        }


        private UsbEventArgs OnDevice(UsbDeviceAction action, IntPtr lparam)
        {
            //int size = Marshal.ReadInt32(lparam, 0);
            int deviceType = Marshal.ReadInt32(lparam, 4);

            switch (deviceType)
            {
            case DBT_DEVTYP_OEM:
                var oem = (DevBroadcastOEM)Marshal.PtrToStructure(lparam, typeof(DevBroadcastOEM));
                Debug.WriteLine($"OEM: Size={oem.Size}, DeviceType={oem.DeviceType}, Reserved={oem.Reserved}, Identifier={oem.Identifier}, SuppFunc={oem.SuppFunc}");

                
                break;
            case DBT_DEVTYP_VOLUME:
                var volume = (DevBroadcastVolume)Marshal.PtrToStructure(lparam, typeof(DevBroadcastVolume));
                char drive = FirstDriveFromMask(volume.UnitMask);
                Debug.WriteLine($"Volume: size={volume.Size}, deviceType={volume.DeviceType}, reserved={volume.Reserved}, unitmask={volume.UnitMask}, flags={volume.Flags}, drive={drive}");
                return new UsbEventVolumeArgs(action, volume.UnitMask, volume.Flags, drive);
            case DBT_DEVTYP_PORT:
                var port = (DevBroadcastPort)Marshal.PtrToStructure(lparam, typeof(DevBroadcastPort));
                Debug.WriteLine($"Port: Size={port.Size}, DeviceType={port.DeviceType}, Reserved={port.Reserved}, Name={port.Name}");
                break;
            case DBT_DEVTYP_DEVICEINTERFACE:
                var device = (DevBroadcastDeviceInterface)Marshal.PtrToStructure(lparam, typeof(DevBroadcastDeviceInterface));
                Debug.WriteLine($"DeviceInterface: Size={device.Size}, DeviceType={device.DeviceType}, Reserved={device.Reserved}, ClassGuid={device.ClassGuid}, Name={device.Name}");
                break;
            case DBT_DEVTYP_HANDLE:
                var handle = (DevBroadcastHandle)Marshal.PtrToStructure(lparam, typeof(DevBroadcastHandle));
                Debug.WriteLine($"DeviceInterface: Size={handle.Size}, DeviceType={handle.DeviceType}, Reserved={handle.Reserved}, Handle={handle.Handle}, DevNotify={handle.DevNotify}, EventGuid={handle.EventGuid}, NameOffset={handle.NameOffset}, Data={handle.Data}");
                break;
            default:
                break;
            }
            return null;
        }

        private char FirstDriveFromMask(uint unitmask)
        {
            for (byte i = 0; i < 26; ++i, unitmask >>= 1)
            {
                if ((unitmask & 0x1) != 0)
                {
                    return (char)(((byte)'A') + i);
                }
            }
            return (char)0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DevBroadcastOEM
        {
            public uint Size;
            public uint DeviceType;
            public uint Reserved;
            public uint Identifier;
            public uint SuppFunc;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct DevBroadcastPort
        {
            public uint Size;
            public uint DeviceType;
            public uint Reserved; 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public String Name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DevBroadcastDeviceInterface
        {
            public uint Size;
            public uint DeviceType;
            public uint Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public String Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DevBroadcastVolume
        {
            public uint Size;
            public uint DeviceType;
            public uint Reserved;
            public uint UnitMask;
            public ushort Flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct DevBroadcastHandle
        {
            public uint Size;
            public uint DeviceType;
            public uint Reserved;
            public IntPtr Handle;
            public IntPtr DevNotify;
            public Guid EventGuid;
            public ulong NameOffset;
            public byte[] Data;
        }
        
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        }
    }
}
