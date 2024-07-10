using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using CustomDevices;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

namespace MultiMouse
{
    public class InputRedirector : MonoBehaviour
    {
        private const int RawDeviceCount = 1;
        private static readonly WndProcDelegate SWndProc = WndProc;
        private static readonly IntPtr SHInstance = User32.GetModuleHandleW(null);
        private static readonly List<IntPtr> _deviceHandles = new();
        private static Rect _screen;
        private static PlayerManager _playerManager;
        private static Vector2 mouseOne = Vector2.zero;
        private static Vector2 mouseTwo = Vector2.zero;
        private IntPtr _hwnd;
        private User32.RawInputDevice[] _queryDevices = new User32.RawInputDevice[RawDeviceCount];
        private IntPtr _windowClass;

        public void Awake()
        {
            _windowClass = RegisterWindowClass();
            _hwnd = User32.CreateWindowExW(0, _windowClass, "Raw Input Redirection Window", 0, 0, 0, 0, 0,
                IntPtr.Zero,
                IntPtr.Zero, SHInstance, IntPtr.Zero);
            if (_hwnd == null)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to create raw input redirection window.");


            InputSystem.AddDevice<MultiMouseDevice>();
            _playerManager = FindObjectOfType<PlayerManager>();
            _screen = new Rect(0, 0, 1280, 720);
            InputSystem.QueueStateEvent(MultiMouseDevice.current, new MultiMouseDeviceState
            {
                mouseOnePosition = _screen.center,
                mouseTwoPosition = _screen.center
            });

            RegisterDevices();
        }

        public void Update()
        {
            var numberOfDevices = 0;
            User32.GetRegisteredRawInputDevices(null, ref numberOfDevices, Marshal.SizeOf<User32.RawInputDevice>());
            if (_queryDevices.Length < numberOfDevices)
                Array.Resize(ref _queryDevices, numberOfDevices);

            if (User32.GetRegisteredRawInputDevices(_queryDevices, ref numberOfDevices,
                    Marshal.SizeOf<User32.RawInputDevice>()) == -1)
            {
                var error = Marshal.GetLastWin32Error();
                Debug.LogError("GetRegisteredRawInputDevices failed: " + new Win32Exception(error).Message);
            }
            else
            {
                for (var i = 0; i < numberOfDevices; i++)
                    if (_queryDevices[i].UsUsage == User32.HidUsageGenericMouse ||
                        _queryDevices[i].UsUsage == User32.HidUsageGenericKeyboard)
                        if (_queryDevices[i].HwndTarget != _hwnd)
                        {
                            RegisterDevices();
                            break;
                        }
            }
        }

        public void FixedUpdate()
        {
            InputSystem.QueueStateEvent(MultiMouseDevice.current, new MultiMouseDeviceState
            {
                mouseOnePosition = mouseOne,
                mouseTwoPosition = mouseTwo
            });
        }

        private void OnDestroy()
        {
            if (!User32.DestroyWindow(_hwnd))
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "Failed to destroy raw input redirection window class.");

            if (!User32.UnregisterClassW(_windowClass, SHInstance))
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "Failed to unregister raw input redirection window class.");
        }

        private static IntPtr RegisterWindowClass()
        {
            var wndClass = new User32.WndClassExw
            {
                CbSize = Marshal.SizeOf<User32.WndClassExw>(),
                LpfnWndProc = Marshal.GetFunctionPointerForDelegate(SWndProc),
                HInstance = SHInstance,
                LpszClassName = "RawInputRedirector"
            };

            var registeredClass = User32.RegisterClassExW(ref wndClass);
            if (registeredClass == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to register the window class.");

            return new IntPtr(registeredClass);
        }

        private void RegisterDevices()
        {
            var mouse = new User32.RawInputDevice
            {
                UsUsagePage = User32.HidUsagePageGeneric,
                UsUsage = User32.HidUsageGenericMouse,
                HwndTarget = _hwnd
            };

            var devices = new User32.RawInputDevice[RawDeviceCount];
            devices[0] = mouse;

            if (!User32.RegisterRawInputDevices(devices, RawDeviceCount, Marshal.SizeOf<User32.RawInputDevice>()))
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "Failed to register mouse and keyboard for Raw Input.");
        }

        [MonoPInvokeCallback(typeof(WndProcDelegate))]
        private static IntPtr WndProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (message == User32.WmInput)
                    ProcessRawInputMessage(lParam);

                return User32.DefWindowProcW(window, message, wParam, lParam);
            }
            catch (Exception e)
            {
                // Never let exception escape to native code as that will crash the app
                Debug.LogException(e);
                return IntPtr.Zero;
            }
        }

        private static void ProcessRawInputMessage(IntPtr lParam)
        {
            var dwSize = 0;
            User32.GetRawInputData(lParam, User32.RidInput, null, ref dwSize,
                Marshal.SizeOf<User32.RawInputHeader>());
            var lpb = new byte[dwSize];

            if (User32.GetRawInputData(lParam, User32.RidInput, lpb, ref dwSize,
                    Marshal.SizeOf<User32.RawInputHeader>()) == -1)
            {
                Debug.LogError("GetRawInputData does not return correct size!");
                return;
            }

            var rawInputHandle = ByteArrayToHandle(lpb);
            var rawInputPtr = rawInputHandle.AddrOfPinnedObject();
            var rawInput = Marshal.PtrToStructure<User32.RawInput>(rawInputPtr);

            var rawHeader = rawInput.Header;
            var rawData = rawInput.Data;

            var rawHeaderPtr = Marshal.AllocHGlobal(Marshal.SizeOf<User32.RawInputHeader>());
            var rawDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<User32.InputData>());

            Marshal.StructureToPtr(rawHeader, rawHeaderPtr, false);
            Marshal.StructureToPtr(rawData, rawDataPtr, false);

            // Is mouse
            if (rawHeader.DwType == 0)
            {
                if (!_deviceHandles.Any(ptr => ptr.Equals(rawHeader.HDevice))) _deviceHandles.Add(rawHeader.HDevice);
                var mouse = rawData.Mouse;

                var currentDevice = MultiMouseDevice.current;

                if (_deviceHandles[0].Equals(rawHeader.HDevice))
                {
                    if (mouse.Buttons.usButtonFlags == 0x0001) _playerManager.AddMultiMousePlayer(true);

                    var mouseVector = currentDevice.mouseOnePosition.value;
                    var mouseMove = new Vector2(mouse.LastX, -mouse.LastY);
                    mouseVector += mouseMove;
                    if (_screen.Contains(mouseVector))
                        mouseOne = mouseVector;
                }
                else if (_deviceHandles[1].Equals(rawHeader.HDevice))
                {
                    if (mouse.Buttons.usButtonFlags == 0x0001) _playerManager.AddMultiMousePlayer(false);

                    var mouseVector = currentDevice.mouseTwoPosition.value;
                    var mouseMove = new Vector2(mouse.LastX, -mouse.LastY);
                    mouseVector += mouseMove;
                    if (_screen.Contains(mouseVector))
                        mouseTwo = mouseVector;
                }
            }

            Input.ForwardRawInput(rawHeaderPtr, rawDataPtr, 1, rawInputPtr, (uint)rawHeader.DwSize);
            rawInputHandle.Free();
        }

        private static GCHandle ByteArrayToHandle(byte[] bytes)
        {
            var pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return pinnedArray;
        }

        private delegate IntPtr WndProcDelegate(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    }
}