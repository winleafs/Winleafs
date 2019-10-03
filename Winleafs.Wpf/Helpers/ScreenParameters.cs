/*
 * Big thanks to https://github.com/morbius1st/ScreenParameters
 * for providing this code
 */

#region + Using Directives

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Point = System.Windows.Point;
using PointConverter = System.Drawing.PointConverter;
using Size = System.Windows.Size;

#endregion

namespace Winleafs.Wpf.Helpers
{
	public class ScreenParameters
	{
	#region + consts

		private const double NativeScreenDpi = 96.0;
		private const int    CCHDEVICENAME   = 32;

	#endregion

	#region + properties

		// private method to get the handle of the window
		// this keeps this class contained / not dependant
		public static int GetNativeScreenDpi
		{
			get => (int) NativeScreenDpi;
		}

	#endregion

	#region + public

		// get the name of the monitor
		public static string GetMonitorName(Window win)
		{
			return GetMonitorInfo(GetWindowHandle(win)).DeviceName;
		}

		// the actual screen DPI adjusted for the scaling factor
		public static int GetScreenDpi(Window win)
		{
			return GetDpiForWindow(GetWindowHandle(win));
		}

		// this is the ratio of the current screen Dpi
		// and the base Dpi
		public static double GetScreenScaleFactor(Window win)
		{ 
			return (GetScreenDpi(win) / NativeScreenDpi);
		}

		// this is the ratio of the current screen Dpi
		// and the base Dpi for non-dpi aware programs
		public static double GetScreenScaleFactorNonDpiAware(Window win)
		{
			Rectangle scaledRC = GetScaledScreenSize(win);

			Rectangle nonDpiRC = GetNativeScreenSizeNonDpiAware(win);

			// ReSharper disable once PossibleLossOfFraction
			return (double) nonDpiRC.Width / (double) scaledRC.Width;
		}

		// this is the conversion factor between screen coordinates 
		// and sizes and their actual actual coordinate and size
		// e.g. for a screen set to 125%, this factor applied 
		// to the native screen dimensions, will provide the 
		// actual screen dimensions
		public static double GetScreenScalingFactor(Window win)
		{
			return 1 / GetScreenScaleFactor(win);
		}

		// this is the ratio of the current screen Dpi
		// and the base Dpi for non-dpi aware programs
		public static double GetScreenScalingFactorNonDpiAware(Window win)
		{
			// ReSharper disable once PossibleLossOfFraction
			return 1/ GetScreenScaleFactorNonDpiAware(win);
		}

		// get the dimensions of the physical / native screen
		// ignoring any applied scaling
		public static Rectangle GetNativeScreenSize(Window win)
		{
			MONITORINFOEX mi = GetMonitorInfo(GetWindowHandle(win));

			return ConvertRectToRectangle(mi.rcMonitor);
		}

		// get the dimensions of the physical / native screen
		// ignoring any applied scaling
		public static Rectangle GetNativeScreenSizeNonDpiAware(Window win)
		{
			DEVMODE dm = DISPLAYSETTINGSEX(win);

			return ConvertRectToRectangle(dm);
		}

		public static Rectangle GetNativeWorkArea(Window win)
		{
			MONITORINFOEX mi = GetMonitorInfo(GetWindowHandle(win));

			return ConvertRectToRectangle(mi.rcWorkArea);
		}

		// get the screen dimensions taking the screen scaling into account
		public static Rectangle GetScaledScreenSize(Window win)
		{
			double ScalingFactor = GetScreenScalingFactor(win);

			Rectangle rc = GetNativeScreenSize(win);

			if (ScalingFactor == 1) return rc;

			return rc.Scale(ScalingFactor);
		}

		// get the screen dimensions taking the screen scaling into account
		public static Rectangle GetScaledWorkArea(Window win)
		{
			double ScalingFactor = GetScreenScalingFactor(win);

			Rectangle rc = GetNativeWorkArea(win);

			if (ScalingFactor == 1) return rc;

			return rc.Scale(ScalingFactor);
		}

		public static bool IsProcessDPIAware(out PROCESS_DPI_AWARENESS awareness)
		{
			System.Diagnostics.Process proc = Process.GetCurrentProcess();

			int result = GetProcessDpiAwareness(proc.Handle, out awareness);

			if (result != 0)
			{
				return false;
			}

			return true;
		}

