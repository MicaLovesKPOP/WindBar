using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WindBar.Core;

namespace WindBar.App.Views
{
    public sealed class MediaMiniPlayerModule : UserControl
    {
        private readonly IMediaProvider _provider;
        private readonly BarEdge _edge;
        private readonly Border _shell = new Border();
        private readonly Border _badge = new Border();
        private readonly TextBlock _badgeText = new TextBlock();
        private readonly TextBlock _title = new TextBlock();
        private readonly TextBlock _subtitle = new TextBlock();
        private readonly Button _playPause = new Button();
        private readonly Popup _slideOut = new Popup();
        private readonly Border _slideSurface = new Border();
        private readonly TranslateTransform _slideTransform = new TranslateTransform();
        private readonly TextBlock _slideTitle = new TextBlock();
        private readonly TextBlock _slideSubtitle = new TextBlock();
        private readonly TextBlock _slideSource = new TextBlock();
        private readonly TextBlock _slideProgress = new TextBlock();
        private readonly Button _slidePlayPause = new Button();
        private readonly List<Button> _controls = new List<Button>();

        public MediaMiniPlayerModule(IMediaProvider provider, BarTheme theme, BarEdge edge)
        {
            _provider = provider;
            _edge = edge;
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

            var shellBackground = theme switch
            {
                BarTheme.Light => Color.FromArgb(215, 255, 255, 255),
                BarTheme.Oled => Color.FromArgb(255, 0, 0, 0),
                BarTheme.Transparent => Color.FromArgb(95, 24, 24, 24),
                _ => Color.FromArgb(90, 255, 255, 255)
            };
            var border = light
                ? Color.FromArgb(80, 0, 0, 0)
                : Color.FromArgb(oled ? (byte)55 : (byte)35, 255, 255, 255);

            _shell.Background = new SolidColorBrush(shellBackground);
            _shell.BorderBrush = new SolidColorBrush(border);
            _slideSurface.Background = new SolidColorBrush(theme switch
            {
                BarTheme.Light => Color.FromArgb(245, 255, 255, 255),
                BarTheme.Oled => Color.FromArgb(250, 0, 0, 0),
                BarTheme.Transparent => Color.FromArgb(215, 24, 24, 24),
                _ => Color.FromArgb(245, 32, 32, 32)
            });
            _slideSurface.BorderBrush = new SolidColorBrush(border);

            var primary = light ? Brushes.Black : Brushes.White;
            var secondary = light
                ? new SolidColorBrush(Color.FromArgb(210, 40, 40, 40))
                : new SolidColorBrush(Color.FromArgb(210, 220, 220, 220));

            _title.Foreground = primary;
            _subtitle.Foreground = secondary;
            _slideTitle.Foreground = primary;
            _slideSubtitle.Foreground = secondary;
            _slideSource.Foreground = secondary;
            _slideProgress.Foreground = secondary;

            _badge.Background = new SolidColorBrush(transparent
                ? Color.FromArgb(210, 200, 40, 40)
                : Color.FromArgb(180, 200, 40, 40));
            _badgeText.Foreground = Brushes.White;

            foreach (var button in _controls)
            {
                button.Foreground = primary;
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
            _shell.Cursor = Cursors.Hand;
            _shell.MouseLeftButtonUp += ToggleSlideOut;

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
            BuildSlideOut();
            return _shell;
        }

        private void BuildSlideOut()
        {
            _slideSurface.Width = 360;
            _slideSurface.CornerRadius = new CornerRadius(16);
            _slideSurface.Padding = new Thickness(16);
            _slideSurface.BorderThickness = new Thickness(1);
            _slideSurface.RenderTransform = _slideTransform;

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = "Now playing",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromArgb(210, 160, 160, 160)),
                Margin = new Thickness(0, 0, 0, 8)
            });

            _slideTitle.FontSize = 20;
            _slideTitle.FontWeight = FontWeights.SemiBold;
            _slideTitle.TextTrimming = TextTrimming.CharacterEllipsis;
            stack.Children.Add(_slideTitle);

