using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _asyncExecute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public AsyncRelayCommand(Func<Task> asyncExecute, Func<bool> canExecute = null)
        {
            _asyncExecute = asyncExecute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        private async Task ExecuteAsync()
        {
            await _asyncExecute();
        }
    }
}