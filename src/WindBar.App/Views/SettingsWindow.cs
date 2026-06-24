using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindBar.App.Services;
using WindBar.Core;

namespace WindBar.App.Views
{
    public sealed class SettingsWindow : Window
    {
        private readonly WindBarSettings _settings;
        private readonly SettingsStore _store;
        private readonly Action _onSaved;

        public SettingsWindow(WindBarSettings settings, SettingsStore store, Action onSaved)
        {
            _settings = settings;
            _store = store;
            _onSaved = onSaved;

            Title = "WindBar Settings";
            Width = 460;
            Height = 660;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
            Foreground = Brushes.White;
            Content = BuildContent();
        }

        private UIElement BuildContent()
        {
            var root = new StackPanel { Margin = new Thickness(24) };
            root.Children.Add(new TextBlock
            {
                Text = "WindBar Settings",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 16)
            });

            root.Children.Add(MakeCheckBox("Start button", _settings.ShowStartButton, value => _settings.ShowStartButton = value));
            root.Children.Add(MakeCheckBox("Search", _settings.ShowSearchButton, value => _settings.ShowSearchButton = value));
            root.Children.Add(MakeCheckBox("Start switcher", _settings.ShowStartSwitcher, value => _settings.ShowStartSwitcher = value));
            root.Children.Add(MakeCheckBox("Pinned apps", _settings.ShowPinnedApps, value => _settings.ShowPinnedApps = value));
            root.Children.Add(MakeCheckBox("Running apps", _settings.ShowOpenApps, value => _settings.ShowOpenApps = value));
            root.Children.Add(MakeCheckBox("Settings button", _settings.ShowSettingsButton, value => _settings.ShowSettingsButton = value));
            root.Children.Add(MakeCheckBox("Theme button", _settings.ShowThemeButton, value => _settings.ShowThemeButton = value));
            root.Children.Add(MakeCheckBox("Placement button", _settings.ShowPlacementButton, value => _settings.ShowPlacementButton = value));
            root.Children.Add(MakeCheckBox("Auto-hide button", _settings.ShowAutoHideButton, value => _settings.ShowAutoHideButton = value));
            root.Children.Add(MakeCheckBox("Clock", _settings.ShowClock, value => _settings.ShowClock = value));
            root.Children.Add(MakeCheckBox("Auto-hide taskbar", _settings.AutoHide, value => _settings.AutoHide = value));

            root.Children.Add(MakeSection("Theme"));
            root.Children.Add(MakeCombo(Enum.GetNames(typeof(BarTheme)), _settings.Theme.ToString(), value => _settings.Theme = Enum.Parse<BarTheme>(value)));

            root.Children.Add(MakeSection("Placement"));
            root.Children.Add(MakeCombo(new[] { "Bottom", "Top" }, _settings.Edge.ToString(), value => _settings.Edge = Enum.Parse<BarEdge>(value)));

            root.Children.Add(MakeSection("Start menu"));
            root.Children.Add(MakeCombo(new[] { "start.win11", "start.win10", "start.win81", "start.classic" }, _settings.StartProviderId, value => _settings.StartProviderId = value));

            var save = new Button
            {
                Content = "Save and apply",
                Margin = new Thickness(0, 24, 0, 0),
                Padding = new Thickness(12),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            save.Click += (_, __) =>
            {
                _store.Save(_settings);
                _onSaved();
                Close();
            };
            root.Children.Add(save);

            return new ScrollViewer { Content = root };
        }

        private static TextBlock MakeSection(string title)
        {
            return new TextBlock
            {
                Text = title,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 18, 0, 8)
            };
        }

        private static CheckBox MakeCheckBox(string text, bool initial, Action<bool> update)
        {
            var check = new CheckBox
            {
                Content = text,
                IsChecked = initial,
                Margin = new Thickness(0, 6, 0, 6),
                Foreground = Brushes.White
            };
            check.Checked += (_, __) => update(true);
            check.Unchecked += (_, __) => update(false);
            return check;
        }

        private static ComboBox MakeCombo(string[] values, string selected, Action<string> update)
        {
            var combo = new ComboBox
            {
                ItemsSource = values,
                SelectedItem = selected,
                Margin = new Thickness(0, 0, 0, 6)
            };
            combo.SelectionChanged += (_, __) =>
            {
                if (combo.SelectedItem is string value) update(value);
            };
            return combo;
        }
    }
}
