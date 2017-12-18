using System;
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

        private void UsbMonitorWindow_UsbUpdate(object sender, UsbEventArgs e)
        {
            this.textBox.Text += e.ToString() + "\r\n";
        }

        private void UsbMonitorWindow_UsbChanged(object sender, EventArgs e)
        {
            this.textBox.Text += "Changed\r\n";
        }
    }
}
