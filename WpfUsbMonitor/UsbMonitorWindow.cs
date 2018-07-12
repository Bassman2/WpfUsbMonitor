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

        private DeviceChangeManager deviceChangeManager = new DeviceChangeManager();

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
        /// Event for USB changed
        /// </summary>
        //public event EventHandler UsbChanged;

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method is
        /// invoked whenever System.Windows.FrameworkElement.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.windowHandle = new WindowInteropHelper(this).EnsureHandle();
            HwndSource.FromHwnd(this.windowHandle)?.AddHook(HwndHandler);

            if (this.UsbNotification)
            {
                Start();
            }
        }

        /// <summary>
        /// USB Changed dependency property
        /// </summary>
        public static readonly DependencyProperty UsbChangedCommandProperty =
            DependencyProperty.Register("UsbChangedCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB update.
        /// </summary>
        public ICommand UsbChangedCommand
        {
            get { return (ICommand)GetValue(UsbChangedCommandProperty); }
            set { SetValue(UsbChangedCommandProperty, value); }
        }

        /// <summary>
        /// USB OEM dependency property
        /// </summary>
        public static readonly DependencyProperty UsbOemCommandProperty =
            DependencyProperty.Register("UsbOemCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB OEM.
        /// </summary>
        public ICommand UsbOemCommand
        {
            get { return (ICommand)GetValue(UsbOemCommandProperty); }
            set { SetValue(UsbOemCommandProperty, value); }
        }

        /// <summary>
        /// USB volume dependency property
        /// </summary>
        public static readonly DependencyProperty UsbVolumeCommandProperty =
            DependencyProperty.Register("UsbVolumeCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB volume.
        /// </summary>
        public ICommand UsbVolumeCommand
        {
            get { return (ICommand)GetValue(UsbVolumeCommandProperty); }
            set { SetValue(UsbVolumeCommandProperty, value); }
        }

        /// <summary>
        /// USB port dependency property
        /// </summary>
        public static readonly DependencyProperty UsbPortCommandProperty =
            DependencyProperty.Register("UsbPortCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB port.
        /// </summary>
        public ICommand UsbPortCommand
        {
            get { return (ICommand)GetValue(UsbPortCommandProperty); }
            set { SetValue(UsbPortCommandProperty, value); }
        }

        /// <summary>
        /// USB DeviceInterface dependency property
        /// </summary>
        public static readonly DependencyProperty UsbDeviceInterfaceCommandProperty =
            DependencyProperty.Register("UsbDeviceInterfaceCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB DeviceInterface.
        /// </summary>
        public ICommand UsbDeviceInterfaceCommand
        {
            get { return (ICommand)GetValue(UsbDeviceInterfaceCommandProperty); }
            set { SetValue(UsbDeviceInterfaceCommandProperty, value); }
        }

        /// <summary>
        /// USB Handle dependency property
        /// </summary>
        public static readonly DependencyProperty UsbHandleCommandProperty =
            DependencyProperty.Register("UsbHandleCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB Handle.
        /// </summary>
        public ICommand UsbHandleCommand
        {
            get { return (ICommand)GetValue(UsbHandleCommandProperty); }
            set { SetValue(UsbHandleCommandProperty, value); }
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
        protected virtual void OnUsbOem(UsbEventOemArgs args)
        { }

        protected virtual void OnUsbVolume(UsbEventVolumeArgs args)
        { }

        protected virtual void OnUsbPort(UsbEventPortArgs args)
        { }

        protected virtual void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        { }

        protected virtual void OnUsbHandle(UsbEventHandleArgs args)
        { }

        protected virtual void OnUsbUpdate(UsbEventArgs args)
        { }

        /// <summary>
        /// Override to handle USB changed notification.
        /// </summary>
        protected virtual void OnUsbChanged(UsbEventArgs args)
        { }

        /// <summary>
        /// Enable USB notification.
        /// </summary>
        public void Start()
        {
            this.deviceChangeManager.Register(this.windowHandle);
            //int size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));

            //var deviceInterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();
            //deviceInterface.dbcc_size = size;
            //deviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            //deviceInterface.dbcc_reserved = 0;
            //deviceInterface.dbcc_classguid = new Guid().ToByteArray();

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
            this.deviceChangeManager.Unregister();
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
                        var oemArgs = this.deviceChangeManager.OnDeviceOem(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbOem?.Invoke(this, oemArgs);
                        // execute command
                        if (this.UsbOemCommand?.CanExecute(oemArgs) ?? false)
                        {
                            this.UsbOemCommand?.Execute(oemArgs);
                        }
                        // call virtual method
                        OnUsbOem(oemArgs);
                        break;
                    case DBT_DEVTYP_VOLUME:
                        var volumeArgs = this.deviceChangeManager.OnDeviceVolume(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbVolume?.Invoke(this, volumeArgs);
                        // execute command
                        if (this.UsbVolumeCommand?.CanExecute(volumeArgs) ?? false)
                        {
                            this.UsbVolumeCommand?.Execute(volumeArgs);
                        }
                        // call virtual method
                        OnUsbVolume(volumeArgs);
                        break;
                    case DBT_DEVTYP_PORT:
                        var portArgs = this.deviceChangeManager.OnDevicePort(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbPort?.Invoke(this, portArgs);
                        // execute command
                        if (this.UsbPortCommand?.CanExecute(portArgs) ?? false)
                        {
                            this.UsbPortCommand?.Execute(portArgs);
                        }
                        // call virtual method
                        OnUsbPort(portArgs);
                        break;
                    case DBT_DEVTYP_DEVICEINTERFACE:
                        var interfaceArgs = this.deviceChangeManager.OnDeviceInterface(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbDeviceInterface?.Invoke(this, interfaceArgs);
                        // execute command
                        if (this.UsbDeviceInterfaceCommand?.CanExecute(interfaceArgs) ?? false)
                        {
                            this.UsbDeviceInterfaceCommand?.Execute(interfaceArgs);
                        }
                        // call virtual method
                        OnUsbInterface(interfaceArgs);
                        break;
                    case DBT_DEVTYP_HANDLE:
                        var handleArgs = this.deviceChangeManager.OnDeviceHandle(deviceChangeEvent, lparam);
                        // fire event
                        this.UsbHandle?.Invoke(this, handleArgs);
                        // execute command
                        if (this.UsbHandleCommand?.CanExecute(handleArgs) ?? false)
                        {
                            this.UsbHandleCommand?.Execute(handleArgs);
                        }
                        // call virtual method
                        OnUsbHandle(handleArgs);
                        break;
                    default:
                        break;
                    }
                    break;
                case UsbDeviceChangeEvent.Changed:
                    var changedArgs = new UsbEventArgs(deviceChangeEvent);
                    // fire event
                    this.UsbChanged?.Invoke(this, changedArgs);
                    // execute command
                    if (this.UsbChangedCommand?.CanExecute(changedArgs) ?? false)
                    {
                        this.UsbChangedCommand?.Execute(changedArgs);
                    }
                    // call virtual method
                    OnUsbChanged(changedArgs);
                    break;
                }
            

                /*
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
                    else
                    {

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
                    else
                    {

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
                    else
                    {

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
                */
            }
            handled = false;
            return IntPtr.Zero;
        }

        //private UsbEventArgs CreateUsbEventArgs(UsbDeviceAction action, IntPtr lparam)
        //{
        //    int size = Marshal.ReadInt32(lparam, 0);
        //    //var deviceInterface = Marshal.PtrToStructure<NativeMethods.DEV_BROADCAST_DEVICEINTERFACE>(lparam);
        //    var deviceInterface = (NativeMethods.DEV_BROADCAST_DEVICEINTERFACE) Marshal.PtrToStructure(lparam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));
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

            [StructLayout(LayoutKind.Sequential)]
            public struct DEV_BROADCAST_VOLUME
            {
                public uint dbch_Size;
                public uint dbch_Devicetype;
                public uint dbch_Reserved;
                public uint dbch_Unitmask;
                public ushort dbch_Flags;
            }

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        }
    }
}