            _slideSubtitle.FontSize = 13;
            _slideSubtitle.TextTrimming = TextTrimming.CharacterEllipsis;
            _slideSubtitle.Margin = new Thickness(0, 2, 0, 0);
            stack.Children.Add(_slideSubtitle);

            _slideSource.FontSize = 11;
            _slideSource.TextTrimming = TextTrimming.CharacterEllipsis;
            _slideSource.Margin = new Thickness(0, 10, 0, 0);
            stack.Children.Add(_slideSource);

            _slideProgress.FontSize = 11;
            _slideProgress.Margin = new Thickness(0, 8, 0, 12);
            stack.Children.Add(_slideProgress);

            var controls = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            controls.Children.Add(MakeControl("⏮", (_, __) => _provider.Previous()));
            _slidePlayPause.Margin = new Thickness(6, 0, 0, 0);
            _slidePlayPause.Padding = new Thickness(12, 3, 12, 3);
            _slidePlayPause.Click += (_, __) => _provider.PlayPause();
            _controls.Add(_slidePlayPause);
            controls.Children.Add(_slidePlayPause);
            controls.Children.Add(MakeControl("⏭", (_, __) => _provider.Next()));
            stack.Children.Add(controls);

            _slideSurface.Child = stack;
            _slideOut.AllowsTransparency = true;
            _slideOut.StaysOpen = false;
            _slideOut.PopupAnimation = PopupAnimation.Fade;
            _slideOut.PlacementTarget = _shell;
            _slideOut.Placement = _edge == BarEdge.Top ? PlacementMode.Bottom : PlacementMode.Top;
            _slideOut.Child = _slideSurface;
        }

        private Button MakeControl(string text, RoutedEventHandler action)
        {
            var button = new Button
            {
                Content = text,
                Margin = new Thickness(4, 0, 0, 0),
                Padding = new Thickness(6, 0, 6, 0),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };
            button.Click += action;
            _controls.Add(button);
            return button;
        }

        private void ToggleSlideOut(object sender, MouseButtonEventArgs e)
        {
            if (IsInsideButton(e.OriginalSource))
                return;

            if (_slideOut.IsOpen)
            {
                _slideOut.IsOpen = false;
            }
            else
            {
                _slideOut.IsOpen = true;
                AnimateSlideOut();
            }

            e.Handled = true;
        }

        private void AnimateSlideOut()
        {
            _slideTransform.Y = _edge == BarEdge.Top ? -18 : 18;
            var animation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            _slideTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private static bool IsInsideButton(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is Button)
                    return true;
                current = VisualTreeHelper.GetParent(current);
            }
            return false;
        }

        private void Refresh()
        {
            var state = _provider.Current;
            _title.Text = state.HasMedia ? state.Title : "Nothing playing";
            _subtitle.Text = state.HasMedia ? $"{state.Artist} • {state.SourceName}" : state.SourceName;
            _slideTitle.Text = state.HasMedia ? state.Title : "Nothing playing";
            _slideSubtitle.Text = state.HasMedia ? state.Artist : "No active media session";
            _slideSource.Text = state.SourceName;
            _slideProgress.Text = FormatProgress(state);
            var playPauseText = state.PlaybackState == MediaPlaybackState.Playing ? "⏸" : "▶";
            _playPause.Content = playPauseText;
            _slidePlayPause.Content = playPauseText;
            ToolTip = $"{state.Title}\n{state.Artist}\n{state.SourceName}";
        }

        private static string FormatProgress(MediaSessionState state)
        {
            if (state.PositionSeconds == null || state.DurationSeconds == null || state.DurationSeconds <= 0)
                return state.PlaybackState.ToString();

            return $"{FormatTime(state.PositionSeconds.Value)} / {FormatTime(state.DurationSeconds.Value)}";
        }

        private static string FormatTime(double seconds)
        {
            var time = TimeSpan.FromSeconds(Math.Max(0, seconds));
            return time.TotalHours >= 1
                ? time.ToString(@"h\:mm\:ss")
                : time.ToString(@"m\:ss");
        }
    }
}
