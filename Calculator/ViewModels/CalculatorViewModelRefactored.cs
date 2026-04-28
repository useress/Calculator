using Calculator.Models;
using Calculator.Models.Commands;
using Calculator.Models.Facades;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    /// <summary>
    /// Refactored ViewModel using Facade Pattern
    /// Demonstrates clean separation between UI and business logic
    /// </summary>
    public class CalculatorViewModelRefactored : INotifyPropertyChanged
    {
        private readonly CalculatorFacade _calculatorFacade;
        private bool _canUndo;
        private bool _canRedo;

        public string Display
        {
            get => _calculatorFacade.Display;
            set => _calculatorFacade.Display = value;
        }

        public bool IsExtendedLayout
        {
            get => _calculatorFacade.IsExtendedLayout;
            set => _calculatorFacade.IsExtendedLayout = value;
        }

        public bool CanUndo
        {
            get => _canUndo;
            set { _canUndo = value; OnPropertyChanged(); }
        }

        public bool CanRedo
        {
            get => _canRedo;
            set { _canRedo = value; OnPropertyChanged(); }
        }

        // Commands exposed to UI
        public ICommand InputCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ToggleLayoutCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public CalculatorViewModelRefactored(CalculatorFacade calculatorFacade)
        {
            _calculatorFacade = calculatorFacade ?? throw new ArgumentNullException(nameof(calculatorFacade));

            // Subscribe to facade events
            _calculatorFacade.StateChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            _calculatorFacade.UndoRedoChanged += (s, e) =>
            {
                CanUndo = _calculatorFacade.CanUndo;
                CanRedo = _calculatorFacade.CanRedo;
            };

            // Initialize commands
            InputCommand = new RelayCommand(p => _calculatorFacade.Input(p?.ToString() ?? ""));
            ClearCommand = new RelayCommand(_ => _calculatorFacade.Clear());
            BackspaceCommand = new RelayCommand(_ => _calculatorFacade.Backspace());
            CalculateCommand = new RelayCommand(_ => _calculatorFacade.Calculate());
            ToggleLayoutCommand = new RelayCommand(_ => _calculatorFacade.ToggleLayout());
            UndoCommand = new RelayCommand(_ => _calculatorFacade.Undo(), _ => _calculatorFacade.CanUndo);
            RedoCommand = new RelayCommand(_ => _calculatorFacade.Redo(), _ => _calculatorFacade.CanRedo);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// RelayCommand implementation for WPF
        /// </summary>
        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object parameter) => _execute(parameter);
        }
    }
}
