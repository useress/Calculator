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
            ClearCommand = new RelayCommand(_ =>
            {
                try
                {
                    Display = string.Empty;
                }
                catch { }
            });
            BackspaceCommand = new RelayCommand(_ =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(Display) && Display.Length > 0)
                    {
                        Display = Display.Substring(0, Display.Length - 1);
                    }
                }
                catch { }
            });
            CalculateCommand = new RelayCommand(_ => OnCalculate());
            ToggleLayoutCommand = new RelayCommand(_ => IsExtendedLayout = !IsExtendedLayout);
        }

        private void OnInput(string token)
        {
            try
            {
                // Validate token - null or empty is invalid
                if (string.IsNullOrEmpty(token))
                {
                    // Don't process empty input
                    return;
                }

                // If currently showing "Error" or null display — clear it when user starts typing
                if (Display == "Error" || Display == null)
                {
                    Display = string.Empty;
                }

                // Append the token to the current display
                string newDisplay = Display + token;
                
                // Safety check: ensure new display is not null or empty before setting
                if (!string.IsNullOrEmpty(newDisplay))
                {
                    Display = newDisplay;
                }
            }
            catch (Exception _)
            {
                // SafetyGuard: Don't let any exception break input
                // Just silently ignore and keep display as-is
                System.Diagnostics.Debug.WriteLine("OnInput exception occurred");
            }
        }

        private void OnCalculate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Display)) return;

                double result = _model.Evaluate(Display);
                
                // Format the result to avoid truncation and scientific notation issues
                // Use proper decimal precision
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    Display = "Error";
                    return;
                }
                
                if (result == 0)
                {
                    Display = "0";
                    return;
                }
                
                // Use F format with 10 decimal places, then remove trailing zeros
                string formatted = result.ToString("F10", CultureInfo.InvariantCulture);
                
                // Remove trailing zeros after decimal point
                if (formatted.Contains("."))
                {
                    formatted = formatted.TrimEnd('0').TrimEnd('.');
                }
                
                // Ensure formatted string is not empty
                if (string.IsNullOrEmpty(formatted) || formatted == "-")
                {
                    formatted = "0";
                }
                
                // If the result is very large or very small, use G format but with better precision
                if (formatted.Length > 15 || double.Abs(result) > 1e10 || (double.Abs(result) < 1e-5 && result != 0))
                {
                    formatted = result.ToString("G10", CultureInfo.InvariantCulture);
                }
                
                // Final safety check
                if (string.IsNullOrEmpty(formatted))
                {
                    Display = result.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    Display = formatted;
                }
            }
            catch (Exception _)
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