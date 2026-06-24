namespace WindBar.Core
{
    public sealed class MediaSessionState
    {
        public string SourceName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public MediaPlaybackState PlaybackState { get; set; } = MediaPlaybackState.Unknown;
        public double? PositionSeconds { get; set; }
        public double? DurationSeconds { get; set; }
        public bool CanGoPrevious { get; set; }
        public bool CanPlayPause { get; set; }
        public bool CanGoNext { get; set; }
        public bool HasMedia => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Artist);
    }
}
