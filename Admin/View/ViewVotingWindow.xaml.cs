using FNS.Admin.Model;
using System.Windows;
using FNS.Admin.ViewModel;


namespace FNS.Admin.View
{
    public partial class ViewVotingWindow : Window
    {
        public ViewVotingWindow(VotingModel selectedVoting, ApiService apiService)
        {
            InitializeComponent();
            var viewModel = new ViewVotingViewModel(selectedVoting, apiService);
            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
        }
    }
}
