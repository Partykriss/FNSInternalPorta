using System.Collections.ObjectModel;
using System.Windows.Input;
using FNS.Admin.Commands;
using System.Windows;
using FNS.Admin.Model;
using FNS.Admin.View;
using FNS.Admin.Services;

namespace FNS.Admin.ViewModel
{
    public class VotingsViewModel : ObservableObject
    {
        private ObservableCollection<VotingModel> _votings;
        private VotingModel _selectedVoting;
        private readonly ApiService _apiService;
        private readonly WindowService _windowService;

        public ObservableCollection<VotingModel> Votings
        {
            get => _votings;
            set
            {
                _votings = value;
                OnPropertyChanged(nameof(Votings));
            }
        }

        public VotingModel SelectedVoting
        {
            get => _selectedVoting;
            set
            {
                _selectedVoting = value;
                OnPropertyChanged(nameof(SelectedVoting));
            }
        }

        public ICommand AddVotingCommand { get; private set; }
        public ICommand ViewVotingCommand { get; private set; }

        public VotingsViewModel()
        {
            _apiService = new ApiService();
            _windowService = new WindowService();
            LoadVotings();

            AddVotingCommand = new RelayCommand(async () => await AddVoting());
            ViewVotingCommand = new RelayCommand(async () => await ViewVoting());
        }

        private async Task LoadVotings()
        {
            var votings = await _apiService.GetAsync<VotingModel>(GlobalSettings.VotingsApi);
            var recentVotings = votings
                .Where(v => v.EndDate >= DateTime.Now.AddMonths(-12))
                .OrderByDescending(v => v.EndDate)
                .ToList();

            Votings = new ObservableCollection<VotingModel>(recentVotings);
        }

        private async Task AddVoting()
        {
            var addVotingWindow = new AddVotingWindow();
            addVotingWindow.Owner = Application.Current.MainWindow;

            await Task.Run (() =>
            {
                addVotingWindow.Dispatcher.Invoke(() =>
                {
                    addVotingWindow.ShowDialog();
                });
            });
        }

        private async Task ViewVoting()
        {
            if (CanPrepareVoting())
            {
                var viewVotingWindow = new ViewVotingWindow(SelectedVoting, _apiService);
                viewVotingWindow.Owner = Application.Current.MainWindow;

                viewVotingWindow.Closed += async (sender, args) =>
                {
                    await LoadVotings();
                };

                await Task.Run(() =>
                {
                    viewVotingWindow.Dispatcher.Invoke(() =>
                    {
                        viewVotingWindow.ShowDialog();
                    });
                });
            }
        }

        private bool CanPrepareVoting() => SelectedVoting != null;
    }
}
