using System.Collections.Generic;

namespace WindBar.Core
{
    /// <summary>
    /// Scans installed and portable applications on the system.
    /// This is a stub implementation. In a production environment it would query the Start menu,
    /// registry, and user folders to discover applications. The scanner assigns a confidence score
    /// to candidate apps so that high confidence apps can be automatically pinned.
    /// </summary>
    public class AppScanner
    {
        public class AppInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public double Confidence { get; set; }
        }

        public IEnumerable<AppInfo> ScanInstalledApps()
        {
            return new List<AppInfo>();
        }

        public IEnumerable<AppInfo> ScanPortableApps(IEnumerable<string> additionalFolders)
        {
            return new List<AppInfo>();
        }
    }
}
