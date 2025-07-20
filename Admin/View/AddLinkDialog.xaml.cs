using FNS.Admin.ViewModel;
using System.Windows;

namespace FNS.Admin.View
{
    public partial class AddLinkDialog : Window
    {
        public AddLinkDialog(LinksViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
        }
    }
}
