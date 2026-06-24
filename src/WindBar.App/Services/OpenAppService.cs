using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WindBar.App.Services
{
    public sealed class OpenAppService
    {
        public sealed class OpenApp
        {
            public string Title { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
        }

        public IEnumerable<OpenApp> GetOpenApps()
        {
            return Process.GetProcesses()
                .Where(process => !string.IsNullOrWhiteSpace(process.MainWindowTitle))
                .Select(process => new OpenApp
                {
                    Title = process.MainWindowTitle,
                    ProcessName = process.ProcessName
                })
                .OrderBy(app => app.ProcessName)
                .ThenBy(app => app.Title)
                .ToList();
        }
    }
}
