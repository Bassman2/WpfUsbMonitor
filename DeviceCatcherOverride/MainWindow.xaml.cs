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

        protected override void OnUsbUpdate(UsbEventArgs args)
        {
            this.textBox.Text += args.ToString() + "\r\n";
        }

        protected override void OnUsbChanged()
        {
            this.textBox.Text += "Changed\r\n";
        }
    }
}
