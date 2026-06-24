using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WindBar.Core;

namespace WindBar.App.Services
{
    public sealed class AppBarService : IDisposable
    {
        private const int ABM_NEW = 0x00000000;
        private const int ABM_REMOVE = 0x00000001;
        private const int ABM_QUERYPOS = 0x00000002;
        private const int ABM_SETPOS = 0x00000003;
        private const int ABE_LEFT = 0;
        private const int ABE_TOP = 1;
        private const int ABE_RIGHT = 2;
        private const int ABE_BOTTOM = 3;

        private IntPtr _windowHandle;
        private bool _registered;

        public bool RegisterOrUpdate(Window window, BarEdge edge, int thickness)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero)
                return false;

            _windowHandle = handle;
            if (!_registered)
            {
                var add = CreateAppBarData(handle);
                if (SHAppBarMessage(ABM_NEW, ref add) == UIntPtr.Zero)
                    return false;

                _registered = true;
            }

            return SetPosition(window, edge, thickness);
        }

        public void Unregister()
        {
            if (!_registered || _windowHandle == IntPtr.Zero)
                return;

            var data = CreateAppBarData(_windowHandle);
            SHAppBarMessage(ABM_REMOVE, ref data);
            _registered = false;
            _windowHandle = IntPtr.Zero;
        }

        private static bool SetPosition(Window window, BarEdge edge, int thickness)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero)
                return false;

            var data = CreateAppBarData(handle);
            data.uEdge = edge == BarEdge.Top ? ABE_TOP : ABE_BOTTOM;
            data.rc.Left = 0;
            data.rc.Top = 0;
            data.rc.Right = (int)Math.Round(SystemParameters.PrimaryScreenWidth);
            data.rc.Bottom = (int)Math.Round(SystemParameters.PrimaryScreenHeight);

            SHAppBarMessage(ABM_QUERYPOS, ref data);

            if (edge == BarEdge.Top)
                data.rc.Bottom = data.rc.Top + thickness;
            else
                data.rc.Top = data.rc.Bottom - thickness;

            SHAppBarMessage(ABM_SETPOS, ref data);

            window.Left = data.rc.Left;
            window.Top = data.rc.Top;
            window.Width = Math.Max(1, data.rc.Right - data.rc.Left);
            window.Height = Math.Max(1, data.rc.Bottom - data.rc.Top);
            MoveWindow(handle, data.rc.Left, data.rc.Top, data.rc.Right - data.rc.Left, data.rc.Bottom - data.rc.Top, true);
            return true;
        }

        private static APPBARDATA CreateAppBarData(IntPtr handle)
        {
            return new APPBARDATA
            {
                cbSize = Marshal.SizeOf<APPBARDATA>(),
                hWnd = handle
            };
        }

        public void Dispose()
        {
            Unregister();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern UIntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
    }
}
