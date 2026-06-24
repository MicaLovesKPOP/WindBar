using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WindBar.App.Services;
using WindBar.App.Views;
using WindBar.Core;

namespace WindBar.App
{
    public partial class MainWindow : Window
    {
        private readonly SettingsStore _settingsStore = new SettingsStore();
        private readonly PinnedAppService _pinnedAppService = new PinnedAppService();
        private readonly OpenAppService _openAppService = new OpenAppService();
        private readonly WindBarSettings _settings;
        private readonly Grid _root = new Grid();
        private readonly StackPanel _left = new StackPanel { Orientation = Orientation.Horizontal };
        private readonly StackPanel _center = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        private readonly StackPanel _right = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        private readonly Dictionary<string, IStartMenuProvider> _startProviders = new Dictionary<string, IStartMenuProvider>();
        private readonly DispatcherTimer _clock = new DispatcherTimer();
        private readonly DispatcherTimer _runningRefresh = new DispatcherTimer();
        private Button? _clockButton;
        private bool _isHidden;

        public MainWindow()
        {
            _settings = _settingsStore.Load();
            RegisterStartProviders();
            ConfigureWindow();
            BuildUi();
            ApplyTheme();
            ApplyPlacement();
            StartClock();
            StartRunningRefresh();
            Closing += (_, __) => _settingsStore.Save(_settings);
        }

        private void RegisterStartProviders()
        {
            _startProviders["start.win11"] = new StartMenu11();
            _startProviders["start.win10"] = new StartMenuTen();
            _startProviders["start.win81"] = new StartScreen81();
            _startProviders["start.classic"] = new StartMenuClassic();
        }

        private void ConfigureWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = _settings.BarThickness;
            Left = 0;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            ShowInTaskbar = false;
            Content = _root;
            MouseEnter += (_, __) => RevealFromAutoHide();
            MouseLeave += (_, __) => HideIfAutoHide();
        }

        private void BuildUi()
        {
            _root.ColumnDefinitions.Clear();
            _root.Children.Clear();
            _root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            _root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            _root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            _left.Children.Clear();
            _center.Children.Clear();
            _right.Children.Clear();

            Grid.SetColumn(_left, 0);
            Grid.SetColumn(_center, 1);
            Grid.SetColumn(_right, 2);
            _root.Children.Add(_left);
            _root.Children.Add(_center);
            _root.Children.Add(_right);

            if (_settings.ShowStartButton)
                _left.Children.Add(MakeButton("⊞", ShowStart, "Start"));
            if (_settings.ShowSearchButton)
                _left.Children.Add(MakeButton("Search", null, "Search"));
            if (_settings.ShowStartSwitcher)
            {
                _left.Children.Add(MakeButton("11", (_, __) => SelectStart("start.win11"), "Use Windows 11 Start"));
                _left.Children.Add(MakeButton("10", (_, __) => SelectStart("start.win10"), "Use modern Start"));
                _left.Children.Add(MakeButton("8.1", (_, __) => SelectStart("start.win81"), "Use full screen Start"));
            }

            BuildCenterZone();

            if (_settings.ShowSettingsButton)
                _right.Children.Add(MakeButton("Settings", OpenSettings, "WindBar settings"));
            if (_settings.ShowThemeButton)
                _right.Children.Add(MakeButton("Theme", CycleTheme, "Cycle theme"));
            if (_settings.ShowPlacementButton)
                _right.Children.Add(MakeButton("Top/Bottom", ToggleEdge, "Move taskbar"));
            if (_settings.ShowAutoHideButton)
                _right.Children.Add(MakeButton("AutoHide", ToggleAutoHide, "Toggle auto hide"));
            if (_settings.ShowClock)
            {
                _clockButton = MakeButton(DateTime.Now.ToString("HH:mm"), null, "Clock");
                _right.Children.Add(_clockButton);
            }
        }

        private void BuildCenterZone()
        {
            _center.Children.Clear();
            if (_settings.ShowPinnedApps)
            {
                foreach (var app in _pinnedAppService.LoadOrCreateDefaults())
                {
                    _center.Children.Add(MakeButton(app.Name, (_, __) => Launch(app.Path), app.Path));
                }
            }

            if (_settings.ShowOpenApps)
            {
                var added = 0;
                foreach (var app in _openAppService.GetOpenApps())
                {
                    if (added >= 6) break;
                    _center.Children.Add(MakeButton("● " + app.ProcessName, null, app.Title));
                    added++;
                }
            }

            if (_center.Children.Count == 0)
            {
                _center.Children.Add(MakeButton("Files", null, "Pinned app"));
                _center.Children.Add(MakeButton("Browser", null, "Pinned app"));
                _center.Children.Add(MakeButton("Code", null, "Pinned app"));
            }
        }

        private Button MakeButton(string text, RoutedEventHandler? action, string tooltip)
        {
            var button = new Button
            {
                Content = text,
                ToolTip = tooltip,
                Margin = new Thickness(4, 6, 4, 6),
                Padding = new Thickness(12, 0, 12, 0),
                MinWidth = 40,
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };
            if (action != null) button.Click += action;
            return button;
        }

