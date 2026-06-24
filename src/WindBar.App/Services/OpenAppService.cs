using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace WindBar.App.Services
{
    public sealed class OpenAppService
    {
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        public sealed class OpenApp
        {
            public string Title { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
            public IntPtr WindowHandle { get; set; }
            public bool IsActive { get; set; }
        }

        public IEnumerable<OpenApp> GetOpenApps()
        {
            var currentProcessId = Environment.ProcessId;
            var foreground = GetForegroundWindow();
            return Process.GetProcesses()
                .Where(process => process.Id != currentProcessId)
                .Where(process => process.MainWindowHandle != IntPtr.Zero)
                .Where(process => !string.IsNullOrWhiteSpace(process.MainWindowTitle))
                .Select(process => new OpenApp
                {
                    Title = process.MainWindowTitle,
                    ProcessName = process.ProcessName,
                    WindowHandle = process.MainWindowHandle,
                    IsActive = process.MainWindowHandle == foreground
                })
                .OrderByDescending(app => app.IsActive)
                .ThenBy(app => app.ProcessName)
                .ThenBy(app => app.Title)
                .ToList();
        }

        public void ActivateOrToggle(OpenApp app)
        {
            if (app.WindowHandle == IntPtr.Zero) return;
            if (GetForegroundWindow() == app.WindowHandle)
            {
                ShowWindow(app.WindowHandle, SW_MINIMIZE);
                return;
            }

            ShowWindow(app.WindowHandle, SW_RESTORE);
            SetForegroundWindow(app.WindowHandle);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
