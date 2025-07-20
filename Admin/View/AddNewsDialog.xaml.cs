using System.Windows;
using FNS.Admin.ViewModel;

namespace FNS.Admin.View
{
    /// <summary>
    /// Логика взаимодействия для AddNewsDialog.xaml
    /// </summary>
    public partial class AddNewsDialog : Window
    {
        public AddNewsDialog(NewsViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CloseAction = new Action (this.Close);
            this.DataContext = viewModel;
        }
    }
}
