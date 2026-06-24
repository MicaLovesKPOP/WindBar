using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindBar.Core
{
    public class AppScanner
    {
        public class AppInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public double Confidence { get; set; }
            public string Source { get; set; } = string.Empty;
            public string Group { get; set; } = "Apps";
        }

        public IEnumerable<AppInfo> ScanInstalledApps()
        {
            var roots = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
            };

            foreach (var root in roots.Where(Directory.Exists))
            {
                foreach (var file in Directory.EnumerateFiles(root, "*.lnk", SearchOption.AllDirectories).Take(200))
                {
                    yield return new AppInfo
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Path = file,
                        Confidence = 0.95,
                        Source = "Shortcut",
                        Group = GuessGroup(file)
                    };
                }
            }
        }

        public IEnumerable<AppInfo> ScanPortableApps(IEnumerable<string> additionalFolders)
        {
            foreach (var folder in additionalFolders.Where(Directory.Exists))
            {
                foreach (var file in Directory.EnumerateFiles(folder, "*.exe", SearchOption.AllDirectories).Take(200))
                {
                    yield return new AppInfo
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Path = file,
                        Confidence = ScorePortableCandidate(file),
                        Source = "Portable",
                        Group = GuessGroup(file)
                    };
                }
            }
        }

        public IEnumerable<AppInfo> ScanSmartDefaults(IEnumerable<string>? portableFolders = null)
        {
            var list = ScanInstalledApps().ToList();
            if (portableFolders != null)
            {
                list.AddRange(ScanPortableApps(portableFolders).Where(x => x.Confidence >= 0.7));
            }
            return list.GroupBy(x => x.Path, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).OrderByDescending(x => x.Confidence).ThenBy(x => x.Name);
        }

        private static double ScorePortableCandidate(string path)
        {
            var score = 0.55;
            var name = Path.GetFileNameWithoutExtension(path);
            var folder = Path.GetFileName(Path.GetDirectoryName(path) ?? string.Empty);
            if (string.Equals(name, folder, StringComparison.OrdinalIgnoreCase)) score += 0.25;
            if (path.Contains("Portable", StringComparison.OrdinalIgnoreCase)) score += 0.1;
            return Math.Min(score, 0.98);
        }

        private static string GuessGroup(string path)
        {
            var lower = path.ToLowerInvariant();
            if (lower.Contains("steam") || lower.Contains("game")) return "Games";
            if (lower.Contains("studio") || lower.Contains("code") || lower.Contains("git")) return "Development";
            if (lower.Contains("music") || lower.Contains("audio")) return "Music";
            if (lower.Contains("photo") || lower.Contains("paint") || lower.Contains("adobe")) return "Creative";
            if (lower.Contains("discord") || lower.Contains("teams")) return "Communication";
            return "Apps";
        }
    }
}
