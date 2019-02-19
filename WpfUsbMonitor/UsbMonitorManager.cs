using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace UsbMonitor
{
    /// <summary>
    /// USB Monitor class to notify if the USB content changes
    /// </summary>
    public partial class UsbMonitorManager 
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Main window of the application.</param>
        /// <param name="start">Enable USB notification on startup or not.</param>
        public UsbMonitorManager(Window window, bool start = true)
        {
            this.windowHandle = new WindowInteropHelper(window).EnsureHandle();
            HwndSource.FromHwnd(this.windowHandle)?.AddHook(HwndHandler);
            if (start)
            {
                Start();
            }
        }
    }
}
