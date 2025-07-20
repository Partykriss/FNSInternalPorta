using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FNS.Admin.Commands;
using FNS.Admin.Model;
using FNS.Admin.Services;
using FNS.Admin.View;

namespace FNS.Admin.ViewModel
{
    public class NewsViewModel : ObservableObject
    {
        public Action CloseAction { get; set; }

        private ObservableCollection<NewsModel> _news;
        private NewsModel _selectedNews;

        private string _title;
        private string _content;

        private readonly ApiService _apiService;
        private readonly WindowService _windowService;

        public ObservableCollection<NewsModel> News
        {
            get => _news;
            set
            {
                _news = value;
                OnPropertyChanged(nameof(News));
            }
        }

        public NewsModel SelectedNews
        {
            get => _selectedNews;
            set
            {
                _selectedNews = value;
                OnPropertyChanged(nameof(SelectedNews));
                (DeleteNewsCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public ICommand AddNewsCommand { get; private set; }
        public ICommand DeleteNewsCommand { get; private set; }
        public ICommand PublishCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand CloseCommand { get; }

        public NewsViewModel()
        {
            _apiService = new ApiService();
            _windowService = new WindowService();
            LoadNews();

            AddNewsCommand = new RelayCommand(async () => await AddNews());
            DeleteNewsCommand = new RelayCommand(async () => await DeleteNews());
            PublishCommand = new RelayCommand(ExecutePublish);
            CancelCommand = new RelayCommand(ExecuteCancel);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private async void ExecutePublish()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
            {
                MessageBox.Show("Заголовок и содержание должны быть заполнены.");
                return;
            }

            var newsItem = new NewsModel
            {
                Title = this.Title,
                Content = this.Content,
                PublishDate = DateTime.Now
            };

            var isSuccess = await _apiService.PostAsync(GlobalSettings.NewsApi, newsItem);

            if (isSuccess)
            {
                MessageBox.Show($"Новость {newsItem.Title} успешно опубликована."); ;
                if (Application.Current.Windows.OfType<AddNewsDialog>().FirstOrDefault() is AddNewsDialog dialog)
                {
                    dialog.DialogResult = true;
                }
                _title = string.Empty;
                _content = string.Empty;
                ExecuteCancel();
            }
            else
            {
                MessageBox.Show("Ошибка при публикации новости.");
            }
        }

        private void ExecuteCancel()
        {
            _title = string.Empty;
            _content = string.Empty;
            CloseAction?.Invoke();
        }

        private void ExecuteClose()
        {
            _windowService.CloseWindow(this);
        }

        private async Task AddNews()
        {
            var addNewsDialog = new AddNewsDialog(this);
            addNewsDialog.Owner = Application.Current.MainWindow;

            await Task.Run(() =>
            {
                addNewsDialog.Dispatcher.Invoke(() =>
                {
                    var dialogResult = addNewsDialog.ShowDialog();

                    if (dialogResult == true)
                        LoadNews();
                });
            });         
        }

        private async Task DeleteNews()
        {
            if (_windowService.CanPrepare(SelectedNews) && _windowService.ConfirmService($"Удалить новость {SelectedNews.Title} от {SelectedNews.PublishDate}?"))
            {
                if (await _apiService.DeleteAsync(GlobalSettings.NewsApi, SelectedNews.Id))
                {
                    News.Remove(SelectedNews);
                    SelectedNews = null;
                }
            }
        }

        private async Task LoadNews()
        {
            var allNews = await _apiService.GetAsync<NewsModel>(GlobalSettings.NewsApi);
            var actuallyNews = allNews
                               .Where(news => news.PublishDate >= DateTime.Now.AddMonths(-1))
                               .OrderByDescending(news => news.PublishDate)
                               .ToList();

            News = new ObservableCollection<NewsModel>(actuallyNews);
        }
    }
}