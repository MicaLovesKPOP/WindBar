using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindBar.Core;

namespace WindBar.App.Views
{
    public sealed class StartScreen81 : UserControl, IStartMenuProvider
    {
        private readonly AppScanner _scanner = new AppScanner();

        public StartScreen81()
        {
            Content = CreateView();
        }

        public string DisplayName => "Full screen Start";

        public object CreateView()
        {
            var panel = new WrapPanel { Margin = new Thickness(32) };
            foreach (var app in _scanner.ScanSmartDefaults().Take(80))
            {
                panel.Children.Add(Tile(app.Name));
            }
            if (panel.Children.Count == 0)
            {
                panel.Children.Add(Tile("Desktop"));
                panel.Children.Add(Tile("Files"));
                panel.Children.Add(Tile("Browser"));
                panel.Children.Add(Tile("Settings"));
            }
            return panel;
        }

        private Border Tile(string text)
        {
            return new Border
            {
                Width = 160,
                Height = 100,
                Margin = new Thickness(8),
                CornerRadius = new CornerRadius(12),
                Background = new SolidColorBrush(Color.FromArgb(220, 0, 120, 215)),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = Brushes.White,
                    FontSize = 18,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(12)
                }
            };
        }
    }
}
