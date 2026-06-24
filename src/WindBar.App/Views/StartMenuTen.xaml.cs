using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindBar.Core;

namespace WindBar.App.Views
{
    public partial class StartMenuTen : UserControl, IStartMenuProvider
    {
        public StartMenuTen()
        {
            Content = new TextBlock
            {
                Text = "Windows 10 style Start, re-imagined for Windows 11",
                Foreground = Brushes.White,
                FontSize = 24,
                Margin = new Thickness(24)
            };
        }

        public string DisplayName => "Windows 10 Start";
        public object CreateView() => this;
    }
}
