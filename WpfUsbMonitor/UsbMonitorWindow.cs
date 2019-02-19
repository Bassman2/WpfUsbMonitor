using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace UsbMonitor
{
    /// <summary>
    /// USB Monitor window class
    /// </summary>
    public class UsbMonitorWindow : Window
    {
        private const int WM_DEVICECHANGE = 0x0219;

        private IntPtr windowHandle;
        
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
        /// ICommand to be called on USB Changed.
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
        /// USB Volume dependency property
        /// </summary>
        public static readonly DependencyProperty UsbVolumeCommandProperty =
            DependencyProperty.Register("UsbVolumeCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB Volume.
        /// </summary>
        public ICommand UsbVolumeCommand
        {
            get { return (ICommand)GetValue(UsbVolumeCommandProperty); }
            set { SetValue(UsbVolumeCommandProperty, value); }
        }

        /// <summary>
        /// USB Port dependency property
        /// </summary>
        public static readonly DependencyProperty UsbPortCommandProperty =
            DependencyProperty.Register("UsbPortCommand", typeof(ICommand), typeof(UsbMonitorWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ICommand to be called on USB Port.
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
        }

        /// <summary>
        /// Disable USB notification.
        /// </summary>
        public void Stop()
        {
            this.deviceChangeManager.Unregister();
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
                    UsbDeviceType deviceType = (UsbDeviceType)Marshal.ReadInt32(lparam, 4);
                    switch (deviceType)
                    {
                    case UsbDeviceType.OEM:
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
                    case UsbDeviceType.Volume:
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
                    case UsbDeviceType.Port:
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
                    case UsbDeviceType.DeviceInterface:
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
                    case UsbDeviceType.Handle:
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
            }
            handled = false;
            return IntPtr.Zero;
        }

        
    }
}
