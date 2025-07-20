using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using FNS.Admin.ViewModel;
using FNS.Admin.Services;
using System.ComponentModel;
using FNS.Admin.Model;

namespace FNS.Admin.View
{
    public partial class MainWindow : Window
    {
        private HubConnection _hubConnection;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalRConnectionAsync();
            this.Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            var viewModel = this.DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.CloseCommand.Execute(null);
            }
        }

        private async Task InitializeSignalRConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(GlobalSettings.ApiBaseUrl+GlobalSettings.HubApi, options =>
                {
                    options.UseDefaultCredentials = true;
                })
                .Build();

            _hubConnection.On<string>("ReceiveCall", userName =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Получен вызов от " + userName);
                });
            });

            _hubConnection.Closed += async (error) =>
            {
                Console.WriteLine($"Соединение закрыто: {error?.Message}");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            try
            {
                await _hubConnection.StartAsync();
                // Код для обработки успешного подключения
            }
            catch (Exception ex)
            {
                // Обработка ошибок подключения
                MessageBox.Show("Ошибка подключения: " + ex.Message);
            }
        }
    }
}
