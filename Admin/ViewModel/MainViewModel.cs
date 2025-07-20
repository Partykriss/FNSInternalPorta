using FNS.Admin.Model;
using System.Collections.ObjectModel;
using FNS.Admin.View;
using System.Windows.Threading;
using System.Windows.Input;
using FNS.Admin.Commands;
using System.Windows;
using FNS.Admin.Services;

namespace FNS.Admin.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<ActionModel> _actions;
        private DispatcherTimer _timer;
        private readonly ApiService _apiService;
        private readonly WindowService _windowService;

        public ObservableCollection<ActionModel> Actions
        {
            get => _actions;
            set
            {
                _actions = value;
                OnPropertyChanged(nameof(Actions));
            }
        }
        
        public ICommand OpenNewsWindowCommand { get; private set; }
        public ICommand OpenLinksWindowCommand { get; private set; }
        public ICommand OpenVotingsWindowsCommand { get; private set; }
        public ICommand CloseCommand { get; }

        public MainViewModel()
        {
            _apiService = new ApiService();
            _windowService = new WindowService();
            LoadLatestActions();
            InitializeTimer();

            OpenNewsWindowCommand = new RelayCommand(OpenNewsWindow);
            OpenLinksWindowCommand = new RelayCommand(OpenLinksWindow);
            OpenVotingsWindowsCommand = new RelayCommand(OpenVotingsWindow);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private void ExecuteClose()
        {
            _windowService.CloseWindow(this);
        }

        private void OpenLinksWindow()
        {
            LinksWindow linksWindow = LinksWindow.Instance;
            linksWindow.Owner = Application.Current.MainWindow;
            linksWindow.Show();
            linksWindow.Activate();
        }

        private void OpenNewsWindow()
        {
            NewsWindow newsWindow = NewsWindow.Instance;
            newsWindow.Owner = Application.Current.MainWindow;
            newsWindow.Show();
            newsWindow.Activate();
        }

        private void OpenVotingsWindow()
        {
            VotingsWindow votingsWindow = VotingsWindow.Instance;
            votingsWindow.Owner = Application.Current.MainWindow;
            votingsWindow.Show();
            votingsWindow.Activate();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += async (sender, e) =>
            {
                await LoadLatestActions();
            };
            _timer.Start();
        }

        private async Task LoadLatestActions()
        {
            var actions = await _apiService.GetAsync<ActionModel>(GlobalSettings.LastActionsApi);
            Actions = new ObservableCollection<ActionModel>(actions);
        }
    }
}
