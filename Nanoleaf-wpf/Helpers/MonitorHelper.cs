using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Winleafs.Wpf.Helpers
{
    public static class MonitorHelper
    {
        [DllImport("user32.dll")]
#pragma warning disable S4214 // "P/Invoke" methods should not be visible
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref MonitorInfo monitorInfo);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

#pragma warning restore S4214 // "P/Invoke" methods should not be visible

        public static Rectangle GetScreenBounds(int monitorIndex)
        {
            // We select monitors via the new API but the MonitorInfo class works via the Windows Forms diaply name.
            // Hence we retrieve the selected WIndows Forms monitor via our settings and then by comapring the DevicePath
            var monitors = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets();
            var formsMonitors = WindowsDisplayAPI.Display.GetDisplays();

            var selectedMonitor = formsMonitors.FirstOrDefault(monitor => monitor.DevicePath.Equals(monitors[monitorIndex].DevicePath));

            var monitorInfo = new MonitorInfo();
            EnumDisplaySettings(selectedMonitor.DisplayName, -1, ref monitorInfo);

            return new Rectangle(monitorInfo.dmPositionX, monitorInfo.dmPositionY, monitorInfo.dmPelsWidth, monitorInfo.dmPelsHeight);
        }

        //https://stackoverflow.com/questions/29438430/how-to-get-dpi-scale-for-all-screens
        public static void GetDpi(int left, int top, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new Point(left + 10, top + 10);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
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

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
}
