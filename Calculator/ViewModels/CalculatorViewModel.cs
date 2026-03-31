using Calculator.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly EngineeringCalculatorModel _model = new EngineeringCalculatorModel();
        private string _display = string.Empty;
        private bool _isExtendedLayout = false; // По умолчанию false (обычный режим)

        public string Display
        {
            get => _display;
            set { _display = value; OnPropertyChanged(); }
        }

        public bool IsExtendedLayout
        {
            get => _isExtendedLayout;
            set { _isExtendedLayout = value; OnPropertyChanged(); }
        }

        public ICommand InputCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ToggleLayoutCommand { get; }

        public CalculatorViewModel()
        {
            InputCommand = new RelayCommand(p => OnInput(p?.ToString() ?? ""));
            ClearCommand = new RelayCommand(_ => Display = string.Empty);
            BackspaceCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrEmpty(Display))
                    Display = Display.Substring(0, Display.Length - 1);
            });
            CalculateCommand = new RelayCommand(_ => OnCalculate());
            ToggleLayoutCommand = new RelayCommand(_ => IsExtendedLayout = !IsExtendedLayout);
        }

        private void OnInput(string token)
        {
            if (string.IsNullOrEmpty(token)) return;

            // если сейчас отображается "Error" — очищаем при вводе
            if (Display == "Error") Display = string.Empty;

            Display += token;
        }

        private void OnCalculate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Display)) return;

                double result = _model.Evaluate(Display);
                Display = result.ToString("G15", CultureInfo.InvariantCulture);
            }
            catch
            {
                Display = "Error";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Func<object?, bool>? _canExecute;

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object? parameter) => _execute(parameter);
        }
    }
}