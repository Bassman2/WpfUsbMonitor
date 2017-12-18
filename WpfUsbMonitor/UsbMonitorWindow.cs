using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WpfUsbMonitor
{
    /// <summary>
    /// USB Monitor window class
    /// </summary>
    public class UsbMonitorWindow : Window
    {
        private const int WM_DEVICECHANGE = 0x0219;

        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        private const int DBT_DEVNODES_CHANGED = 0x0007;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEQUERYREMOVE = 0x8001;
        private const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        private const int DBT_DEVICEREMOVEPENDING = 0x8003;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVICETYPESPECIFIC = 0x8005;
        private const int DBT_CUSTOMEVENT = 0x8006;

        private IntPtr windowHandle;
        private IntPtr deviceEventHandle;

        /// <summary>
        /// Event for USB update
        /// </summary>
        public event EventHandler<UsbEventArgs> UsbUpdate;

        /// <summary>
        /// Event for USB changed
        /// </summary>
        public event EventHandler UsbChanged;

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method is
        /// invoked whenever System.Windows.FrameworkElement.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            windowHandle = new WindowInteropHelper(this).EnsureHandle();
            HwndSource.FromHwnd(windowHandle)?.AddHook(HwndHandler);

            if (this.UsbNotification)
            {
                Start();
            }
        }

        /// <summary>
        /// USB Update dependency property
        /// </summary>
        public static readonly DependencyProperty UsbUpdateCommandProperty =
            DependencyProperty.Register("UsbUpdateCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB update.
        /// </summary>
        public ICommand UsbUpdateCommand
        {
            get { return (ICommand)GetValue(UsbUpdateCommandProperty); }
            set { SetValue(UsbUpdateCommandProperty, value); }
        }

        /// <summary>
        /// USB Changed dependency property
        /// </summary>
        public static readonly DependencyProperty UsbChangedCommandProperty =
            DependencyProperty.Register("UsbChangedCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB changed.
        /// </summary>
        public ICommand UsbChangedCommand
        {
            get { return (ICommand)GetValue(UsbChangedCommandProperty); }
            set { SetValue(UsbChangedCommandProperty, value); }
        }

        /// <summary>
        /// USB Notification dependency property
        /// </summary>
        public static readonly DependencyProperty UsbNotificationProperty =
            DependencyProperty.Register("UsbNotification", typeof(bool), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnUsbNotificationChanged)));

        private static void OnUsbNotificationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UsbMonitorWindow usbMonitorWindow = (UsbMonitorWindow)o;
            bool value = (bool)e.NewValue;
            if (value)
            {
                usbMonitorWindow.Start();
            }
            else
            {
                usbMonitorWindow.Stop();
            }
        }

        /// <summary>
        /// Enable USB notification.
        /// </summary>
        public bool UsbNotification
        {
            get { return (bool)GetValue(UsbNotificationProperty); }
            set { SetValue(UsbNotificationProperty, value); }
        }

        /// <summary>
        /// Override to handle USB interface notification.
        /// </summary>
        /// <param name="args">Update arguments</param>
        protected virtual void OnUsbUpdate(UsbEventArgs args)
        { }

        /// <summary>
        /// Override to handle USB changed notification.
        /// </summary>
        protected virtual void OnUsbChanged()
        { }

        /// <summary>
        /// Enable USB notification.
        /// </summary>
        public void Start()
        {
            int size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));
            var deviceInterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();
            deviceInterface.dbcc_size = size;
            deviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);
            this.deviceEventHandle = NativeMethods.RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_WINDOW_HANDLE | DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            if (deviceEventHandle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
            }
            Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// Disable USB notification.
        /// </summary>
        public void Stop()
        {
            if (deviceEventHandle != IntPtr.Zero)
            {
                NativeMethods.UnregisterDeviceNotification(deviceEventHandle);
            }
            deviceEventHandle = IntPtr.Zero;
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                switch (wparam.ToInt32())
                {
                case DBT_DEVICEARRIVAL:
                    if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        UsbEventArgs args = CreateUsbEventArgs(UsbDeviceAction.Arrival, lparam);
                        // fire event
                        this.UsbUpdate?.Invoke(this, args);
                        // execute command
                        if (this.UsbUpdateCommand?.CanExecute(args) ?? false)
                        {
                            this.UsbUpdateCommand?.Execute(args);
                        }
                        // call virtual method
                        OnUsbUpdate(args);
                    }
                    break;
                case DBT_DEVICEQUERYREMOVE:
                    if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        UsbEventArgs args = CreateUsbEventArgs(UsbDeviceAction.QueryRemove, lparam);
                        // fire event
                        this.UsbUpdate?.Invoke(this, args);
                        // execute command
                        if (this.UsbUpdateCommand?.CanExecute(args) ?? false)
                        {
                            this.UsbUpdateCommand?.Execute(args);
                        }
                        // call virtual method
                        OnUsbUpdate(args);
                    }
                    break;
                case DBT_DEVICEREMOVECOMPLETE:
                    if (Marshal.ReadInt32(lparam, 4) == DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        UsbEventArgs args = CreateUsbEventArgs(UsbDeviceAction.RemoveComplete, lparam);
                        // fire event
                        this.UsbUpdate?.Invoke(this, args);
                        // execute command
                        if (this.UsbUpdateCommand?.CanExecute(args) ?? false)
                        {
                            this.UsbUpdateCommand?.Execute(args);
                        }
                        // call virtual method
                        OnUsbUpdate(args);
                    }
                    break;
                case DBT_DEVNODES_CHANGED:
                    {
                        // fire event
                        this.UsbChanged?.Invoke(this, new EventArgs());
                        // execute command
                        if (this.UsbChangedCommand?.CanExecute(null) ?? false)
                        {
                            this.UsbChangedCommand?.Execute(null);
                        }
                        // call virtual method
                        OnUsbChanged();
                        break;
                    }
                }
            }
            handled = false;
            return IntPtr.Zero;
        }

        private UsbEventArgs CreateUsbEventArgs(UsbDeviceAction action, IntPtr lparam)
        {
            int size = Marshal.ReadInt32(lparam, 0);
            //var deviceInterface = Marshal.PtrToStructure<NativeMethods.DEV_BROADCAST_DEVICEINTERFACE>(lparam);
            var deviceInterface = (NativeMethods.DEV_BROADCAST_DEVICEINTERFACE) Marshal.PtrToStructure(lparam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));
            //var deviceType = (UsbDeviceType)deviceInterface.dbcc_devicetype;
            var classGuid = new Guid(deviceInterface.dbcc_classguid);
            var classId = GuidToDeviceClass(classGuid);
            var name = new string(deviceInterface.dbcc_name.TakeWhile(c => c != 0).ToArray());
            return new UsbEventArgs(action, classGuid, classId, name);
        }
        
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
