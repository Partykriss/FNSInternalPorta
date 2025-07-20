using System.Windows;

namespace FNS.Admin.View
{
    public partial class NewsWindow : Window
    {
        private static NewsWindow _instance;

        public static NewsWindow Instance
        {
            get
            {
                if (_instance == null || !_instance.IsLoaded)
                {
                    _instance = new NewsWindow();
                    _instance.Closed += OnWindowClosed;
                }

                return _instance;
            }
        }

        private NewsWindow()
        {
            InitializeComponent();
        }

        private static void OnWindowClosed(object sender, EventArgs e)
        {
            _instance = null;
            Application.Current.MainWindow.Focus();
        }
    }
}
