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

            foreach (var root in roots.Where(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)))
            {
                foreach (var file in EnumerateFilesSafe(root, "*.lnk", 250))
                {
                    yield return new AppInfo
                    {
                        Name = CleanName(Path.GetFileNameWithoutExtension(file)),
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
            foreach (var folder in additionalFolders.Where(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)))
            {
                foreach (var file in EnumerateFilesSafe(folder, "*.exe", 250))
                {
                    yield return new AppInfo
                    {
                        Name = CleanName(Path.GetFileNameWithoutExtension(file)),
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

            return list
                .Where(app => !string.IsNullOrWhiteSpace(app.Path))
                .GroupBy(app => CreateDeduplicationKey(app), StringComparer.OrdinalIgnoreCase)
                .Select(group => group.OrderByDescending(app => app.Confidence).ThenBy(app => app.Name).First())
                .OrderByDescending(app => app.Confidence)
                .ThenBy(app => app.Group)
                .ThenBy(app => app.Name)
                .ToList();
        }

        private static IEnumerable<string> EnumerateFilesSafe(string root, string pattern, int maxResults)
        {
            var results = new List<string>();
            var pending = new Queue<string>();
            pending.Enqueue(root);

            while (pending.Count > 0 && results.Count < maxResults)
            {
                var current = pending.Dequeue();

                try
                {
                    foreach (var file in Directory.EnumerateFiles(current, pattern))
                    {
                        results.Add(file);
                        if (results.Count >= maxResults)
                            break;
                    }
                }
                catch
                {
                }

                if (results.Count >= maxResults)
                    break;

                try
                {
                    foreach (var directory in Directory.EnumerateDirectories(current))
                    {
                        pending.Enqueue(directory);
                    }
                }
                catch
                {
                }
            }

            return results;
        }

        private static string CreateDeduplicationKey(AppInfo app)
        {
            var name = CleanName(app.Name);
            if (!string.IsNullOrWhiteSpace(name))
                return app.Source + ":" + name;

            return app.Path;
        }

        private static string CleanName(string name)
        {
            return (name ?? string.Empty)
                .Replace(".lnk", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" - Shortcut", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" Shortcut", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
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
            if (lower.Contains("steam") || lower.Contains("epic games") || lower.Contains("gog") || lower.Contains("game")) return "Games";
            if (lower.Contains("visual studio") || lower.Contains("studio") || lower.Contains("code") || lower.Contains("git") || lower.Contains("jetbrains")) return "Development";
            if (lower.Contains("music") || lower.Contains("audio") || lower.Contains("spotify") || lower.Contains("foobar") || lower.Contains("winamp")) return "Music";
            if (lower.Contains("photo") || lower.Contains("paint") || lower.Contains("adobe") || lower.Contains("affinity") || lower.Contains("blender")) return "Creative";
            if (lower.Contains("discord") || lower.Contains("teams") || lower.Contains("slack") || lower.Contains("telegram") || lower.Contains("whatsapp")) return "Communication";
            if (lower.Contains("edge") || lower.Contains("chrome") || lower.Contains("firefox") || lower.Contains("browser")) return "Browsers";
            return "Apps";
        }
    }
}