		public static uint GetDpiForMonitor(Window win, MONITOR_DPI_TYPE dpiType)
		{
			IntPtr hMonitor = MonitorFromWindow(GetWindowHandle(win), 0);

			uint dpiX;
			uint dpiY;

			int result =
				GetDpiForMonitor(hMonitor, dpiType, out dpiX, out dpiY);

			if (result != 0) return 0;

			return dpiX;

		}

		public static DEVMODE DISPLAYSETTINGSEX(Window win)
		{
			DEVMODE dm = new DEVMODE();

			string devname = GetMonitorName(win);

			dm.dmSize = (short) Marshal.SizeOf(dm);

			EnumDisplaySettingsEx(devname, ENUM_CURRENT_SETTINGS, ref dm, (uint) dwFlags.MONITOR_DEFAULTTONEAREST);

			return dm;

		}

		internal static MONITORINFOEX GetMonitorInfo(IntPtr ptr)
		{
			IntPtr hMonitor = MonitorFromWindow(ptr, 0);

			MONITORINFOEX mi = new MONITORINFOEX();
			mi.Init();
			GetMonitorInfo(hMonitor, ref mi);

			return mi;
		}

		public static Rectangle ConvertRectToRectangle(RECT rc)
		{
			return new Rectangle(rc.Left, rc.Top,
				rc.Right - rc.Left, rc.Bottom - rc.Top);
		}

		public static Rectangle ConvertRectToRectangle(DEVMODE dm)
		{
			return new Rectangle(dm.dmPosition.x, dm.dmPosition.y,
				dm.dmPelsWidth, dm.dmPelsHeight);
		}

		// private method to get the handle of the window
		// this keeps this class contained / not dependant
		public static IntPtr GetWindowHandle(Window win)
		{
			return new WindowInteropHelper(win).Handle;
		}

	#endregion

	#region + Dll Imports

		[DllImport("user32.dll")]
		internal static extern UInt16 GetDpiForWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		internal static extern UInt16 GetDpiForSystem();

		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

		[DllImport("user32.dll")]
		internal static extern bool EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);

		[DllImport("SHCore.dll", SetLastError = true)]
		private static extern int GetProcessDpiAwareness(IntPtr hprocess, out PROCESS_DPI_AWARENESS awareness);
		
