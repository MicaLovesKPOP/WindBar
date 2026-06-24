using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindBar.Core;

namespace WindBar.App.Views
{
    public partial class StartMenu11 : UserControl, IStartMenuProvider
    {
        public StartMenu11()
        {
            Content = new TextBlock
            {
                Text = "Windows 11 style Start",
                Foreground = Brushes.White,
                FontSize = 24,
                Margin = new Thickness(24)
            };
        }

        public string DisplayName => "Windows 11 Start";
        public object CreateView() => this;
    }
}
