using Calculator.Models;
using Calculator.Models.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    /// <summary>
    /// ViewModel for the calculator application
    /// Uses the Command Pattern to manage all calculator operations
    /// Supports undo/redo through CommandInvoker
    /// </summary>
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly EngineeringCalculatorModel _model = new EngineeringCalculatorModel();
        private readonly CalculatorState _state = new CalculatorState();
        private readonly CommandInvoker _commandInvoker = new CommandInvoker();
        private bool _canUndo;
        private bool _canRedo;

        public string Display
        {
            get => _state.Display;
            set => _state.Display = value;
        }

        public bool IsExtendedLayout
        {
            get => _state.IsExtendedLayout;
            set => _state.IsExtendedLayout = value;
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

        public ICommand InputCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ToggleLayoutCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public CalculatorViewModel()
        {
            // Setup state change notifications
            _state.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

            // Setup command invoker events
            _commandInvoker.UndoRedoChanged += (s, e) =>
            {
                CanUndo = _commandInvoker.CanUndo;
                CanRedo = _commandInvoker.CanRedo;
            };

            // Initialize commands using the Command Pattern
            InputCommand = new RelayCommand(p => ExecuteInputCommand(p?.ToString() ?? ""));
            ClearCommand = new RelayCommand(_ => ExecuteClearCommand());
            BackspaceCommand = new RelayCommand(_ => ExecuteBackspaceCommand());
            CalculateCommand = new RelayCommand(_ => ExecuteCalculateCommand());
            ToggleLayoutCommand = new RelayCommand(_ => ExecuteToggleLayoutCommand());
            UndoCommand = new RelayCommand(_ => _commandInvoker.Undo(), _ => _commandInvoker.CanUndo);
            RedoCommand = new RelayCommand(_ => _commandInvoker.Redo(), _ => _commandInvoker.CanRedo);
        }

        /// <summary>
        /// Execute input command through the command invoker
        /// </summary>
        private void ExecuteInputCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            var command = new InputCommand(_state, input);
            _commandInvoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Execute clear command through the command invoker
        /// </summary>
        private void ExecuteClearCommand()
        {
            var command = new ClearCommand(_state);
            _commandInvoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Execute backspace command through the command invoker
        /// </summary>
        private void ExecuteBackspaceCommand()
        {
            var command = new BackspaceCommand(_state);
            _commandInvoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Execute calculate command through the command invoker
        /// </summary>
        private void ExecuteCalculateCommand()
        {
            var command = new CalculateCommand(_state, _model);
            _commandInvoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Execute toggle layout command through the command invoker
        /// </summary>
        private void ExecuteToggleLayoutCommand()
        {
            var command = new ToggleLayoutCommand(_state);
            _commandInvoker.ExecuteCommand(command);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// RelayCommand implementation for WPF
        /// </summary>
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