		[DllImport("SHCore.dll", SetLastError = true)]
		private static extern int GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);


	#endregion

	#region + Dll Enums

		private const int ENUM_CURRENT_SETTINGS = -1;
		private const int ENUM_REGISTRY_SETTINGS = -2;

		public enum dwFlags : uint
		{
			MONITORINFO_PRIMARY = 1,
			MONITOR_DEFAULTTONEAREST = 2
		}

		public enum PROCESS_DPI_AWARENESS
		{
			PROCESS_DPI_UNAWARE = 0,
			PROCESS_SYSTEM_DPI_AWARE = 1,
			PROCESS_PER_MONITOR_DPI_AWARE = 2
		}

		public enum MONITOR_DPI_TYPE
		{
			MDT_EFFECTIVE_DPI,
			MDT_ANGULAR_DPI,
			MDT_RAW_DPI,
			MDT_DEFAULT
		} ;

	#endregion

	#region + Dll Structs

		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct MONITORINFOEX
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWorkArea;
			public dwFlags Flags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
			public string DeviceName;

			public void Init()
			{
				this.cbSize = 40 + 2 * CCHDEVICENAME;
				this.DeviceName = String.Empty;
			}
		}


		public struct POINTL
		{
			public Int32 x;
			public Int32 y;
		}

		// Selects duplex or double-sided printing for printers capable of duplex printing. 
		internal enum DM : short
		{
		DM_UPDATE           = 1,
		DM_COPY             = 2,
		DM_PROMPT           = 4,
		DM_MODIFY           = 8,

		DM_IN_BUFFER        = DM_MODIFY,
		DM_IN_PROMPT        = DM_PROMPT,
		DM_OUT_BUFFER       = DM_COPY,
		DM_OUT_DEFAULT      = DM_UPDATE,

		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DEVMODE
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmDeviceName;

			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;

//			public short dmOrientation;
//			public short dmPaperSize;
//			public short dmPaperLength;
//			public short dmPaperWidth;
//			public short dmScale;
//			public short dmCopies;
//			public short dmDefaultSource;
//			public short dmPrintQuality;

			public POINTL dmPosition;
			public int dmOrientation;
			public int dmFixedOutput;


			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;

			public short dmLogPixels;
			public short dmBitsPerPel;
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

//
//		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
//		struct DEVMODE
//		{
//			public const int CCHDEVICENAME = 32;
//			public const int CCHFORMNAME = 32;
//
//			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
//			[System.Runtime.InteropServices.FieldOffset(0)]
//			public string dmDeviceName;
//
//			[System.Runtime.InteropServices.FieldOffset(32)]
//			public Int16 dmSpecVersion;
//
//			[System.Runtime.InteropServices.FieldOffset(34)]
//			public Int16 dmDriverVersion;
//
//			[System.Runtime.InteropServices.FieldOffset(36)]
//			public Int16 dmSize;
//
//			[System.Runtime.InteropServices.FieldOffset(38)]
//			public Int16 dmDriverExtra;
//
//			[System.Runtime.InteropServices.FieldOffset(40)]
//			public DM dmFields;
//
//			[System.Runtime.InteropServices.FieldOffset(44)]
//			Int16 dmOrientation;
//
//			[System.Runtime.InteropServices.FieldOffset(46)]
//			Int16 dmPaperSize;
//
//			[System.Runtime.InteropServices.FieldOffset(48)]
//			Int16 dmPaperLength;
//
//			[System.Runtime.InteropServices.FieldOffset(50)]
//			Int16 dmPaperWidth;
//
//			[System.Runtime.InteropServices.FieldOffset(52)]
//			Int16 dmScale;
//
//			[System.Runtime.InteropServices.FieldOffset(54)]
//			Int16 dmCopies;
//
//			[System.Runtime.InteropServices.FieldOffset(56)]
//			Int16 dmDefaultSource;
//
//			[System.Runtime.InteropServices.FieldOffset(58)]
//			Int16 dmPrintQuality;
//
//			[System.Runtime.InteropServices.FieldOffset(44)]
//			public POINTL dmPosition;
//
//			[System.Runtime.InteropServices.FieldOffset(52)]
//			public Int32 dmDisplayOrientation;
//
//			[System.Runtime.InteropServices.FieldOffset(56)]
//			public Int32 dmDisplayFixedOutput;
//
//			[System.Runtime.InteropServices.FieldOffset(60)]
//			public short dmColor; // See note below!
//
//			[System.Runtime.InteropServices.FieldOffset(62)]
//			public short dmDuplex; // See note below!
//
//			[System.Runtime.InteropServices.FieldOffset(64)]
//			public short dmYResolution;
//
//			[System.Runtime.InteropServices.FieldOffset(66)]
//			public short dmTTOption;
//
//			[System.Runtime.InteropServices.FieldOffset(68)]
//			public short dmCollate; // See note below!
//
//			[System.Runtime.InteropServices.FieldOffset(70)]
//			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
//			public string dmFormName;
//
//			[System.Runtime.InteropServices.FieldOffset(102)]
//			public Int16 dmLogPixels;
//
//			[System.Runtime.InteropServices.FieldOffset(104)]
//			public Int32 dmBitsPerPel;
//
//			[System.Runtime.InteropServices.FieldOffset(108)]
//			public Int32 dmPelsWidth;
//
//			[System.Runtime.InteropServices.FieldOffset(112)]
//			public Int32 dmPelsHeight;
//
//			[System.Runtime.InteropServices.FieldOffset(116)]
//			public Int32 dmDisplayFlags;
//
//			[System.Runtime.InteropServices.FieldOffset(116)]
//			public Int32 dmNup;
//
//			[System.Runtime.InteropServices.FieldOffset(120)]
//			public Int32 dmDisplayFrequency;
//		}

	#endregion
	}

    public static class RectangleExtensions
    {
        public static Rectangle Scale(this Rectangle rc, double scaleFactor)
        {
            return new Rectangle(
                (int)(rc.Left * scaleFactor),
                (int)(rc.Top * scaleFactor),
                (int)(rc.Width * scaleFactor),
                (int)(rc.Height * scaleFactor));
        }

        public static string ToString(this Rectangle rc)
        {
            return string.Format("top|{0,5:D} left|{1,5:D} height|{2,5:D} width|{3,5:D}",
                rc.Top, rc.Left, rc.Height, rc.Width);
        }

    }
}