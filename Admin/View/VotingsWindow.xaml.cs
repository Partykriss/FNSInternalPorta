using System.Windows;

namespace FNS.Admin.View
{
    public partial class VotingsWindow : Window
    {
        private static VotingsWindow _instance;
        public static VotingsWindow Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new VotingsWindow();
                    _instance.Closed += OnWindowClosed;
                }

                return _instance;
            }
        }

        private VotingsWindow()
        {
            InitializeComponent();
        }

        private static void OnWindowClosed(object? sender, EventArgs e)
        {
            _instance = null;
            Application.Current.MainWindow.Focus();
        }
    }
}