        private static void Launch(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch
            {
            }
        }

        private void StartClock()
        {
            _clock.Interval = TimeSpan.FromSeconds(1);
            _clock.Tick += (_, __) =>
            {
                if (_clockButton != null) _clockButton.Content = DateTime.Now.ToString("HH:mm");
            };
            _clock.Start();
        }

        private void StartRunningRefresh()
        {
            _runningRefresh.Interval = TimeSpan.FromSeconds(10);
            _runningRefresh.Tick += (_, __) =>
            {
                BuildCenterZone();
                ApplyButtonTheme(_center, _settings.Theme == BarTheme.Light ? Brushes.Black : Brushes.White);
            };
            _runningRefresh.Start();
        }

        private void SelectStart(string id)
        {
            _settings.StartProviderId = id;
            _settingsStore.Save(_settings);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow(_settings, _settingsStore, ApplySettingsFromWindow);
            settings.Owner = this;
            settings.Show();
        }

        private void ApplySettingsFromWindow()
        {
            BuildUi();
            ApplyTheme();
            ApplyPlacement();
        }

        private void ShowStart(object sender, RoutedEventArgs e)
        {
            if (!_startProviders.TryGetValue(_settings.StartProviderId, out var provider))
                provider = _startProviders["start.win11"];

            var content = provider.CreateView() as UIElement ?? new TextBlock { Text = provider.DisplayName };
            var start = new Window
            {
                Width = _settings.StartProviderId == "start.win81" ? SystemParameters.PrimaryScreenWidth : 720,
                Height = _settings.StartProviderId == "start.win81" ? SystemParameters.PrimaryScreenHeight - Height : 640,
                Left = _settings.StartProviderId == "start.win81" ? 0 : 16,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Topmost = true,
                ShowInTaskbar = false,
                Content = content,
                Background = Background
            };
            start.Top = _settings.Edge == BarEdge.Bottom ? SystemParameters.PrimaryScreenHeight - Height - start.Height - 12 : Height + 12;
            start.Deactivated += (_, __) => start.Close();
            start.Show();
        }

        private void ToggleEdge(object sender, RoutedEventArgs e)
        {
            _settings.Edge = _settings.Edge == BarEdge.Bottom ? BarEdge.Top : BarEdge.Bottom;
            ApplyPlacement();
            _settingsStore.Save(_settings);
        }

        private void ToggleAutoHide(object sender, RoutedEventArgs e)
        {
            _settings.AutoHide = !_settings.AutoHide;
            if (!_settings.AutoHide) RevealFromAutoHide();
            else HideIfAutoHide();
            _settingsStore.Save(_settings);
        }

        private void CycleTheme(object sender, RoutedEventArgs e)
        {
            _settings.Theme = _settings.Theme switch
            {
                BarTheme.Dark => BarTheme.Oled,
                BarTheme.Oled => BarTheme.Transparent,
                BarTheme.Transparent => BarTheme.Light,
                _ => BarTheme.Dark
            };
            ApplyTheme();
            _settingsStore.Save(_settings);
        }

        private void ApplyPlacement()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = _settings.BarThickness;
            Left = 0;
            Top = _settings.Edge == BarEdge.Bottom ? SystemParameters.PrimaryScreenHeight - Height : 0;
            _isHidden = false;
        }

        private void ApplyTheme()
        {
            var background = _settings.Theme switch
            {
                BarTheme.Light => Color.FromArgb(238, 245, 245, 245),
                BarTheme.Oled => Color.FromArgb(255, 0, 0, 0),
                BarTheme.Transparent => Color.FromArgb(80, 32, 32, 32),
                _ => Color.FromArgb(238, 32, 32, 32)
            };
            var foreground = _settings.Theme == BarTheme.Light ? Brushes.Black : Brushes.White;
            Background = new SolidColorBrush(background);
            ApplyButtonTheme(_left, foreground);
            ApplyButtonTheme(_center, foreground);
            ApplyButtonTheme(_right, foreground);
        }

        private void ApplyButtonTheme(Panel panel, Brush foreground)
        {
            foreach (var child in panel.Children)
            {
                if (child is Button button)
                {
                    button.Foreground = foreground;
                    button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    button.BorderBrush = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255));
                }
            }
        }

        private void HideIfAutoHide()
        {
            if (!_settings.AutoHide || _isHidden) return;
            Top = _settings.Edge == BarEdge.Bottom ? SystemParameters.PrimaryScreenHeight - 2 : -Height + 2;
            _isHidden = true;
        }

        private void RevealFromAutoHide()
        {
            if (!_settings.AutoHide || !_isHidden) return;
            Top = _settings.Edge == BarEdge.Bottom ? SystemParameters.PrimaryScreenHeight - Height : 0;
            _isHidden = false;
        }
    }
}
