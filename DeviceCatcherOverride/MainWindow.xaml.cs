using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfUsbMonitor;

namespace DeviceCatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UsbMonitorWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnUsbOem(UsbEventOemArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbVolume(UsbEventVolumeArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbPort(UsbEventPortArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbHandle(UsbEventHandleArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbUpdate(UsbEventArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }
    }
}
