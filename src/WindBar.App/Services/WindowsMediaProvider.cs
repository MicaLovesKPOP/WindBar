using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WindBar.Core;
using Windows.Media.Control;

namespace WindBar.App.Services
{
    public sealed class WindowsMediaProvider : IMediaProvider, IDisposable
    {
        private readonly Timer _pollTimer;
        private GlobalSystemMediaTransportControlsSessionManager? _manager;
        private MediaSessionState _current = new MediaSessionState
        {
            SourceName = "Windows media session",
            Title = "Nothing playing",
            Artist = string.Empty,
            PlaybackState = MediaPlaybackState.Stopped,
            CanPlayPause = false,
            CanGoPrevious = false,
            CanGoNext = false
        };

        public string Id => "media.windows";
        public string DisplayName => "Windows media provider";
        public MediaSessionState Current => _current;
        public bool IsAvailable { get; private set; }

        public event EventHandler? Changed;

        public WindowsMediaProvider()
        {
            _pollTimer = new Timer(1500);
            _pollTimer.Elapsed += async (_, __) => await RefreshAsync();
            _pollTimer.AutoReset = true;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                IsAvailable = _manager != null;
                await RefreshAsync();
                if (IsAvailable)
                    _pollTimer.Start();
            }
            catch
            {
                IsAvailable = false;
                _current = new MediaSessionState
                {
                    SourceName = "Windows media session",
                    Title = "Unavailable",
                    Artist = "WindBar",
                    PlaybackState = MediaPlaybackState.Unknown,
                    CanPlayPause = false,
                    CanGoPrevious = false,
                    CanGoNext = false
                };
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void PlayPause()
        {
            try
            {
                var session = GetPreferredSession();
                if (session != null)
                {
                    await session.TryTogglePlayPauseAsync();
                    await RefreshAsync();
                }
            }
            catch
            {
            }
        }

        public async void Previous()
        {
            try
            {
                var session = GetPreferredSession();
                if (session != null)
                {
                    await session.TrySkipPreviousAsync();
                    await RefreshAsync();
                }
            }
            catch
            {
            }
        }

        public async void Next()
        {
            try
            {
                var session = GetPreferredSession();
                if (session != null)
                {
                    await session.TrySkipNextAsync();
                    await RefreshAsync();
                }
            }
            catch
            {
            }
        }

        private async Task RefreshAsync()
        {
            try
            {
                var session = GetPreferredSession();
                if (session == null)
                {
                    _current = new MediaSessionState
                    {
                        SourceName = "Universal media",
                        Title = "Nothing playing",
                        Artist = string.Empty,
                        PlaybackState = MediaPlaybackState.Stopped,
                        CanPlayPause = false,
                        CanGoPrevious = false,
                        CanGoNext = false
                    };
                    Changed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var media = await session.TryGetMediaPropertiesAsync();
                var playback = session.GetPlaybackInfo();
                var timeline = session.GetTimelineProperties();

                _current = new MediaSessionState
                {
                    SourceName = session.SourceAppUserModelId ?? "Unknown source",
                    Title = media?.Title ?? "Unknown title",
                    Artist = media?.Artist ?? string.Empty,
                    Album = media?.AlbumTitle ?? string.Empty,
                    PlaybackState = MapPlaybackState(playback?.PlaybackStatus),
                    PositionSeconds = timeline != null ? timeline.Position.TotalSeconds : null,
                    DurationSeconds = timeline != null ? timeline.EndTime.TotalSeconds : null,
                    CanGoPrevious = playback?.Controls?.IsPreviousEnabled ?? false,
                    CanPlayPause = (playback?.Controls?.IsPauseEnabled ?? false) || (playback?.Controls?.IsPlayEnabled ?? false),
                    CanGoNext = playback?.Controls?.IsNextEnabled ?? false
                };

                Changed?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
            }
        }

        private GlobalSystemMediaTransportControlsSession? GetPreferredSession()
        {
            if (_manager == null)
                return null;

            var sessions = _manager.GetSessions();
            if (sessions == null || sessions.Count == 0)
                return null;

            var playing = sessions
                .FirstOrDefault(s =>
                {
                    try
                    {
                        return s.GetPlaybackInfo()?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
                    }
                    catch
                    {
                        return false;
                    }
                });

            if (playing != null)
                return playing;

            return _manager.GetCurrentSession() ?? sessions.FirstOrDefault();
        }

        private static MediaPlaybackState MapPlaybackState(GlobalSystemMediaTransportControlsSessionPlaybackStatus? status)
        {
            return status switch
            {
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing => MediaPlaybackState.Playing,
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused => MediaPlaybackState.Paused,
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped => MediaPlaybackState.Stopped,
                _ => MediaPlaybackState.Unknown
            };
        }

        public void Dispose()
        {
            _pollTimer.Stop();
            _pollTimer.Dispose();
        }
    }
}
