using System;
using System.Runtime.InteropServices;

namespace MultiMouse
{
    public static class User32
    {
        #region Structs

        public struct RawInput
        {
            public RawInputHeader Header;
            public InputData Data;
        }

        public struct InputData
        {
            public RawMouse Mouse;
            public RawKeyboard Keyboard;
            public RawHid Hid;
        }

        public struct RawInputHeader
        {
            public uint DwType;
            public int DwSize;
            public IntPtr HDevice;
            public IntPtr WParam;
        }

        // Mouse
        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouse
        {
            [FieldOffset(0)] public ushort usFlags;

            [FieldOffset(4)] public uint ulButtons;

            [FieldOffset(4)] public MouseButtonsStruct Buttons;

            [FieldOffset(8)] public uint ulRawButtons;

            [FieldOffset(12)] public int LastX;

            [FieldOffset(16)] public int LastY;

            [FieldOffset(20)] public uint ulExtraInformation;

            [StructLayout(LayoutKind.Sequential)]
            public struct MouseButtonsStruct
            {
                public ushort usButtonFlags;
                public ushort usButtonData;
            }
        }

        // Keyboard
        public struct RawKeyboard
        {
            public ushort MakeCode;
            public RawKeyboardFlags Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [Flags]
        public enum RawKeyboardFlags : ushort
        {
            RI_KEY_MAKE = 0,
            RI_KEY_BREAK = 1,
            RI_KEY_E0 = 2,
            RI_KEY_E1 = 4,
            RI_KEY_TERMSRV_SET_LED = 8,
            RI_KEY_TERMSRV_SHADOW = 0x10
        }

        // HID
        public struct RawHid
        {
            public uint DwSizeHid;
            public uint DwCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] BRawData;
        }

        // Device
        public struct RawInputDevice
        {
            public ushort UsUsagePage;
            public ushort UsUsage;
            public uint DwFlags;
            public IntPtr HwndTarget;
        }

        // Window
        public struct WndClassExw
        {
            public int CbSize;
            public int Style;
            public IntPtr LpfnWndProc;
            public int CbClsExtra;
            public int CbWndExtra;
            public IntPtr HInstance;
            public IntPtr HIcon;
            public IntPtr HCursor;
            public IntPtr HbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)] public string LpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)] public string LpszClassName;
            public IntPtr HIconSm;
        }

        #endregion

        #region Consts

        public const uint WmInput = 0x00FF;
        public const uint RidInput = 0x10000003;
        public const ushort HidUsagePageGeneric = 0x01;
        public const ushort HidUsageGenericMouse = 0x02;
        public const ushort HidUsageGenericKeyboard = 0x06;

        #endregion

        #region DllImports

        // Window
        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProcW(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern ushort RegisterClassExW([In] ref WndClassExw lpwcx);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowExW(uint dwExStyle, IntPtr windowClass,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance,
            IntPtr pvParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterClassW(IntPtr windowClass, IntPtr hInstance);

        // RawInput
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int GetRawInputData(IntPtr hRawInput, uint uiCommand,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pData, ref int pcbSize, int cbSizeHeader);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices(
            [In] [MarshalAs(UnmanagedType.LPArray)]
            RawInputDevice[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int GetRegisteredRawInputDevices(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray)]
            RawInputDevice[] pRawInputDevices, ref int puiNumDevices,
            int cbSize);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int GetRawInputBuffer([MarshalAs(UnmanagedType.LPArray)] byte[] pData, ref int pcbSize,
            int cbSizeHeader);

        #endregion
    }
}