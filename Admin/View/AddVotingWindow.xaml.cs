using FNS.Admin.ViewModel;
using System.Windows;

namespace FNS.Admin.View
{
    public partial class AddVotingWindow : Window
    {
        public AddVotingWindow()
        {
            InitializeComponent();
            var viewModel = new AddVotingViewModel();
            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
        }
    }
}
