using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CMVDDC
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //------------------common defined-----------------------//
        public const Int32 MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;
        public const Int32 PHYSICAL_MONITOR_DESCRIPTION_SIZE = 128;


        public const uint MC_CAPS_NONE = 0x00000000;
        public const uint MC_CAPS_MONITOR_TECHNOLOGY_TYPE = 0x00000001;
        public const uint MC_CAPS_BRIGHTNESS = 0x00000002;
        public const uint MC_CAPS_CONTRAST = 0x00000004;
        public const uint MC_CAPS_COLOR_TEMPERATURE = 0x00000008;
        public const uint MC_CAPS_RED_GREEN_BLUE_GAIN = 0x00000010;
        public const uint MC_CAPS_RED_GREEN_BLUE_DRIVE = 0x00000020;
        public const uint MC_CAPS_DEGAUSS = 0x00000040;
        public const uint MC_CAPS_DISPLAY_AREA_POSITION = 0x00000080;
        public const uint MC_CAPS_DISPLAY_AREA_SIZE = 0x00000100;
        public const uint MC_CAPS_RESTORE_FACTORY_DEFAULTS = 0x00000400;
        public const uint MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS = 0x00000800;
        public const uint MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS = 0x00001000;


        //------------------import the dll-----------------------//
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true, EntryPoint = "GetMonitorCapabilities")]
        public static extern bool GetMonitorCapabilities(
            IntPtr hMonitor, ref uint pdwMonitorCapabilities, ref uint pdwSupportedColorTemperatures);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors", SetLastError = true)]
        public static extern bool DestroyPhysicalMonitors(
                uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("gdi32.dll", EntryPoint = "DDCCIGetCapabilitiesStringLength", SetLastError = true)]
        public static extern bool DDCCIGetCapabilitiesStringLength(
                [In] IntPtr hMonitor, ref uint pdwLength);

        [DllImport("gdi32.dll", EntryPoint = "DDCCIGetCapabilitiesString", SetLastError = true)]
        public static extern bool DDCCIGetCapabilitiesString(
            [In] IntPtr hMonitor, StringBuilder pszString, uint dwLength);

        [DllImport("gdi32.dll", EntryPoint = "DDCCIGetVCPFeature", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DDCCIGetVCPFeature([In] IntPtr hMonitor, [In] uint dwVCPCode, uint pvct, ref uint pdwCurrentValue, ref uint pdwMaximumValue);

        //------------------define the struct-----------------------//
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = PHYSICAL_MONITOR_DESCRIPTION_SIZE)]
            public char[] szPhysicalMonitorDescription;
        }

        #region Nested type: RECT
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static readonly RECT Empty;

            public int Width
            {
                get { return Math.Abs(right - left); }
            }
            public int Height
            {
                get { return bottom - top; }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }
        #endregion







        public MainWindow()
        {
            InitializeComponent();

            InitializeWindowsViewComponent();

            
        }

        private void InitializeWindowsViewComponent()
        {
            LBLInfo.Background = Brushes.Yellow;
            LBLInfo.Content = "Waitting";

            TBLog.Text += "Starting connect the monitor via DDC port" + "\n";

        }


        private void BTNConnectMonitor_Click(object sender, RoutedEventArgs e)
        {
            uint cPhysicalMonitors;
            
            var hwnd = new WindowInteropHelper(this).EnsureHandle();
            IntPtr CurrentMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            //TBLog.Text += "CurMonitor: " + CurrentMonitor.ToString() + "\n";

            //GetMonitorDetailedInformation(CurrentMonitor);

            if (GetNumberOfPhysicalMonitorsFromHMONITOR(CurrentMonitor, out cPhysicalMonitors) == false)
            {
                TBLog.Text += "GetNumberOfPhysicalMonitorsFromHMONITOR Error \n";
                return;
            }

            TBLog.Text += "Number of Monitor: " + cPhysicalMonitors.ToString() + "\n";
            PHYSICAL_MONITOR[] physicalMonitorArray = new PHYSICAL_MONITOR[cPhysicalMonitors];
            Array.Clear(physicalMonitorArray, 0, (int)cPhysicalMonitors);

            if (!GetPhysicalMonitorsFromHMONITOR(CurrentMonitor, cPhysicalMonitors, physicalMonitorArray))
            {
                TBLog.Text += "GetPhysicalMonitorsFromHMONITOR Error \n";
                return;
            }

            String mystring = new String(physicalMonitorArray[0].szPhysicalMonitorDescription);
            TBLog.Text += "Name: " + mystring + "\n";


            uint uiCapabilityies = 0, uiColorTemp = 0;
            if (!GetMonitorCapabilities(physicalMonitorArray[0].hPhysicalMonitor, ref uiCapabilityies, ref uiColorTemp))
            {
                TBLog.Text += "Don't Support the MSDN provide API to get Brightness..etc. \nUse the VCP API to get the customize data\n";
            }

            DistinguishCaps(uiCapabilityies, uiColorTemp, physicalMonitorArray);

            if (CBInternalFunction.IsChecked == true)
            {
                ShowInternalCaps(physicalMonitorArray[0].hPhysicalMonitor);
            }
            
            SendVCPCommand(physicalMonitorArray[0].hPhysicalMonitor);

            DestroyPhysicalMonitors(cPhysicalMonitors, physicalMonitorArray);
        }

        private void SendVCPCommand(IntPtr hPhysicalMonitor)
        {
            TBLog.Text += "Go to VCP method\n";

            uint uiCurrentValue = 0, uiMaxValue = 0;
            DDCCIGetVCPFeature(hPhysicalMonitor, 0x10, 0, ref uiCurrentValue, ref uiMaxValue);
            TBLog.Text += "_DDCCI_OPCODE_VCP_BACKLIGHT: " + uiCurrentValue.ToString() + " " + uiMaxValue.ToString() + "\n";

            uiCurrentValue = 0;
            uiMaxValue = 0;
            DDCCIGetVCPFeature(hPhysicalMonitor, 0x12, 0, ref uiCurrentValue, ref uiMaxValue);
            TBLog.Text += "_DDCCI_OPCODE_VCP_CONTRAST: " + uiCurrentValue.ToString() + " " + uiMaxValue.ToString() + "\n";
        }

        private void ShowInternalCaps(IntPtr hPhysicalMonitor)
        {
            uint capabilitiesStringLength = 0u;
            var capabilitiesStringLengthSuccess = DDCCIGetCapabilitiesStringLength(hPhysicalMonitor, ref capabilitiesStringLength);

            var capabilitiesString = new StringBuilder((int)capabilitiesStringLength + 1);
            var capabilitiesStringSuccess = DDCCIGetCapabilitiesString(hPhysicalMonitor, capabilitiesString, (uint)capabilitiesString.Capacity);
            TBCaps.Text += "Caps from internal function which from Scalar board: \n" + capabilitiesString.ToString() + "\n";
        }

        private void DistinguishCaps(uint uiCapabilityies, uint uiColorTemp, PHYSICAL_MONITOR[] physicalMonitorArray)
        {
            if (uiCapabilityies == MC_CAPS_NONE)
            {
                TBLog.Text += "Support: " + uiCapabilityies.ToString() + "\n Don't support anything \n";
            }
            if (uiColorTemp == MC_CAPS_NONE)
            {
                TBLog.Text += " Support Temp: " + uiColorTemp.ToString() + "\n Don't support any temperture \n";
            }

            //This is one of the API for monitor information sample
            double dBrightValue = 0;
            if ((uiCapabilityies & MC_CAPS_BRIGHTNESS) == MC_CAPS_BRIGHTNESS)
            {
                dBrightValue = GetBrightness(physicalMonitorArray[0]);
                TBLog.Text += "Brightness value " + dBrightValue + "\n";
            }
        }

        private double GetBrightness(PHYSICAL_MONITOR pHYSICAL_MONITOR)
        {
            uint dwMinimumBrightness, dwCurrentBrightness, dwMaximumBrightness;
            if (!GetMonitorBrightness(pHYSICAL_MONITOR.hPhysicalMonitor, out dwMinimumBrightness, out dwCurrentBrightness, out dwMaximumBrightness))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return (double)(dwCurrentBrightness - dwMinimumBrightness) / (double)(dwMaximumBrightness - dwMinimumBrightness);
        }

        //FIXME: This is function is not working
        private void GetMonitorDetailedInformation(IntPtr CurrentMonitor)
        {
            MONITORINFO MonitorInfo = new MONITORINFO();
            if (GetMonitorInfo(CurrentMonitor, ref MonitorInfo) == false)
            {
                TBLog.Text += "GetMonitorInfo Failed \n";
                return;
            }

            RECT rcWorkArea = MonitorInfo.rcWork;
            RECT rcMonitorArea = MonitorInfo.rcMonitor;

            TBLog.Text += ("Width:" + rcWorkArea.left.ToString() + " " + rcMonitorArea.left.ToString() + "\n");
            TBLog.Text += ("Height:" + rcWorkArea.top.ToString() + " " + rcMonitorArea.top.ToString() + "\n");
            TBLog.Text += ("Width:" + rcWorkArea.right.ToString() + " " + rcWorkArea.left.ToString() + "\n");
            TBLog.Text += ("Height:" + rcWorkArea.bottom.ToString() + " " + rcWorkArea.top.ToString() + "\n");
        }
    }
}
