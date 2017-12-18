using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;
using WpfUsbMonitor;

namespace DeviceCatcher
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string text;

        public DelegateCommand<UsbEventArgs> UsbUpdateCommand { get; private set; }
        public DelegateCommand UsbChangedCommand { get; private set; }

        public MainViewModel()
        {
            this.UsbUpdateCommand = new DelegateCommand<UsbEventArgs>(OnUsbUpdate, OnCanUsbUpdate);
            this.UsbChangedCommand = new DelegateCommand(OnUsbChanged, OnCanUsbChanged);
        }

        public void OnUsbUpdate(UsbEventArgs args)
        {
            this.Text += args.ToString() + "\r\n";
        }

        public bool OnCanUsbUpdate(UsbEventArgs args)
        {
            return true;
        }

        public void OnUsbChanged()
        {
            this.Text += "Changed\r\n";
        }

        public bool OnCanUsbChanged()
        {
            return true;
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                NotifyPropertyChanged(nameof(Text));
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private delegate void NotifyPropertyChangedDeleagte(string propertyName);

        public virtual void NotifyPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (GetType().GetProperty(propertyName) == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName), $"No property with name {propertyName} exists.");
            }

            if (Dispatcher.CurrentDispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.DataBind, new NotifyPropertyChangedDeleagte(NotifyPropertyChanged), propertyName);
            }
        }

        #endregion
    }
}
