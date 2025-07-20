using System.Windows;

namespace FNS.Admin.View
{
    public partial class LinksWindow : Window
    {
        private static LinksWindow _instance;

        public static LinksWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LinksWindow();
                    _instance.Closed += OnWindowClosed;
                }

                return _instance;
            }
        }

        private LinksWindow()
        {
            InitializeComponent();
        }

        private static void OnWindowClosed(object? sender, EventArgs e)
        {
            _instance = null;
            Application.Current.MainWindow.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = SystemParameters.PrimaryScreenWidth / 4;
            this.Top = 0;
        }
    }
}
