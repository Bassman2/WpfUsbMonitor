using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace UsbMonitor
{
    /// <summary>
    /// USB Monitor window class
    /// </summary>
    public class UsbMonitorWindow : Window, IUsbMonitorCommands, IUsbMonitorEvents, IUsbMonitorOverrides
    {

        private IntPtr windowHandle;

        private DeviceChangeManager deviceChangeManager = new DeviceChangeManager();

        #region IUsbMonitorEvents 

        public event EventHandler<UsbEventOemArgs> UsbOem;

        public event EventHandler<UsbEventVolumeArgs> UsbVolume;

        public event EventHandler<UsbEventPortArgs> UsbPort;

        public event EventHandler<UsbEventDeviceInterfaceArgs> UsbDeviceInterface;

        public event EventHandler<UsbEventHandleArgs> UsbHandle;

        /// <summary>
        /// Event for USB update
        /// </summary>
        public event EventHandler<UsbEventArgs> UsbChanged;

        public void CallUsbOem(object sender, UsbEventOemArgs args)
        {
            this.UsbOem?.Invoke(sender, args);
        }
        public void CallUsbVolumem(object sender, UsbEventVolumeArgs args)
        {
            this.UsbVolume?.Invoke(sender, args);
        }
        public void CallUsbPort(object sender, UsbEventPortArgs args)
        {
            this.UsbPort?.Invoke(sender, args);
        }
        public void CallUsbDeviceInterface(object sender, UsbEventDeviceInterfaceArgs args)
        {
            this.UsbDeviceInterface?.Invoke(sender, args);
        }
        public void CallUsbHandle(object sender, UsbEventHandleArgs args)
        {
            this.UsbHandle?.Invoke(sender, args);
        }
        public void CallUsbChanged(object sender, UsbEventArgs args)
        {
            this.UsbChanged?.Invoke(sender, args);
        }

        #endregion

        #region IUsbMonitorOverrides

        /// <summary>
        /// Override to handle USB interface notification.
        /// </summary>
        /// <param name="args">Update arguments</param>
        public virtual void OnUsbOem(UsbEventOemArgs args)
        { }

        public virtual void OnUsbVolume(UsbEventVolumeArgs args)
        { }

        public virtual void OnUsbPort(UsbEventPortArgs args)
        { }

        public virtual void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        { }

        public virtual void OnUsbHandle(UsbEventHandleArgs args)
        { }

        /// <summary>
        /// Override to handle USB changed notification.
        /// </summary>
        public virtual void OnUsbChanged(UsbEventArgs args)
        { }

        #endregion

        #region IUsbMonitorCommands
        
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

        #endregion

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
            DeviceChangeManager.HwndHandler(this, hwnd, msg, wparam, lparam);
            return IntPtr.Zero;
        }

        
    }
}
