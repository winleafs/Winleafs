﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsDisplayAPI.DisplayConfig;

namespace Winleafs.Wpf.Helpers
{
    public static class ScreenBoundsHelper
    {
        [DllImport("user32.dll")]
        internal static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref MonitorInfo monitorInfo);

        public static Rectangle GetScreenBounds(int monitorIndex)
        {
            // We select monitors via the new API but the MonitorInfo class works via the Windows Forms diaply name.
            // Hence we retrieve the selected WIndows Forms monitor via our settings and then by comapring the DevicePath
            var monitors = GetMonitors();
            var formsMonitors = WindowsDisplayAPI.Display.GetDisplays();

            var selectedMonitor = formsMonitors.FirstOrDefault(monitor => monitor.DevicePath.Equals(monitors[monitorIndex].DevicePath));

            var monitorInfo = new MonitorInfo();
            EnumDisplaySettings(selectedMonitor.DisplayName, -1, ref monitorInfo);
            return new Rectangle(monitorInfo.dmPositionX, monitorInfo.dmPositionY, monitorInfo.dmPelsWidth, monitorInfo.dmPelsHeight);
        }

        public static List<PathDisplayTarget> GetMonitors()
        {
            return PathDisplayTarget.GetDisplayTargets().OrderBy(monitor => monitor.FriendlyName).ToList();
        }

        public static List<string> GetMonitorNames()
        {
            return GetMonitors().Select(monitor => monitor.FriendlyName).ToList();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        private const int CCHDEVICENAME = 0x20;
        private const int CCHFORMNAME = 0x20;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public ScreenOrientation dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }
}
