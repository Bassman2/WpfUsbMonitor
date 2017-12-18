using System;
using System.Windows;
using WpfUsbMonitor;

namespace DeviceCatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UsbMonitor usbMonitor;

        public MainWindow()
        {
            InitializeComponent();

            this.usbMonitor = new UsbMonitor(this);
            this.usbMonitor.UsbUpdate += OnUsbUpdate;
            this.usbMonitor.UsbChanged += OnUsbChanged;

        }

        private void OnUsbChanged(object sender, EventArgs e)
        {
            this.textBox.Text += "Changed\r\n";
        }

        private void OnUsbUpdate(object sender, UsbEventArgs e)
        {
            this.textBox.Text += e.ToString() + "\r\n";
        }
    }
}
