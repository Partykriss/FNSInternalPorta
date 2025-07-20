using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FNS.Admin.Commands;
using FNS.Admin.Model;
using FNS.Admin.Services;

namespace FNS.Admin.ViewModel
{
    public class AddVotingViewModel : ObservableObject
    {
        public Action CloseAction { get; set; }
        private string _title;
        private string _description;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now.AddDays(7);
        private ObservableCollection<AnswerModel> _answers;
        private readonly ApiService _apiService;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
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

        public ICommand AddVotingCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddAnswerCommand { get; }

        public AddVotingViewModel()
        {
            _apiService = new ApiService();
            Answers = new ObservableCollection<AnswerModel>();

            AddAnswer();
            AddAnswer();

            AddVotingCommand = new RelayCommand(async () => await AddVotingAsync());
            CloseCommand = new RelayCommand(ExecuteClose);
            AddAnswerCommand = new RelayCommand(AddAnswer);
        }

        private void AddAnswer()
        {
            Answers.Add(new AnswerModel());
        }

        private async Task AddVotingAsync()
        {
            var voting = new VotingModel
            {
                Title = this.Title,
                Description = this.Description,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                IsActive = true,
                Answers = Answers.Select(a => new AnswerModel
                {
                    Title = a.Title
                }).ToList()
            };

            var result = await _apiService.PostAsync(GlobalSettings.VotingsApi, voting);
            if (result)
            {
                MessageBox.Show($"Голосование {voting.Title} успешно опубликовано.");
                ExecuteClose();
            }
            else
            {
                MessageBox.Show($"Ошибка при публикации голосования {voting.Title}.");
            }
        }

        private void ExecuteClose()
        {
            CloseAction?.Invoke();
        }
    }
}
