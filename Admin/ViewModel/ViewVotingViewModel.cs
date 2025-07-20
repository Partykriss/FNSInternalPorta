using FNS.Admin.Commands;
using FNS.Admin.Model;
using System.Collections.ObjectModel;
using System.Windows;
using FNS.Admin.Services;

namespace FNS.Admin.ViewModel
{
    public class ViewVotingViewModel : ObservableObject
    {
        public Action CloseAction { get; set; }
        private VotingModel _voting;
        private ObservableCollection<AnswerModel> _answers;
        private readonly WindowService _windowService;
        private readonly ApiService _apiService;

        public VotingModel Voting
        {
            get => _voting;
            set
            {
                _voting = value;
                OnPropertyChanged(nameof(Voting));
            }
        }

        public ObservableCollection<AnswerModel> Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                OnPropertyChanged(nameof(Answers));
            }
        }

        public RelayCommand CloseCommand { get; }
        public RelayCommand DeactivateCommand { get; }

        public ViewVotingViewModel(VotingModel voting, ApiService apiService)
        {
            _windowService = new WindowService();
            _apiService = apiService;
            Voting = voting;
            Answers = new ObservableCollection<AnswerModel>(Voting.Answers);

            CloseCommand = new RelayCommand(ExecuteClose);
            DeactivateCommand = new RelayCommand(async () => await ExecuteDeactivateAsync()); 
        }

        private async Task ExecuteDeactivateAsync()
        {
            if (Voting.IsActive && _windowService.ConfirmService($"Деактивировать голосование {Voting.Title}?"))
            {
                var isSuccess = await _apiService.DeactivateVotingAsync(GlobalSettings.VotingsApi, Voting.Id);

                if (isSuccess)
                {
                    MessageBox.Show("Статус голосования обновлен.");

                    OnPropertyChanged(nameof(Voting));
                    OnPropertyChanged(nameof(Answers));
                    ExecuteClose();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении статуса голосования.");
                }
            }                        
        }

        private void ExecuteClose()
        {
            CloseAction?.Invoke();
        }
    }
}