﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUsbMonitor
{
    public enum UsbDeviceInterface
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("Unknown")]
        [GuidAttribute()]
        Unknown = 0,

        /// <summary>
        /// Brightness
        /// </summary>
        [Description("Brightness")]
        [GuidAttribute("FDE5BBA4-B3F9-46FB-BDAA-0728CE3100B4")]
        Brightness,

        /// <summary>
        /// CD Changer
        /// </summary>
        [Description("CD Changer")]
        [GuidAttribute("53F56312-B6BF-11D0-94F2-00A0C91EFB8B")]
        CDChanger,

        /// <summary>
        /// CD ROM
        /// </summary>
        [Description("CD ROM")]
        [GuidAttribute("53F56308-B6BF-11D0-94F2-00A0C91EFB8B")]
        CDROM,

        /// <summary>
        /// COM Port
        /// </summary>
        [Description("COM Port")]
        [GuidAttribute("86E0D1E0-8089-11D0-9CE4-08003E301F73")]
        COMPort,

        /// <summary>
        /// Disk
        /// </summary>
        [Description("Disk")]
        [GuidAttribute("53F56307-B6BF-11D0-94F2-00A0C91EFB8B")]
        Disk,

        /// <summary>
        /// Display Adapter
        /// </summary>
        [Description("Display Adapter")]
        [GuidAttribute("5B45201D-F2F2-4F3B-85BB-30FF1F953599")]
        DisplayAdapter,

        /// <summary>
        /// Floppy
        /// </summary>
        [Description("Floppy")]
        [GuidAttribute("53F56311-B6BF-11D0-94F2-00A0C91EFB8B")]
        Floppy,

        /// <summary>
        /// HID
        /// </summary>
        [Description("HID")]
        [GuidAttribute("4D1E55B2-F16F-11CF-88CB-001111000030")]
        HID,

        /// <summary>
        /// I2C
        /// </summary>
        [Description("I2C")]
        [GuidAttribute("2564AA4F-DDDB-4495-B497-6AD4A84163D7")]
        I2C,

        /// <summary>
        /// Image
        /// </summary>
        [Description("Image")]
        [GuidAttribute("6BDD1FC6-810F-11D0-BEC7-08002BE2092F")]
        Image,

        /// <summary>
        /// Keyboard
        /// </summary>
        [Description("Keyboard")]
        [GuidAttribute("884b96c3-56ef-11d1-bc8c-00a0c91405dd")]
        Keyboard,

        /// <summary>
        /// Medium Changer
        /// </summary>
        [Description("Medium Changer")]
        [GuidAttribute("53F56310-B6BF-11D0-94F2-00A0C91EFB8B")]
        MediumChanger,

        /// <summary>
        /// Modem
        /// </summary>
        [Description("Modem")]
        [GuidAttribute("2C7089AA-2E0E-11D1-B114-00C04FC2AAE4")]
        Modem,

        /// <summary>
        /// Monitor
        /// </summary>
        [Description("Monitor")]
        [GuidAttribute("E6F07B5F-EE97-4a90-B076-33F57BF4EAA7")]
        Monitor,

        /// <summary>
        /// Mouse
        /// </summary>
        [Description("Mouse")]
        [GuidAttribute("378DE44C-56EF-11D1-BC8C-00A0C91405DD")]
        Mouse,

        /// <summary>
        /// Network
        /// </summary>
        [Description("Network")]
        [GuidAttribute("CAC88484-7515-4C03-82E6-71A87ABAC361")]
        Net,

        /// <summary>
        /// Output Protection Management 
        /// </summary>
        [Description("Output Protection Management")]
        [GuidAttribute("BF4672DE-6B4E-4BE4-A325-68A91EA49C09")]
        OPM,

        /// <summary>
        /// Parallel Port IEEE 1284
        /// </summary>
        [Description("Parallel Port IEEE 1284")]
        [GuidAttribute("97F76EF0-F883-11D0-AF1F-0000F800845C")]
        Parallel,

        /// <summary>
        /// Parallel Class
        /// </summary>
        [Description("Parallel Class")]
        [GuidAttribute("811FC6A5-F728-11D0-A537-0000F8753ED1")]
        ParClass,

        /// <summary>
        /// Partition Device
        /// </summary>
        [Description("Partition Device")]
        [GuidAttribute("53F5630A-B6BF-11D0-94F2-00A0C91EFB8B")]
        Partition,

        /// <summary>
        /// Serial Bus Enumerator
        /// </summary>
        [Description("Serial Bus Enumerator")]
        [GuidAttribute("4D36E978-E325-11CE-BFC1-08002BE10318")]
        SerenumBusEnumerator,

        /// <summary>
        /// Side Show
        /// </summary>
        [Description("Side Show")]
        [GuidAttribute("152E5811-FEB9-4B00-90F4-D32947AE1681")]
        SideShow,

        /// <summary>
        /// Storage Port
        /// </summary>
        [Description("Storage Port")]
        [GuidAttribute("2ACCFE60-C130-11D2-B082-00A0C91EFB8B")]
        StoragePort,

        /// <summary>
        /// Tape
        /// </summary>
        [Description("Tape")]
        [GuidAttribute("53F5630B-B6BF-11D0-94F2-00A0C91EFB8B")]
        Tape,
        
        /// <summary>
        /// USB Device
        /// </summary>
        [Description("USB Device")]
        [GuidAttribute("A5DCBF10-6530-11D2-901F-00C04FB951ED")]
        UsbDevice,

        /// <summary>
        /// USB Host Controller
        /// </summary>
        [Description("USB Host Controller")]
        [GuidAttribute("3ABF6F2D-71C4-462A-8A92-1E6861E6AF27")]
        UsbHostController,

        /// <summary>
        /// USB Hub
        /// </summary>
        [Description("USB Hub")]
        [GuidAttribute("F18A0E88-C30C-11D0-8815-00A0C906BED8")]
        UsbHub,

        /// <summary>
        /// Video Output Arrival
        /// </summary>
        [Description("Video Output Arrival")]
        [GuidAttribute("1AD9E4F0-F88D-4360-BAB9-4C2D55E564CD")]
        VideoOutputArrival,

        /// <summary>
        /// Volume
        /// </summary>
        [Description("Volume")]
        [GuidAttribute("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B")]
        Volume,

        /// <summary>
        /// Windows Portable Devices
        /// </summary>
        [Description("Windows Portable Devices")]
        [GuidAttribute("6AC27878-A6FA-4155-BA85-F98F491D4F33")]
        WPD,

        /// <summary>
        /// Private Windows Portable Devices
        /// </summary>
        [Description("Private Windows Portable Devices")]
        [GuidAttribute("BA0C718F-4DED-49B7-BDD3-FABE28661211")]
        WPDPrivate,

        /// <summary>
        /// Write on CE Disk
        /// </summary>
        [Description("Write on CE Disk")]
        [GuidAttribute("53F5630C-B6BF-11D0-94F2-00A0C91EFB8B")]
        WriteOnCeDisk,

        // non official
        
        /// <summary>
        /// Windows Portable Devices 2
        /// </summary>
        [Description("Windows Portable Devices 2")]
        [GuidAttribute("F33FDC04-D1AC-4E8E-9A30-19BBD4B108AE")]
        WPD2,

        /// <summary>
        /// Microsoft UMBus Root Bus Enumerator generic
        /// </summary>
        [Description("Microsoft UMBus Root Bus Enumerator generic")]
        [GuidAttribute("65a9a6cf-64cd-480b-843e-32c86e1ba19f")]
        UMBus,

        /// <summary>
        /// Rendering Control
        /// </summary>
        [Description("Rendering Control")]
        [GuidAttribute("8660e926-ff3d-580c-959e-8b8af44d7cde")]
        RenderingControl,

        /// <summary>
        /// Connection Manager
        /// </summary>
        [Description("Connection Manager")]
        [GuidAttribute("ae9eb9c4-8819-51d8-879d-9a42ffb89d4e")]
        ConnectionManager,

        /// <summary>
        /// AV Transport
        /// </summary>
        [Description("AV Transport")]
        [GuidAttribute("4c38e836-6a2f-5949-9406-1788ea20d1d5")]
        AVTransport,

        /// <summary>
        /// Content Directory
        /// </summary>
        [Description("Content Directory")]
        [GuidAttribute("575d078a-63b9-5bc0-958b-87cc35b279cc")]
        ContentDirectory,


        // ???? unknown 57edcd85-0281-4893-a224-6719f892b1a4 


        // kernel streaming Category
        // https://docs.microsoft.com/en-us/windows-hardware/drivers/install/kscategory-acoustic-echo-cancel

        /// <summary>
        /// Acoustic Echo Cancel
        /// </summary>
        [Description("Acoustic Echo Cancel")]
        [GuidAttribute("BF963D80-C559-11D0-8A2B-00A0C9255AC1")]
        AcousticEchoCancel,

        /// <summary>
        /// Audio
        /// </summary>
        [Description("Audio")]
        [GuidAttribute("6994AD04-93EF-11D0-A3CC-00A0C9223196")]
        Audio,

        /// <summary>
        /// Audio Device
        /// </summary>
        [Description("Audio Device")]
        [GuidAttribute("FBF6F530-07B9-11D2-A71E-0000F8004788")]
        AudioDevice,

        /// <summary>
        /// Audio GFX
        /// </summary>
        [Description("Audio GFX")]
        [GuidAttribute("9BAF9572-340C-11D3-ABDC-00A0C90AB16F")]
        AudioGFX,

        /// <summary>
        /// Audio Splitter
        /// </summary>
        [Description("Audio Splitter")]
        [GuidAttribute("9EA331FA-B91B-45F8-9285-BD2BC77AFCDE")]
        AudioSplitter,

        /// <summary>
        /// Broadcast Driver Architecture sink filter
        /// </summary>
        [Description("BDA IP Sink")]
        [GuidAttribute("71985F4A-1CA1-11d3-9CC8-00C04F7971E0")]
        BDAIPSink,

        // todo

        /// <summary>
        /// Bridge
        /// </summary>
        [Description("Bridge")]
        [GuidAttribute("085AFF00-62CE-11CF-A5D6-28DB04C10000")]
        Bridge,

        /// <summary>
        /// Capture WAV and MIDI data
        /// </summary>
        [Description("Capture")]
        [GuidAttribute("65E8773D-8F56-11D0-A3B9-00A0C9223196")]
        Capture,
        
        /// <summary>
        /// Clock
        /// </summary>
        [Description("Clock")]
        [GuidAttribute("53172480-4791-11D0-A5D6-28DB04C10000")]
        Clock,


        /// <summary>
        /// Mixer
        /// </summary>
        [Description("Mixer")]
        [GuidAttribute("AD809C00-7B88-11D0-A5D6-28DB04C10000")]
        Mixer,

        /// <summary>
        /// Network
        /// </summary>
        [Description("Network")]
        [GuidAttribute("67C9CC3C-69C4-11D2-8759-00A0C9223196")]
        Network,

        /// <summary>
        /// Preferred Midi Out Device
        /// </summary>
        [Description("Preferred Midi Out Device")]
        [GuidAttribute("D6C50674-72C1-11D2-9755-0000F8004788")]
        PreferredMidiOutDevice,

        /// <summary>
        /// Preferred Wave In Device
        /// </summary>
        [Description("Preferred Wave In Device")]
        [GuidAttribute("D6C50671-72C1-11D2-9755-0000F8004788")]
        PreferredWaveInDevice,

        /// <summary>
        /// Preferred Wave Out Device
        /// </summary>
        [Description("Preferred Wave Out Device")]
        [GuidAttribute("D6C5066E-72C1-11D2-9755-0000F8004788")]
        PreferredWaveOutDevice,

        /// <summary>
        /// Proxy
        /// </summary>
        [Description("Proxy")]
        [GuidAttribute("97EBAACA-95BD-11D0-A3EA-00A0C9223196")]
        Proxy,

        /// <summary>
        /// Quality
        /// </summary>
        [Description("Quality")]
        [GuidAttribute("97EBAACB-95BD-11D0-A3EA-00A0C9223196")]
        Quality,

        /// <summary>
        /// Realtime
        /// </summary>
        [Description("Realtime")]
        [GuidAttribute("EB115FFC-10C8-4964-831D-6DCB02E6F23F")]
        Realtime,

        /// <summary>
        /// Render WAV and MIDI data
        /// </summary>
        [Description("Render")]
        [GuidAttribute("65E8773E-8F56-11D0-A3B9-00A0C9223196")]
        Render,

        /// <summary>
        /// Sensor Camera
        /// </summary>
        [Description("Sensor Camera")]
        [GuidAttribute("24E552D7-6523-47F7-A647-D3465BF1F5CA")]
        SensorCamera,

        /// <summary>
        /// Splitter
        /// </summary>
        [Description("Splitter")]
        [GuidAttribute("0A4252A0-7E70-11D0-A5D6-28DB04C10000")]
        Splitter,

        /// <summary>
        /// Synthesizer
        /// </summary>
        [Description("Synthesizer")]
        [GuidAttribute("DFF220F3-F70F-11D0-B917-00A0C9223196")]
        Synthesizer,
    }
}
