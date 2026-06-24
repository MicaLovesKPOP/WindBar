namespace WindBar.Core
{
    public enum BarEdge
    {
        Bottom,
        Top,
        Left,
        Right
    }

    public enum BarTheme
    {
        System,
        Light,
        Dark,
        Oled,
        Transparent
    }

    public enum ModuleZone
    {
        Left,
        Center,
        Right
    }

    public sealed class WindBarSettings
    {
        public BarEdge Edge { get; set; } = BarEdge.Bottom;
        public BarTheme Theme { get; set; } = BarTheme.Dark;
        public bool AutoHide { get; set; }
        public int BarThickness { get; set; } = 48;
        public string StartProviderId { get; set; } = "start.win11";
        public bool ShowStartButton { get; set; } = true;
        public bool ShowSearchButton { get; set; } = true;
        public bool ShowStartSwitcher { get; set; } = true;
        public bool ShowPinnedApps { get; set; } = true;
        public bool ShowOpenApps { get; set; } = true;
        public bool ShowThemeButton { get; set; } = true;
        public bool ShowPlacementButton { get; set; } = true;
        public bool ShowAutoHideButton { get; set; } = true;
        public bool ShowClock { get; set; } = true;
        public bool ShowSettingsButton { get; set; } = true;
    }
}
