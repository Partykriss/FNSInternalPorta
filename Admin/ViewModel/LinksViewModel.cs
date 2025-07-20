using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using FNS.Admin.Model;
using System.Windows.Input;
using FNS.Admin.Commands;
using System.Windows;
using FNS.Admin.View;
using FNS.Admin.Services;

namespace FNS.Admin.ViewModel
{
    public class LinksViewModel : ObservableObject, IDropTarget
    {
        public Action CloseAction { get; set; }

        private ObservableCollection<LinkModel> _link;
        private LinkModel _selectedLink;

        private string _title;
        private string _url;

        private readonly ApiService _apiService;
        private readonly WindowService _windowService;

        public ObservableCollection<LinkModel> Links
        {
            get => _link;
            set
            {
                _link = value;
                OnPropertyChanged(nameof(Links));
            }
        }

        public LinkModel SelectedLink
        {
            get => _selectedLink;
            set
            {
                _selectedLink = value;
                OnPropertyChanged(nameof(SelectedLink));
                (DeleteLinkCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }


        public ICommand AddLinkCommand { get; private set; }
        public ICommand DeleteLinkCommand { get; private set; }
        public ICommand ChangedLinksCommand {  get; private set; }
        public ICommand PublishLinkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public LinksViewModel()
        {
            _apiService = new ApiService();
            _windowService = new WindowService();
            LoadLinks();

            AddLinkCommand = new RelayCommand(async () => await AddLink());
            DeleteLinkCommand = new RelayCommand(async () => await DeleteLink());
            ChangedLinksCommand = new RelayCommand(async () => await ChangedLinks());
            PublishLinkCommand = new RelayCommand(ExecuteLinkPublish);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private async void ExecuteLinkPublish()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Url))
            {
                MessageBox.Show("Заголовок и адрес ссылки должны быть заполнены.");
                return;
            }

            var allLinks = await _apiService.GetAsync<LinkModel>(GlobalSettings.LinksApi);

            int maxOrder = 0;

            if (allLinks != null && allLinks.Count > 0)
                maxOrder = allLinks
                       .OrderByDescending(link => link.DisplayOrder)
                       .First()
                       .DisplayOrder;

            var linkItem = new LinkModel
            {
                Title = this.Title,
                Url = this.Url,
                DisplayOrder = ++maxOrder
            };

            var isSuccess = await _apiService.PostAsync(GlobalSettings.LinksApi, linkItem);

            if (isSuccess)
            {
                MessageBox.Show($"Ссылка {linkItem.Title} на ресурс {linkItem.Url} успешно опубликована."); ;
                if (Application.Current.Windows.OfType<AddLinkDialog>().FirstOrDefault() is AddLinkDialog dialog)
                {
                    dialog.DialogResult = true;
                }
                ExecuteCancel();
            }
            else
            {
                MessageBox.Show("Ошибка при публикации ссылки.");
            }
        }

        private void ExecuteCancel()
        {
            CloseAction?.Invoke();
        }

        private async Task AddLink()
        {
            var addLinkDialog = new AddLinkDialog(this);
            addLinkDialog.Owner = Application.Current.MainWindow;

            await Task.Run(() =>
            {
                addLinkDialog.Dispatcher.Invoke(() =>
                {
                    var dialogResult = addLinkDialog.ShowDialog();

                    if (dialogResult == true)
                        LoadLinks();
                });
            });
        }

        private async Task DeleteLink()
        {
            if (_windowService.CanPrepare(SelectedLink)) 
                if (_windowService.ConfirmService($"Удалить ссылку {SelectedLink.Title} на ресурс {SelectedLink.Url}?"))
                {
                    var isDeleted = await _apiService.DeleteAsync(GlobalSettings.LinksApi, SelectedLink.Id);
                    if (isDeleted)
                    {
                        Links.Remove(SelectedLink);
                        SelectedLink = null;
                    }
                }
        }


        private async Task ChangedLinks()
        {
            if (_windowService.ConfirmService("Подтыверждаете изменение порядка ссылок?"))
            {
                foreach (var link in Links)
                {
                    await _apiService.PutAsync<LinkModel>(GlobalSettings.LinksApi, link.Id, link);
                }
            }
        }

        private async Task LoadLinks()
        {
            var allLinks = await _apiService.GetAsync<LinkModel>(GlobalSettings.LinksApi);
            var orderedLinks = allLinks
                                 .OrderBy(link => link.DisplayOrder)
                                 .ToList();

            Links = new ObservableCollection<LinkModel>(orderedLinks);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is LinkModel sourceItem && dropInfo.TargetItem is LinkModel targetItem)
            {
                if (sourceItem != targetItem)
                {
                    dropInfo.Effects = System.Windows.DragDropEffects.Move;
                }
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is LinkModel sourceItem && dropInfo.TargetItem is LinkModel targetItem)
            {
                var sourceIndex = Links.IndexOf(sourceItem);
                var targetIndex = Links.IndexOf(targetItem);
                Links.Move(sourceIndex, targetIndex);

                UpdateOrder();
            }
        }

        private void UpdateOrder()
        {
            for (int i = 0; i < Links.Count; i++)
                Links[i].DisplayOrder = i + 1;
        }
    }
}
