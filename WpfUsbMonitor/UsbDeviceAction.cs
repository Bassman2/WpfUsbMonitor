namespace WpfUsbMonitor
{
    /// <summary>
    /// Device action
    /// </summary>
    public enum UsbDeviceAction
    {
        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        Arrival = 1,

        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        QueryRemove = 2,

        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        RemoveComplete = 3,

        /// <summary>
        /// A device has been added to or removed from the system.
        /// </summary>
        Changed = 4
    }
}
