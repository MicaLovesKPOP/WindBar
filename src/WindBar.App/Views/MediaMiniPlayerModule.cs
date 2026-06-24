using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindBar.Core;

namespace WindBar.App.Views
{
    public sealed class MediaMiniPlayerModule : UserControl
    {
        private readonly IMediaProvider _provider;
        private readonly Border _shell = new Border();
        private readonly TextBlock _title = new TextBlock();
        private readonly TextBlock _subtitle = new TextBlock();
        private readonly Button _playPause = new Button();

        public MediaMiniPlayerModule(IMediaProvider provider)
        {
            _provider = provider;
            _provider.Changed += (_, __) => Refresh();
            Content = Build();
            Refresh();
        }

        private UIElement Build()
        {
            _shell.CornerRadius = new CornerRadius(10);
            _shell.Margin = new Thickness(4, 6, 4, 6);
            _shell.Padding = new Thickness(8, 4, 8, 4);
            _shell.BorderBrush = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255));
            _shell.BorderThickness = new Thickness(1);
            _shell.Background = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));

            var row = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            var badge = new Border
            {
                Width = 22,
                Height = 22,
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(0, 0, 8, 0),
                Background = new SolidColorBrush(Color.FromArgb(180, 200, 40, 40)),
                Child = new TextBlock
                {
                    Text = "♪",
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 13
                }
            };
            row.Children.Add(badge);

            var textStack = new StackPanel { Width = 150, VerticalAlignment = VerticalAlignment.Center };
            _title.Foreground = Brushes.White;
            _title.FontSize = 12;
            _title.FontWeight = FontWeights.SemiBold;
            _title.TextTrimming = TextTrimming.CharacterEllipsis;
            _subtitle.Foreground = Brushes.LightGray;
            _subtitle.FontSize = 10;
            _subtitle.TextTrimming = TextTrimming.CharacterEllipsis;
            textStack.Children.Add(_title);
            textStack.Children.Add(_subtitle);
            row.Children.Add(textStack);

            var previous = MakeControl("⏮", (_, __) => _provider.Previous());
            _playPause.Click += (_, __) => _provider.PlayPause();
            _playPause.Margin = new Thickness(4, 0, 0, 0);
            _playPause.Padding = new Thickness(6, 0, 6, 0);
            var next = MakeControl("⏭", (_, __) => _provider.Next());
            row.Children.Add(previous);
            row.Children.Add(_playPause);
            row.Children.Add(next);

            _shell.Child = row;
            return _shell;
        }

        private static Button MakeControl(string text, RoutedEventHandler action)
        {
            var button = new Button
            {
                Content = text,
                Margin = new Thickness(4, 0, 0, 0),
                Padding = new Thickness(6, 0, 6, 0),
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255))
            };
            button.Click += action;
            return button;
        }

        private void Refresh()
        {
            var state = _provider.Current;
            _title.Text = state.HasMedia ? state.Title : "Nothing playing";
            _subtitle.Text = state.HasMedia ? $"{state.Artist} • {state.SourceName}" : state.SourceName;
            _playPause.Content = state.PlaybackState == MediaPlaybackState.Playing ? "⏸" : "▶";
            ToolTip = $"{state.Title}\n{state.Artist}\n{state.SourceName}";
        }
    }
}
