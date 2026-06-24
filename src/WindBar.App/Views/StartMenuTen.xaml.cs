using WindBar.Core;

namespace WindBar.App.Views
{
    public partial class StartMenuTen : System.Windows.Controls.UserControl, IStartMenuProvider
    {
        public StartMenuTen()
        {
            InitializeComponent();
        }

        public string DisplayName => "Windows 10 Start";

        public object CreateView()
        {
            return this;
        }
    }
}
