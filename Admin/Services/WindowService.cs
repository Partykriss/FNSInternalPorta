using System.Windows;

namespace FNS.Admin.Services
{
    public class WindowService
    {
        private readonly Dictionary<object, Window> _openWindows = new Dictionary<object, Window>();

        public void RegisterWindow(object viewModel, Window window, Window owner = null)
        {
            _openWindows[viewModel] = window;
            if (owner != null)
            {
                window.Owner = owner;
            }
        }

        public void CloseWindow(object viewModel)
        {
            if (_openWindows.TryGetValue(viewModel, out Window window))
            {
                window.Close();
                _openWindows.Remove(viewModel);
            }
        }

        public void CloseChildWindows(Window parent)
        {
            foreach (var window in _openWindows.Values)
            {
                if (window.Owner == parent)
                {
                    window.Close();
                }
            }
        }

        public bool ConfirmService(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public bool CanPrepare(object obj)
        {
            return obj != null;
        }
    }
}
