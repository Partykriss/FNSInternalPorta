using System.Windows;

namespace FNS.Admin.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
