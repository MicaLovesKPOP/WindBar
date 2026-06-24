using System.Collections.Generic;
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
        private readonly Border _badge = new Border();
        private readonly TextBlock _badgeText = new TextBlock();
        private readonly TextBlock _title = new TextBlock();
        private readonly TextBlock _subtitle = new TextBlock();
        private readonly Button _playPause = new Button();
        private readonly List<Button> _controls = new List<Button>();

        public MediaMiniPlayerModule(IMediaProvider provider, BarTheme theme)
        {
            _provider = provider;
            _provider.Changed += (_, __) => Refresh();
            Content = Build();
            ApplyTheme(theme);
            Refresh();
        }

        public void ApplyTheme(BarTheme theme)
        {
            var light = theme == BarTheme.Light;
            var oled = theme == BarTheme.Oled;
            var transparent = theme == BarTheme.Transparent;

            _shell.Background = new SolidColorBrush(theme switch
            {
                BarTheme.Light => Color.FromArgb(215, 255, 255, 255),
                BarTheme.Oled => Color.FromArgb(255, 0, 0, 0),
                BarTheme.Transparent => Color.FromArgb(95, 24, 24, 24),
                _ => Color.FromArgb(90, 255, 255, 255)
            });
            _shell.BorderBrush = new SolidColorBrush(light
                ? Color.FromArgb(80, 0, 0, 0)
                : Color.FromArgb(oled ? (byte)55 : (byte)35, 255, 255, 255));

            _title.Foreground = light ? Brushes.Black : Brushes.White;
            _subtitle.Foreground = light
                ? new SolidColorBrush(Color.FromArgb(210, 40, 40, 40))
                : new SolidColorBrush(Color.FromArgb(210, 220, 220, 220));

            _badge.Background = new SolidColorBrush(transparent
                ? Color.FromArgb(210, 200, 40, 40)
                : Color.FromArgb(180, 200, 40, 40));
            _badgeText.Foreground = Brushes.White;

            foreach (var button in _controls)
            {
                button.Foreground = light ? Brushes.Black : Brushes.White;
                button.Background = Brushes.Transparent;
                button.BorderBrush = new SolidColorBrush(light
                    ? Color.FromArgb(65, 0, 0, 0)
                    : Color.FromArgb(35, 255, 255, 255));
            }
        }

        private UIElement Build()
        {
            _shell.CornerRadius = new CornerRadius(10);
            _shell.Margin = new Thickness(4, 6, 4, 6);
            _shell.Padding = new Thickness(8, 4, 8, 4);
            _shell.BorderThickness = new Thickness(1);

            var row = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            _badge.Width = 22;
            _badge.Height = 22;
            _badge.CornerRadius = new CornerRadius(6);
            _badge.Margin = new Thickness(0, 0, 8, 0);
            _badgeText.Text = "♪";
            _badgeText.HorizontalAlignment = HorizontalAlignment.Center;
            _badgeText.VerticalAlignment = VerticalAlignment.Center;
            _badgeText.FontSize = 13;
            _badge.Child = _badgeText;
            row.Children.Add(_badge);

            var textStack = new StackPanel { Width = 150, VerticalAlignment = VerticalAlignment.Center };
            _title.FontSize = 12;
            _title.FontWeight = FontWeights.SemiBold;
            _title.TextTrimming = TextTrimming.CharacterEllipsis;
            _subtitle.FontSize = 10;
            _subtitle.TextTrimming = TextTrimming.CharacterEllipsis;
            textStack.Children.Add(_title);
            textStack.Children.Add(_subtitle);
            row.Children.Add(textStack);

            var previous = MakeControl("⏮", (_, __) => _provider.Previous());
            _playPause.Click += (_, __) => _provider.PlayPause();
            _playPause.Margin = new Thickness(4, 0, 0, 0);
            _playPause.Padding = new Thickness(6, 0, 6, 0);
            _controls.Add(_playPause);
            var next = MakeControl("⏭", (_, __) => _provider.Next());
            row.Children.Add(previous);
            row.Children.Add(_playPause);
            row.Children.Add(next);

            _shell.Child = row;
            return _shell;
        }

        private Button MakeControl(string text, RoutedEventHandler action)
        {
            var button = new Button
            {
                Content = text,
                Margin = new Thickness(4, 0, 0, 0),
                Padding = new Thickness(6, 0, 6, 0),
                Background = Brushes.Transparent
            };
            button.Click += action;
            _controls.Add(button);
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
